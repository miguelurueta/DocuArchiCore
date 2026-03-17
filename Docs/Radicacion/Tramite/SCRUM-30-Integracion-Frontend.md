# SCRUM-30 - Guia de Integracion Frontend

## Endpoint

- Metodo: `GET`
- URL: `/api/tramite/tramites/apListaRadicadosPendientes`
- Query params: ninguno.
- El backend toma `defaulalias` y `usuarioid` desde claims del token.

## Contrato de respuesta actualizado por SCRUM-71

```json
{
  "success": true,
  "message": "OK",
  "errors": [],
  "data": {
    "tableId": "lista-radicados-pendientes",
    "title": "lista-radicados-pendientes",
    "columns": [
      {
        "key": "id_estado_radicado",
        "columnName": "id_estado_radicado",
        "headerName": "IdEstadoRadicado",
        "dataType": "number",
        "renderType": "hidden",
        "visible": false
      },
      {
        "key": "consecutivo_radicado",
        "columnName": "consecutivo_radicado",
        "headerName": "Numero Radicado",
        "dataType": "text",
        "renderType": "grid_text",
        "visible": true
      },
      {
        "key": "remitente",
        "columnName": "remitente",
        "headerName": "Remitente",
        "dataType": "text",
        "renderType": "grid_text",
        "visible": true
      },
      {
        "key": "fecha_registro",
        "columnName": "fecha_registro",
        "headerName": "Fecha",
        "dataType": "date",
        "renderType": "grid_date",
        "visible": true
      },
      {
        "key": "actions",
        "columnName": "actions",
        "headerName": "Opciones",
        "dataType": "custom",
        "renderType": "custom",
        "visible": true
      }
    ],
    "rows": [
      {
        "id": "1",
        "values": {
          "id_estado_radicado": 1,
          "consecutivo_radicado": "RAD-2026-0001",
          "remitente": "Juan Perez",
          "fecha_registro": "2026-03-02"
        }
      }
    ],
    "cellActions": [
      {
        "columnKey": "actions",
        "action": {
          "actionId": "asignacion-tarea",
          "label": "asignacion de tarea",
          "presentation": "button",
          "behavior": "client_event",
          "request": {
            "rowIdField": "id_estado_radicado",
            "payloadFields": {
              "id_estado_radicado": "id_estado_radicado"
            }
          }
        }
      }
    ]
  }
}
```

## DTO de salida para tabla MUI

`DynamicUiTableDto`

- `columns`: definicion visual y funcional de columnas.
- `rows`: valores de cada radicado pendiente.
- `cellActions`: acciones ligadas a la columna `actions`.
- `request.rowIdField`: campo del row que el frontend debe usar como identificador funcional.
- `request.payloadFields`: mapeo de campos que deben viajar al disparar la accion.

## Estados esperados

- `success=true` + `message="OK"`: hay tabla dinamica con radicados pendientes.
- `success=true` + `message="Sin resultados"` + `data=null`: no hay pendientes para filtros.
- `success=false` + `message="El usuario radicador relacionado al usuario de gestión es null o 0"`: `Relacion_Id_Usuario_Radicacion` invalido.
- `success=false`: error controlado; revisar `errors`.

## Requisitos desde frontend

- Enviar token con claims `defaulalias` y `usuarioid`.
- No enviar `defaultDbAlias` por query string para este endpoint.
- Para la accion `asignacion de tarea`, leer `cellActions[*].action.request`.
- El frontend debe extraer `id_estado_radicado` desde `rows[*].values`.
