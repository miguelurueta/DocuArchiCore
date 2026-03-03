# SCRUM-34 - Guia de Integracion Frontend (Actualizada en SCRUM-35)

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
  "tableId": "radicadosPendientes",
  "page": 1,
  "pageSize": 25,
  "sortField": "fecha_registro",
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
- `sortField` debe existir dentro de columnas permitidas por el handler; si no, retorna validacion.
- Claims de seguridad (`role`, `permission`, `permiso`) se derivan del token y se reflejan en `userClaims`.

## Request action

```json
{
  "tableId": "radicadosPendientes",
  "actionId": "openDetail",
  "rowId": "101",
  "columnKey": "opciones",
  "selectedRowIds": ["101", "102"],
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
    "tableId": "radicadosPendientes",
    "title": "radicadosPendientes",
    "columns": [
      {
        "key": "consecutivo_radicado",
        "columnName": "consecutivo_radicado",
        "headerName": "Radicado",
        "dataType": "text",
        "renderType": "grid_text",
        "visible": true,
        "sortable": true,
        "order": 1,
        "width": 160,
        "align": "left",
        "isActionColumn": false
      }
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
        },
        "meta": null
      }
    ],
    "toolbarActions": [],
    "bulkActions": [],
    "rowActions": [],
    "cellActions": [],
    "userClaims": ["radicacion.consultar", "radicacion.gestionar"],
    "pagination": {
      "page": 1,
      "pageSize": 25,
      "total": 1
    },
    "sorting": {
      "sortField": "fecha_registro",
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
    "tableId": "radicadosPendientes",
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
- Campos base de ejemplo para pendientes: `id_estado_radicado`, `consecutivo_radicado`, `remitente`, `fecha_registro`, `estado`.

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

## Ejemplo React + MUI DataGrid

```tsx
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { useEffect, useState } from "react";

export function RadicadosPendientesGrid({ token }: { token: string }) {
  const [rows, setRows] = useState<any[]>([]);
  const [columns, setColumns] = useState<GridColDef[]>([]);

  useEffect(() => {
    queryDynamicTable(token, {
      tableId: "radicadosPendientes",
      page: 1,
      pageSize: 25,
      sortField: "fecha_registro",
      sortDir: "asc",
      includeConfig: true
    }).then((data: any) => {
      setRows((data.rows || []).map((r: any) => ({ id: r.id, ...r.values })));
      setColumns(
        (data.columns || [])
          .filter((c: any) => c.visible)
          .map((c: any) => ({ field: c.key, headerName: c.headerName, flex: 1 }))
      );
    });
  }, [token]);

  return <DataGrid rows={rows} columns={columns} autoHeight disableRowSelectionOnClick />;
}
```

## Requisitos desde frontend

- Enviar token con `defaulalias`; sin ese claim el endpoint retorna `400`.
- Controlar `message` y `errors` para toasts/mensajes de validacion.
- Si el backend retorna `data=null` con `success=true`, mostrar estado vacio.
- Para sort, enviar solo columnas permitidas por configuracion fija o por `ui_table_columns`.
