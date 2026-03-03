# SCRUM-35 - Documento Tecnico para Desarrollador: DynamicUiTableBuilder

## Objetivo

Este documento describe el comportamiento tecnico de `DynamicUiTableBuilder` y como consumir `IDynamicUiTableService` desde servicios internos, incluyendo:
- construccion de columnas para `FixedColumns`
- definicion de eventos/acciones (`UiActionDto` / `UiCellActionDto`)
- ejemplo end-to-end con datos de `ra_rad_estados_modulo_radicacion`

## Ubicacion del componente

- Servicio: `MiApp.Services/Service/UI/MuiTable/DynamicUiTableService.cs`
- Builder: `DynamicUiTableBuilder`
- Contratos DTO: `MiApp.DTOs/DTOs/UI/MuiTable/DynamicUiTableDtos.cs`

## Funciones publicas (XML-style)

### `BuildAsync(DynamicUiTableBuildInput input)`

```xml
/// <summary>
/// Construye la respuesta completa de tabla dinamica (configuracion + filas + acciones).
/// </summary>
/// <param name="input">
/// Request: TableId, paginacion, sort, claims, alias.
/// Rows/Total: salida del handler de negocio por TableId.
/// Actions/CellActions: definiciones de eventos del handler.
/// Columns: columnas fijas o columnas desde DB.
/// </param>
/// <returns>
/// DynamicUiTableDto con Columns ordenadas, Rows mapeadas,
/// acciones separadas por placement y metadata de Pagination/Sorting.
/// </returns>
```

Notas de comportamiento:
- Si `UseColumnConfigFromDb=true`, consulta columnas en `IUiTableConfigRepository` usando `DefaultDbAlias` + `TableId`.
- Si no existe columna de acciones, agrega automaticamente `actions` (`Opciones`).

### `BuildRowsOnlyAsync(DynamicUiTableBuildInput input)`

```xml
/// <summary>
/// Construye una respuesta liviana con filas y paginacion.
/// </summary>
/// <param name="input">Input armado por servicio con Rows y Total.</param>
/// <returns>DynamicUiRowsOnlyDto.</returns>
```

### `ToUiRow(object row)` (funcion interna clave)

```xml
/// <summary>
/// Convierte una fila de dominio/anonima a UiRowDto.
/// </summary>
/// <param name="row">Dictionary<string, object?> o POCO.</param>
/// <returns>
/// UiRowDto con Id (campo "id") y Values (mapa completo de columnas).
/// </returns>
```

## Consumo interno recomendado

Flujo:
1. Controller valida claim `defaulalias`.
2. Controller extrae claims de seguridad (`role`, `permission`, `permiso`).
3. Servicio interno arma `DynamicUiTableQueryRequestDto`.
4. Servicio interno llama `QueryAsync` o `ExecuteActionAsync`.

Origen de parametros:
- `TableId`: definido por backend/modulo.
- `DefaultDbAlias`: claim `defaulalias`.
- `UserClaims`: claims del token.
- `Page/PageSize/SortField/SortDir`: request de frontend.

Retornos esperados:
- `AppResponses<object>` con `data`:
  - `DynamicUiTableDto` si `IncludeConfig=true`
  - `DynamicUiRowsOnlyDto` si `IncludeConfig=false`
- Error controlado: `success=false` y `errors` (`Validation`/`Exception`).

## Como construir columnas para pasarlas como parametro

Cuando quieres forzar columnas desde servicio/handler (sin depender de `ui_table_columns`), usa `FixedColumns`:

