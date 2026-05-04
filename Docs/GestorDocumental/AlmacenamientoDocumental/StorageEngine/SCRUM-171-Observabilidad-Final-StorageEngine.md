# SCRUM-171 Observabilidad Final StorageEngine

## Logs implementados en API
- `Storage API request received` con `usuarioId`.
- `Storage API security validation failed` para errores de claim/seguridad.
- `Storage API unexpected failure` para excepciones no controladas.

## Campos recomendados (siguiente iteración)
- `requestId`, `alias`, `usuarioId`, `idAlmacen`, `estadoFinal`, `duracionMs`.

## Métricas sugeridas
- `storage_engine_request_count`
- `storage_engine_success_count`
- `storage_engine_validation_error_count`
- `storage_engine_physical_error_count`
- `storage_engine_duration_ms`

## Troubleshooting rápido
- `feature_disabled`: validar `FeatureFlags:StorageEngineV2`.
- `validation`: revisar claims y payload mínimo.
- `error`: correlacionar con logs del UseCase/Orchestrator por `requestId`.

