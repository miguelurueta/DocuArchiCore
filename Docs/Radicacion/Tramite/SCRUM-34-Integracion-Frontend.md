# SCRUM-34 - Guia de Integracion Frontend

## Endpoints

- Metodo: `POST`
- URL base query: `/api/ui/dynamic-table/query`
- URL base action: `/api/ui/dynamic-table/action`
- Wrappers por modulo:
  - `/api/{modulo}/ui/table/query`
  - `/api/{modulo}/ui/table/action`
- Requiere `Bearer token` con claim `defaulalias`.

## Request query (frontend -> backend)

```json
{
  "tableId": "default",
  "page": 1,
  "pageSize": 25,
  "sortField": "id",
  "sortDir": "asc",
  "search": "",
  "includeConfig": true,
  "useColumnConfigFromDb": true,
  "fixedColumns": []
}
```

Notas:
- `defaultDbAlias` no se envia desde frontend; el backend lo toma del claim `defaulalias`.
- Si usas wrapper por modulo, puedes omitir `tableId`; el backend usa `{modulo}` como fallback.
- `sortField` debe existir dentro de columnas permitidas; si no, retorna validacion.

## Request action

```json
{
  "tableId": "default",
  "actionId": "openDetail",
  "rowId": "123",
  "columnKey": "acciones",
  "selectedRowIds": ["123", "124"],
  "payload": {
    "motivo": "Aprobado"
  }
}
```

## Contrato de respuesta

### 1) Query con `includeConfig=true`

`data` contiene `DynamicUiTableDto`:

```json
{
  "success": true,
  "message": "OK",
  "errors": [],
  "data": {
    "tableId": "default",
    "title": "default",
    "columns": [
      {
        "key": "id",
        "columnName": "id",
        "headerName": "Id",
        "dataType": "text",
        "renderType": "grid_text",
        "visible": true,
        "sortable": true,
        "order": 1,
        "width": 120,
        "align": "left",
        "isActionColumn": false
      }
    ],
    "rows": [
      {
        "id": "123",
        "values": {
          "id": "123",
          "asunto": "Radicado de prueba"
        },
        "meta": null
      }
    ],
    "toolbarActions": [],
    "bulkActions": [],
    "rowActions": [],
    "cellActions": [],
    "userClaims": ["radicacion.consultar"],
    "pagination": {
      "page": 1,
      "pageSize": 25,
      "total": 1
    },
    "sorting": {
      "sortField": "id",
      "sortDir": "asc"
    }
  }
}
```

### 2) Query con `includeConfig=false`

`data` contiene `DynamicUiRowsOnlyDto`:

```json
{
  "success": true,
  "message": "OK",
  "errors": [],
  "data": {
    "tableId": "default",
    "rows": [
      {
        "id": "123",
        "values": {
          "id": "123",
          "asunto": "Radicado de prueba"
        }
      }
    ],
    "pagination": {
      "page": 1,
      "pageSize": 25,
      "total": 1
    }
  }
}
```

### 3) Sin resultados

```json
{
  "success": true,
  "message": "Sin resultados",
  "errors": [],
  "data": null
}
```

### 4) Errores de validacion esperados

- `TableId requerido`
- `Parametros de paginacion invalidos`
- `No existe handler para TableId`
- `SortField no permitido para TableId`
- `TableId y ActionId requeridos`
- `Action no implementada para TableId` (handler default)

## Mapeo recomendado a componente de tabla (frontend)

- `columns` -> definicion de columnas del grid.
- `rows[].values` -> datasource de filas.
- `rowActions` / `toolbarActions` / `bulkActions` / `cellActions` -> render de botones/comandos.
- `pagination.total` -> total de registros para paginador.
- `sorting` -> estado de ordenamiento en UI.

## Ejemplo de consumo (TypeScript)

```ts
type ApiResponse<T> = {
  success: boolean;
  message: string;
  errors?: Array<{ field?: string; message?: string; type?: string }>;
  data: T;
};

export async function queryDynamicTable(token: string, body: Record<string, unknown>) {
  const res = await fetch("/api/ui/dynamic-table/query", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(body)
  });

  const payload = (await res.json()) as ApiResponse<unknown>;
  if (!payload.success) throw payload;
  return payload.data;
}
```

## Requisitos desde frontend

- Enviar token con `defaulalias`; sin ese claim el endpoint retorna `400`.
- Controlar `message` y `errors` para toasts/mensajes de validacion.
- Si el backend retorna `data=null` con `success=true`, mostrar estado vacio.
- Para sort, enviar solo columnas permitidas por configuracion fija o por `ui_table_columns`.
