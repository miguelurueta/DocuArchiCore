# SCRUM-30 - Guia de Integracion Frontend

## Endpoint

- Metodo: `GET`
- URL: `/api/tramite/tramites/apListaRadicadosPendientes`
- Query params: ninguno.
- El backend toma `defaulalias` y `usuarioid` desde claims del token.

## Contrato de respuesta

```json
{
  "success": true,
  "message": "OK",
  "errorMessage": "",
  "errors": [],
  "data": [
    {
      "id_estado_radicado": 1,
      "consecutivo_radicado": "RAD-2026-0001",
      "remitente": "Juan Perez",
      "fecha_registro": "2026-03-02T09:10:00",
      "opciones": null
    }
  ]
}
```

## DTO de salida para tabla MUI

`ListaRadicadosPendientesDto`

- `id_estado_radicado`: estado interno del radicado (oculto en UI).
- `consecutivo_radicado`: identificador visible en columna `Radicado`.
- `remitente`: nombre visible en columna `Remitente`.
- `fecha_registro`: fecha visible en columna `Fecha`.
- `opciones`: columna para acciones/comandos desde frontend.

## Estados esperados

- `success=true` + `message="OK"`: hay lista de radicados pendientes.
- `success=true` + `message="Sin resultados"` + `data=null`: no hay pendientes para filtros.
- `success=false` + `message="El usuario radicador relacionado al usuario de gestión es null o 0"`: `Relacion_Id_Usuario_Radicacion` invalido.
- `success=false`: error controlado; revisar `errorMessage` y/o `errors`.

## Requisitos desde frontend

- Enviar token con claims `defaulalias` y `usuarioid`.
- No enviar `defaultDbAlias` por query string para este endpoint.
