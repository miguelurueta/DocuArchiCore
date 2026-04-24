# SCRUM-156 — Observabilidad

## Logs agregados

- Controller: request con `idPlantillaRadicado`, `idDestinatarioExterno`, `alias`, `requestId`, `xRequestId`.
- Repository: trazabilidad técnica (db, script, plantillaValidacion, table/columns) + tiempo (ms).

## Troubleshooting (empty)

- No existe script activo para la plantilla
- No hay relación a plantilla de validación
- No existe campo marcado como correo
- Tabla/columna no existe en el esquema (`INFORMATION_SCHEMA`)
- No existe registro para `idDestinatarioExterno`