```csharp
var fixedColumns = new List<UiColumnDto>
{
    new()
    {
        Key = "consecutivo_radicado",
        ColumnName = "consecutivo_radicado",
        HeaderName = "Radicado",
        DataType = "text",
        RenderType = "grid_text",
        Visible = true,
        Sortable = true,
        Order = 1,
        Width = 180,
        Align = "left"
    },
    new()
    {
        Key = "remitente",
        ColumnName = "remitente",
        HeaderName = "Remitente",
        DataType = "text",
        RenderType = "grid_text",
        Visible = true,
        Sortable = true,
        Order = 2,
        Width = 220,
        Align = "left"
    },
    new()
    {
        Key = "fecha_registro",
        ColumnName = "fecha_registro",
        HeaderName = "Fecha",
        DataType = "datetime",
        RenderType = "grid_datetime",
        Visible = true,
        Sortable = true,
        Order = 3,
        Width = 180,
        Align = "left"
    },
    new()
    {
        Key = "estado",
        ColumnName = "estado",
        HeaderName = "Estado",
        DataType = "number",
        RenderType = "grid_chip",
        Visible = true,
        Sortable = true,
        Order = 4,
        Width = 120,
        Align = "center"
    }
};

var req = new DynamicUiTableQueryRequestDto
{
    TableId = "radicadosPendientes",
    DefaultDbAlias = defaultDbAlias,
    UserClaims = claims,
    Page = 1,
    PageSize = 25,
    SortField = "fecha_registro",
    SortDir = "asc",
    IncludeConfig = true,
    UseColumnConfigFromDb = false,
    FixedColumns = fixedColumns
};
```

## Como crear el campo de evento (acciones)

En el modelo actual, el "campo de evento" se modela con `UiActionDto`:
- `Behavior`: tipo de evento/accion
- `BehaviorConfig`: configuracion del evento
- `Placement`: donde se muestra (`toolbar`, `bulk`, `row`)
- `Presentation`: como se pinta (`button`, `menu_item`, `icon`)

### Tipos de evento recomendados (convencion)

No hay enum estricto en backend; `Behavior` es string. Convenciones recomendadas:
- `api_call`
- `navigate`
- `modal`
- `download`
- `emit`
- `custom`

### Ejemplos por tipo de evento

```csharp
var toolbarRefresh = new UiActionDto
{
    ActionId = "refresh",
    Label = "Refrescar",
    Placement = "toolbar",
    Presentation = "button",
    Behavior = "api_call",
    BehaviorConfig = new Dictionary<string, object?>
    {
        ["method"] = "POST",
        ["url"] = "/api/ui/dynamic-table/query",
        ["requery"] = true
    },
    Icon = "refresh",
    Tone = "primary"
};

var rowOpen = new UiActionDto
{
    ActionId = "openDetail",
    Label = "Abrir",
    Placement = "row",
    Presentation = "icon",
    Behavior = "navigate",
    BehaviorConfig = new Dictionary<string, object?>
    {
        ["route"] = "/radicacion/radicado/{id_estado_radicado}",
        ["target"] = "self"
    },
    Icon = "open_in_new",
    Tone = "neutral"
};

var rowAssign = new UiActionDto
{
    ActionId = "asignar",
    Label = "Asignar",
    Placement = "row",
    Presentation = "menu_item",
    Behavior = "modal",
    BehaviorConfig = new Dictionary<string, object?>
    {
        ["modalId"] = "asignar-radicado",
        ["size"] = "md"
    },
    RequiresConfirm = false,
    RequiredClaimsAny = new List<string> { "radicacion.asignar" },
    Rules = new List<RowRuleDto>
    {
        new() { Type = "equals", Field = "estado", Value = "1" }
    }
};

var rowDownload = new UiActionDto
{
    ActionId = "downloadPdf",
    Label = "Descargar",
    Placement = "row",
    Presentation = "icon",
    Behavior = "download",
    BehaviorConfig = new Dictionary<string, object?>
    {
        ["url"] = "/api/radicados/{id_estado_radicado}/pdf",
        ["fileName"] = "radicado-{consecutivo_radicado}.pdf"
    },
    Icon = "download",
    Tone = "info"
};

var bulkClose = new UiActionDto
{
    ActionId = "cerrarMasivo",
    Label = "Cerrar seleccion",
    Placement = "bulk",
    Presentation = "button",
    Behavior = "emit",
    BehaviorConfig = new Dictionary<string, object?>
    {
        ["eventName"] = "bulk-close-radicados"
    },
    RequiresConfirm = true,
    ConfirmTitle = "Confirmar cierre masivo",
    ConfirmMessage = "Se cerraran los radicados seleccionados",
    RequiredClaimsAll = new List<string> { "radicacion.cerrar", "radicacion.bulk" }
};

var rowCustom = new UiActionDto
{
    ActionId = "customAction",
    Label = "Accion custom",
    Placement = "row",
    Presentation = "menu_item",
    Behavior = "custom",
    BehaviorConfig = new Dictionary<string, object?>
    {
        ["handler"] = "frontendCustomHandler",
        ["payloadMode"] = "row"
    }
};

var cellWhatsApp = new UiCellActionDto
{
    ColumnKey = "remitente",
    Action = new UiActionDto
    {
        ActionId = "contactar",
        Label = "Contactar",
        Placement = "row",
        Presentation = "icon",
        Behavior = "navigate",
        BehaviorConfig = new Dictionary<string, object?>
        {
            ["route"] = "https://wa.me/{telefono}",
            ["target"] = "blank"
        },
        Icon = "chat",
        Tone = "success"
    }
};
```

