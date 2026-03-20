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
        "dataIndex": "consecutivo_radicado",
        "headerName": "Radicado",
        "title": "Radicado",
        "dataType": "text",
        "renderType": "grid_text",
        "visible": true,
        "sortable": true,
        "order": 1,
        "width": 160,
        "align": "left",
        "isActionColumn": false,
        "filterable": true,
        "filterType": "text",
        "filterOptions": null
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

## Metadata UI agnostica

`DynamicUiTableDto` no devuelve componentes MUI ni AntD; devuelve metadata para que cada frontend la adapte.

- `UiColumnDto.key`:
  - MUI: `field`
  - AntD: `key`
- `UiColumnDto.columnName`:
  - backend/source field
  - AntD: normalmente `dataIndex`
- `UiColumnDto.headerName`:
  - MUI: `headerName`
  - AntD: `title`
- `UiColumnDto.renderType`:
  - convención semántica
  - ejemplos actuales: `grid_text`, `grid_datetime`, `grid_chip`, `custom`
- `UiColumnDto.dataIndex`:
  - alias explícito para `Ant Design columns[].dataIndex`
- `UiColumnDto.title`:
  - alias explícito para `Ant Design columns[].title`
- `UiColumnDto.filterable` / `filterType`:
  - metadata para filtros AntD
  - ejemplos: `text`, `select`, `date`, `datetime`, `none`
- `UiActionDto.placement`:
  - `toolbar`, `bulk`, `row`
- `UiActionDto.presentation`:
  - `button`, `menu_item`, `icon`
- `UiActionDto.behavior`:
  - `api_call`, `navigate`, `modal`, `download`, `emit`, `custom`

Los valores actuales permanecen compatibles con MUI y deben interpretarse como aliases soportados, no como acoplamiento rígido a DataGrid.

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

## Ejemplo React + Ant Design Table

```tsx
import { Button, Dropdown, Space, Table, type TableColumnsType } from "antd";
import { useEffect, useState } from "react";

type DynamicColumn = {
  key: string;
  columnName: string;
  dataIndex?: string;
  headerName: string;
  title?: string;
  visible: boolean;
  width?: number;
  align?: "left" | "center" | "right";
  isActionColumn?: boolean;
  filterable?: boolean;
  filterType?: "text" | "select" | "date" | "datetime" | "none";
  filterOptions?: Array<{ text: string; value: string }>;
};

type DynamicAction = {
  actionId: string;
  label: string;
  placement: "toolbar" | "bulk" | "row";
  presentation: "button" | "menu_item" | "icon";
};

export function RadicadosPendientesAntTable({ token }: { token: string }) {
  const [rows, setRows] = useState<any[]>([]);
  const [columns, setColumns] = useState<TableColumnsType<any>>([]);
  const [toolbarActions, setToolbarActions] = useState<DynamicAction[]>([]);
  const [pagination, setPagination] = useState({ current: 1, pageSize: 25, total: 0 });

  useEffect(() => {
    queryDynamicTable(token, {
      tableId: "radicadosPendientes",
      page: 1,
      pageSize: 25,
      sortField: "fecha_registro",
      sortDir: "asc",
      includeConfig: true
    }).then((data: any) => {
      const nextRows = (data.rows || []).map((r: any) => ({ key: r.id, id: r.id, ...r.values }));
      const nextColumns = (data.columns || [])
        .filter((c: DynamicColumn) => c.visible)
        .map((c: DynamicColumn) => {
          if (c.isActionColumn) {
            return {
              key: c.key,
              title: c.headerName,
              width: c.width,
              align: c.align,
              render: (_: unknown, record: any) => {
                const rowActions = (data.rowActions || []).filter((a: DynamicAction) => a.placement === "row");
                const menuItems = rowActions.map((action: DynamicAction) => ({
                  key: action.actionId,
                  label: action.label
                }));

                if (menuItems.length === 0) return null;
                return <Dropdown menu={{ items: menuItems }} trigger={["click"]}><Button>Opciones</Button></Dropdown>;
              }
            };
          }

          return {
            key: c.key,
            dataIndex: c.dataIndex || c.columnName || c.key,
            title: c.title || c.headerName,
            width: c.width,
            align: c.align,
            filters: c.filterOptions?.map((option) => ({ text: option.text, value: option.value })),
            filterMode: c.filterType === "select" ? "menu" : undefined
          };
        });

      setRows(nextRows);
      setColumns(nextColumns);
      setToolbarActions(data.toolbarActions || []);
      setPagination({
        current: data.pagination?.page ?? 1,
        pageSize: data.pagination?.pageSize ?? 25,
        total: data.pagination?.total ?? nextRows.length
      });
    });
  }, [token]);

  return (
    <Space direction="vertical" style={{ width: "100%" }}>
      <Space>
        {toolbarActions.map((action) => (
          <Button key={action.actionId}>{action.label}</Button>
        ))}
      </Space>
      <Table rowKey="id" dataSource={rows} columns={columns} pagination={pagination} />
    </Space>
  );
}
```

## Mapping Ant Design

- `data.rows[].values` -> `dataSource`
- `data.rows[].id` -> `rowKey`
- `column.key` -> `columns[].key`
- `column.dataIndex` -> `columns[].dataIndex`
- `column.title` -> `columns[].title`
- `column.width` -> `columns[].width`
- `column.align` -> `columns[].align`
- `column.isActionColumn=true` -> columna con `render`
- `column.filterable/filterType/filterOptions` -> filtros locales o remotos en AntD
- `toolbarActions` -> botones fuera del `Table`
- `rowActions` -> `Dropdown`, `Space`, `Button` o `Menu`
- `bulkActions` -> toolbar contextual cuando haya selección
- `cellActions` -> `render` condicionado por `columnKey`
- `pagination.page/pageSize/total` -> `Table.pagination`
- `sorting.sortField/sortDir` -> estado de sorter/control remoto

## Requisitos desde frontend

- Enviar token con `defaulalias`; sin ese claim el endpoint retorna `400`.
- Controlar `message` y `errors` para toasts/mensajes de validacion.
- Si el backend retorna `data=null` con `success=true`, mostrar estado vacio.
- Para sort, enviar solo columnas permitidas por configuracion fija o por `ui_table_columns`.
