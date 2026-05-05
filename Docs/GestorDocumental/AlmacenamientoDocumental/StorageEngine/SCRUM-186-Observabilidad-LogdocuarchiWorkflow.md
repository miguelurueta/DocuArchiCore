# SCRUM-186 — Observabilidad Logdocuarchi Workflow

## Logs clave
- `logdocuarchi no aplica requestId estado`
- `workflow log insertado requestId idTareaWorkflow idAlmacen`
- `workflow log insertado idTran idTareaWorkflow idRutaWorkflow gabinete` (repository)

## Campos recomendados de trazabilidad
- `requestId`
- `idTran`
- `idTareaWorkflow`
- `idRutaWorkflow`
- `gabinete`
- estado (`READY`, `NO_WORKFLOW`)

## Datos que no se exponen en logs
- `CAMPOS` completo (fulltext)
- contenido documental
- payload de documentos
- rutas sensibles completas fuera del modelo de auditoría