## Ejemplo de handler completo (columnas + eventos)

```csharp
public sealed class RadicadosPendientesHandler : IDynamicUiTableHandler
{
    public string TableId => "radicadosPendientes";

    public List<UiColumnDto>? GetFixedColumns() => new()
    {
        new() { Key = "consecutivo_radicado", ColumnName = "consecutivo_radicado", HeaderName = "Radicado", Order = 1, Sortable = true },
        new() { Key = "remitente", ColumnName = "remitente", HeaderName = "Remitente", Order = 2, Sortable = true },
        new() { Key = "fecha_registro", ColumnName = "fecha_registro", HeaderName = "Fecha", Order = 3, Sortable = true },
        new() { Key = "estado", ColumnName = "estado", HeaderName = "Estado", Order = 4, Sortable = true }
    };

    public List<UiActionDto> GetActions(DynamicUiTableQueryRequestDto req) => new()
    {
        // toolbar
        new UiActionDto { ActionId = "refresh", Label = "Refrescar", Placement = "toolbar", Behavior = "api_call", BehaviorConfig = new() { ["requery"] = true } },
        // row
        new UiActionDto { ActionId = "openDetail", Label = "Abrir", Placement = "row", Behavior = "navigate", BehaviorConfig = new() { ["route"] = "/radicado/{id_estado_radicado}" } },
        // bulk
        new UiActionDto { ActionId = "cerrarMasivo", Label = "Cerrar", Placement = "bulk", Behavior = "emit", BehaviorConfig = new() { ["eventName"] = "bulk-close-radicados" }, RequiresConfirm = true }
    };

    public List<UiCellActionDto> GetCellActions(DynamicUiTableQueryRequestDto req) => new()
    {
        new UiCellActionDto
        {
            ColumnKey = "remitente",
            Action = new UiActionDto { ActionId = "contactar", Label = "Contactar", Behavior = "navigate", BehaviorConfig = new() { ["route"] = "https://wa.me/{telefono}" } }
        }
    };

    public Task<(List<object> rows, int total)> GetRowsAsync(DynamicUiTableQueryRequestDto req)
    {
        var rows = new List<object>
        {
            new Dictionary<string, object?>
            {
                ["id"] = "101",
                ["id_estado_radicado"] = 101,
                ["consecutivo_radicado"] = "RAD-2026-000123",
                ["remitente"] = "Empresa ABC",
                ["fecha_registro"] = "2026-03-03T09:45:00",
                ["estado"] = 1,
                ["telefono"] = "573001112233"
            }
        };

        return Task.FromResult((rows, 1));
    }

    public Task<AppResponses<object>> ExecuteActionAsync(ExecuteUiActionRequestDto req)
        => Task.FromResult(new AppResponses<object>
        {
            success = true,
            message = "OK",
            errors = [],
            data = new { action = req.ActionId, row = req.RowId }
        });
}
```

