# SCRUM-35 - Documentacion Tecnica DynamicUiTableBuilder

## Objetivo

Este documento describe el comportamiento tecnico del componente `DynamicUiTableBuilder` y su consumo desde servicios internos, tomando como base la implementacion de SCRUM-34.

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
/// Parametros de construccion:
/// - Request: llega desde controller/service (TableId, paginacion, sort, claims, alias).
/// - Rows/Total: provienen del handler de negocio por TableId.
/// - Actions/CellActions: provienen del handler.
/// - Columns: columnas fijas del handler o de request.
/// </param>
/// <returns>
/// DynamicUiTableDto con:
/// - Columns ordenadas por "Order"
/// - Columna de acciones inyectada si no existe
/// - Rows mapeadas a UiRowDto
/// - Actions separadas por placement (toolbar/bulk/row)
/// - Pagination y Sorting
/// </returns>
```

Notas de comportamiento:
- Si `UseColumnConfigFromDb=true`, consulta columnas en `IUiTableConfigRepository` usando `DefaultDbAlias` + `TableId`.
- Si no existe columna de acciones, agrega `actions`/`Opciones` con `IsActionColumn=true`.

### `BuildRowsOnlyAsync(DynamicUiTableBuildInput input)`

```xml
/// <summary>
/// Construye una respuesta liviana con filas y paginacion, sin metadata completa de configuracion.
/// </summary>
/// <param name="input">
/// Input armado por servicio con Rows, Total y parametros de paginacion.
/// </param>
/// <returns>
/// DynamicUiRowsOnlyDto con TableId, Rows mapeadas y Pagination.
/// </returns>
```

### `ToUiRow(object row)` (funcion interna clave)

```xml
/// <summary>
/// Convierte una fila de dominio/anonima en UiRowDto para consumo uniforme del frontend.
/// </summary>
/// <param name="row">
/// Puede llegar como Dictionary<string, object?> o como objeto con propiedades.
/// </param>
/// <returns>
/// UiRowDto con:
/// - Id: valor de la clave/propiedad "id" (si existe)
/// - Values: mapa completo de columnas y valores
/// </returns>
```

## Consumo del servicio desde otro servicio interno

### Flujo recomendado

1. Controller valida claims y completa `DefaultDbAlias`.
2. Controller deriva claims del token (`role`, `permission`, `permiso`) y los asigna a `UserClaims`.
3. Servicio consumidor arma `DynamicUiTableQueryRequestDto`.
4. Servicio consumidor invoca:
   - `IDynamicUiTableService.QueryAsync(req)` o
   - `IDynamicUiTableService.ExecuteActionAsync(req)`.

### Parametros y origen

- `TableId`: lo define backend por modulo/feature.
- `DefaultDbAlias`: claim `defaulalias` del token.
- `UserClaims`: claims del token (`role/permission/permiso`).
- `Page/PageSize/SortField/SortDir`: request de frontend validado por backend.

### Retornos esperados

- `AppResponses<object>` con `data` tipo:
  - `DynamicUiTableDto` si `IncludeConfig=true`
  - `DynamicUiRowsOnlyDto` si `IncludeConfig=false`
- En validaciones fallidas: `success=false` + `errors` con `Type="Validation"`.

## Ejemplo completo (tabla `ra_rad_estados_modulo_radicacion`)

### Request de consulta

```json
{
  "tableId": "radicadosPendientes",
  "page": 1,
  "pageSize": 25,
  "sortField": "fecha_registro",
  "sortDir": "asc",
  "search": "",
  "includeConfig": true,
  "useColumnConfigFromDb": true
}
```

### Response de referencia

```json
{
  "success": true,
  "message": "OK",
  "errors": [],
  "data": {
    "tableId": "radicadosPendientes",
    "title": "radicadosPendientes",
    "columns": [
      { "key": "consecutivo_radicado", "headerName": "Radicado", "sortable": true, "order": 1 },
      { "key": "remitente", "headerName": "Remitente", "sortable": true, "order": 2 },
      { "key": "fecha_registro", "headerName": "Fecha Registro", "sortable": true, "order": 3 },
      { "key": "actions", "headerName": "Opciones", "sortable": false, "isActionColumn": true, "order": 99 }
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

## Ejemplo de consumo interno (C#)

```csharp
public sealed class RadicadosPendientesFacade
{
    private readonly IDynamicUiTableService _dynamicUiTableService;

    public RadicadosPendientesFacade(IDynamicUiTableService dynamicUiTableService)
    {
        _dynamicUiTableService = dynamicUiTableService;
    }

    public Task<AppResponses<object>> ConsultarAsync(string defaultDbAlias, List<string> claims)
    {
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
            UseColumnConfigFromDb = true
        };

        return _dynamicUiTableService.QueryAsync(req);
    }
}
```
