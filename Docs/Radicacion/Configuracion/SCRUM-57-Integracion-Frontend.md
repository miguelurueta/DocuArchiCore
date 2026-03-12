# SCRUM-57 Integracion Frontend - Configuracion Plantilla Radicacion

## API
- Metodo: `GET`
- Ruta: `/api/configuracionPlantilla/solicitaConfiguracionPlantilla?idPlantilla={id}&tipoRadicacionPlantilla={tipo}`
- Auth: Bearer token con claim `defaulalias`

## Parametros
- `idPlantilla` (`int`, requerido): identificador de la plantilla de radicacion.
- `tipoRadicacionPlantilla` (`int`, requerido): tipo de radicacion para filtrar configuracion.
- `defaultDbAlias` (`string`, requerido): se obtiene desde claim `defaulalias`.

## Origen de parametros
- `idPlantilla`: frontend (flujo de radicacion actual).
- `tipoRadicacionPlantilla`: frontend (tipo de radicado seleccionado).
- `defaultDbAlias`: backend (JWT claim).

## DTO de respuesta
`AppResponses<RaRadConfigPlantillaRadicacionDto?>`

`RaRadConfigPlantillaRadicacionDto`:
- `id_rad_config_plantilla_radicacion` (`int`)
- `system_plantilla_radicado_id_Plantilla` (`int`)
- `Tipo_radicacion_plantilla` (`int`)
- `Descripcion_tipo_radicacion` (`string?`)
- `util_notificacion_remitente` (`int`)
- `util_notificacion_destinatario` (`int`)
- `util_valida_restriccion_radicacion` (`int`)

## Ejemplo request
```http
GET /api/configuracionPlantilla/solicitaConfiguracionPlantilla?idPlantilla=67&tipoRadicacionPlantilla=1
Authorization: Bearer <token>
```

## Ejemplo response OK
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "id_rad_config_plantilla_radicacion": 1,
    "system_plantilla_radicado_id_Plantilla": 67,
    "Tipo_radicacion_plantilla": 1,
    "Descripcion_tipo_radicacion": "Externa",
    "util_notificacion_remitente": 1,
    "util_notificacion_destinatario": 0,
    "util_valida_restriccion_radicacion": 1
  },
  "meta": null,
  "errors": []
}
```

## Ejemplo response sin resultados
```json
{
  "success": true,
  "message": "Sin resultados",
  "data": null,
  "meta": null,
  "errors": []
}
```

## Consideraciones frontend
- Cuando `data` sea `null`, tratar como ausencia de configuracion y continuar flujo por defecto.
- Manejar `400` cuando backend retorne validaciones (idPlantilla/tipo/alias).
- El endpoint no recibe body; solo query string + token.