## Ejemplo end-to-end (ra_rad_estados_modulo_radicacion)

### Request

```json
{
  "tableId": "radicadosPendientes",
  "page": 1,
  "pageSize": 25,
  "sortField": "fecha_registro",
  "sortDir": "asc",
  "search": "RAD-2026",
  "includeConfig": true,
  "useColumnConfigFromDb": false
}
```

### Response

```json
{
  "success": true,
  "message": "OK",
  "errors": [],
  "data": {
    "tableId": "radicadosPendientes",
    "columns": [
      { "key": "consecutivo_radicado", "headerName": "Radicado", "order": 1 },
      { "key": "remitente", "headerName": "Remitente", "order": 2 },
      { "key": "fecha_registro", "headerName": "Fecha", "order": 3 },
      { "key": "estado", "headerName": "Estado", "order": 4 },
      { "key": "actions", "headerName": "Opciones", "isActionColumn": true, "order": 5 }
    ],
    "rows": [
      {
        "id": "101",
        "values": {
          "id_estado_radicado": 101,
          "consecutivo_radicado": "RAD-2026-000123",
          "remitente": "Empresa ABC",
          "fecha_registro": "2026-03-03T09:45:00",
          "estado": 1
        }
      }
    ],
    "pagination": { "page": 1, "pageSize": 25, "total": 1 },
    "sorting": { "sortField": "fecha_registro", "sortDir": "asc" }
  }
}
```

## Ejemplo completo: consulta especializada con IDapperCrudEngine + DynamicUiTableBuilder

Este es el patron recomendado cuando necesitas consultas de negocio (joins/filtros/paginacion/sort) y luego render UI dinamica.

### 1) Repositorio especializado (consulta con QueryOptions)

```csharp
using MiApp.Repository.DataAccess;

public interface IRadicadosPendientesUiRepository
{
    Task<(List<Dictionary<string, object?>> rows, int total)> QueryAsync(
        string defaultDbAlias,
        int idPlantilla,
        int idUsuarioRadicacion,
        int page,
        int pageSize,
        string? sortField,
        string? sortDir,
        string? search);
}

public sealed class RadicadosPendientesUiRepository : IRadicadosPendientesUiRepository
{
    private readonly IDapperCrudEngine _dapper;

    public RadicadosPendientesUiRepository(IDapperCrudEngine dapper)
    {
        _dapper = dapper;
    }

    public async Task<(List<Dictionary<string, object?>> rows, int total)> QueryAsync(
        string defaultDbAlias,
        int idPlantilla,
        int idUsuarioRadicacion,
        int page,
        int pageSize,
        string? sortField,
        string? sortDir,
        string? search)
    {
        var offset = (page - 1) * pageSize;
        var safeSortField = string.IsNullOrWhiteSpace(sortField) ? "fecha_registro" : sortField;
        var safeSortDir = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

        var options = new QueryOptions
        {
            TableName = "ra_rad_estados_modulo_radicacion",
            Columns =
            [
                "id_estado_radicado",
                "consecutivo_radicado",
                "remitente",
                "fecha_registro",
                "estado",
                "id_usuario_radicado"
            ],
            Filters = new Dictionary<string, object>
            {
                { "system_plantilla_radicado_id_Plantilla", idPlantilla },
                { "id_usuario_radicado", idUsuarioRadicacion },
                { "estado", 1 }
            },
            UseLikeOperator = !string.IsNullOrWhiteSpace(search),
            DefaultAlias = defaultDbAlias,
            OrderByFields = [ new OrderByField { Column = safeSortField, Direction = safeSortDir } ],
            Limit = pageSize,
            Offset = offset
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Busca por consecutivo/remitente usando OR con LIKE
            options.UseOrOperator = true;
            options.Filters["consecutivo_radicado"] = search!;
            options.Filters["remitente"] = search!;
        }

        var query = await _dapper.GetAllAsync<Dictionary<string, object?>>(options);
        if (!query.Success || query.Data == null)
        {
            return (new List<Dictionary<string, object?>>(), 0);
        }

        var rows = query.Data.ToList();
        var normalized = rows.Select(r =>
        {
            // DynamicUiTableBuilder espera "id" como llave principal del row.
            r["id"] = r.TryGetValue("id_estado_radicado", out var v) ? v?.ToString() : string.Empty;
            return r;
        }).ToList();

        var total = query.TotalRecords ?? normalized.Count;
        return (normalized, total);
    }
}
```

