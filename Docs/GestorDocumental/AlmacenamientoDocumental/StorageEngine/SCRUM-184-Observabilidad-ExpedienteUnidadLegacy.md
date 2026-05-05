# SCRUM-184 — Observabilidad Expediente/Unidad Legacy

## Objetivo
Tener trazabilidad del resultado de la fase expediente/unidad sin exponer datos sensibles.

## Logs obligatorios
- `requestId`
- `idExpediente`
- `idUnidadConservacion`
- `idClaseDocumento`
- `idTipoUnidadDocumental`
- `numeroFolios`
- `tipoUnidadConserva`
- `estadoExpedienteElectronico`
- `estado` (ejecutado/error)
- `duracionMs`

## Eventos recomendados
- `expediente_unidad_plan_built`
- `expediente_locked`
- `unidad_locked`
- `expediente_folios_updated`
- `unidad_folios_updated`
- `expediente_unidad_completed`
- `expediente_unidad_failed`

## No registrar
- Fulltext documental.
- Rutas físicas completas.
- Contenido documental.
- Datos sensibles personales.

## Métricas sugeridas
- contador de ejecuciones expediente/unidad.
- contador de errores por validación.
- latencia p95 de fase expediente/unidad.
- porcentaje de casos expediente vs unidad.
