# SCRUM-171 Integración API Frontend

## Endpoint oficial
- Método: `POST`
- Ruta: `/api/gestor-documental/almacenamiento`
- Request: `AlmacenarDocumentoRequest`
- Response: `AppResponses<AlmacenarDocumentoResponse>`

## Claims requeridos
- `defaulalias`
- `usuarioid`

## Feature flag
- `FeatureFlags:StorageEngineV2 = true`: ejecuta Storage Engine.
- `FeatureFlags:StorageEngineV2 = false`: retorna `BadRequest` con `meta.status = "feature_disabled"`.

## Estados funcionales esperados
- `Completed`: flujo completo OK.
- `PhysicalFailed`: DB confirmada, fallo físico post-commit.
- `Failed`: error general de flujo.
- `validation`: error de claims o datos de entrada.

## Recomendaciones Frontend
- Tratar `feature_disabled` como estado funcional, no como caída técnica.
- No asumir rollback DB en todo error: revisar `meta.status` y mensaje.

