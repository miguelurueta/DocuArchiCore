# SCRUM-61 Integracion Frontend

## Endpoint
- `GET /api/radicacion/tramite/solicita-campos-relacion-ruta-plantilla?idPlantillaRadicado={id}&idRuta={id}`

## Headers
- Requiere claim `defaulalias` en el contexto de autenticacion.

## Response DTO
- `RelacionCamposRutaWorklflowDto`
  - `NombreCampoPlantilla`
  - `TipoCampoPlantilla`
  - `DimensionCampoPlantilla`
  - `DatoCampoPlantilla`
  - `NombreCampoRuta`
  - `TipoCampoRuta`
  - `DimensionCampoRuta`