### 2) Handler de tabla (conecta repositorio con builder)

```csharp
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.Services.Service.UI.MuiTable;

public sealed class RadicadosPendientesDynamicHandler : IDynamicUiTableHandler
{
    private readonly IRadicadosPendientesUiRepository _repo;

    public RadicadosPendientesDynamicHandler(IRadicadosPendientesUiRepository repo)
    {
        _repo = repo;
    }

    public string TableId => "radicadosPendientes";

    public async Task<(List<object> rows, int total)> GetRowsAsync(DynamicUiTableQueryRequestDto req)
    {
        // Estos valores pueden venir de claims, request o resolucion de negocio.
        var idPlantilla = 10;
        var idUsuarioRadicacion = 123;

        var (rows, total) = await _repo.QueryAsync(
            req.DefaultDbAlias,
            idPlantilla,
            idUsuarioRadicacion,
            req.Page,
            req.PageSize,
            req.SortField,
            req.SortDir,
            req.Search);

        return (rows.Cast<object>().ToList(), total);
    }

    public List<UiColumnDto>? GetFixedColumns() => new()
    {
        new() { Key = "consecutivo_radicado", ColumnName = "consecutivo_radicado", HeaderName = "Radicado", Order = 1, Sortable = true },
        new() { Key = "remitente", ColumnName = "remitente", HeaderName = "Remitente", Order = 2, Sortable = true },
        new() { Key = "fecha_registro", ColumnName = "fecha_registro", HeaderName = "Fecha", Order = 3, Sortable = true },
        new() { Key = "estado", ColumnName = "estado", HeaderName = "Estado", Order = 4, Sortable = true }
    };

    public List<UiActionDto> GetActions(DynamicUiTableQueryRequestDto req) => new()
    {
        new UiActionDto { ActionId = "refresh", Label = "Refrescar", Placement = "toolbar", Behavior = "api_call", BehaviorConfig = new() { ["requery"] = true } },
        new UiActionDto { ActionId = "openDetail", Label = "Abrir", Placement = "row", Behavior = "navigate", BehaviorConfig = new() { ["route"] = "/radicado/{id_estado_radicado}" } }
    };

    public List<UiCellActionDto> GetCellActions(DynamicUiTableQueryRequestDto req) => [];

    public Task<AppResponses<object>> ExecuteActionAsync(ExecuteUiActionRequestDto req)
        => Task.FromResult(new AppResponses<object> { success = true, message = "OK", errors = [], data = new { req.ActionId, req.RowId } });
}
```

### 3) Registro DI

```csharp
builder.Services.AddScoped<IRadicadosPendientesUiRepository, RadicadosPendientesUiRepository>();
builder.Services.AddScoped<IDynamicUiTableHandler, RadicadosPendientesDynamicHandler>();
```

### 4) Resultado final

Con este flujo:
1. `DynamicUiTableController` valida claims (`defaulalias`) y llama `QueryAsync`.
2. `DynamicUiTableService` resuelve el handler por `TableId`.
3. El handler consulta con `IDapperCrudEngine`.
4. `DynamicUiTableBuilder` transforma filas/columnas/acciones al DTO final (`DynamicUiTableDto` o `DynamicUiRowsOnlyDto`).

## Checklist rapido de implementacion

- Definir `TableId` estable por modulo.
- Definir columnas en `GetFixedColumns` o habilitar `UseColumnConfigFromDb`.
- Definir acciones con `Behavior` + `BehaviorConfig` + claims.
- Validar `SortField` contra columnas permitidas.
- Retornar `AppResponses<object>` con errores controlados.
