# SCRUM-171 Runbook Producción StorageEngine

## Habilitar/deshabilitar StorageEngineV2
- Archivo: `DocuArchi.Api/appsettings*.json`
- Ruta config: `FeatureFlags:StorageEngineV2`
- `true`: nuevo engine activo.
- `false`: endpoint devuelve estado funcional `feature_disabled`.

## Validación operativa
- Verificar salud del endpoint con request mínimo válido y claims.
- Revisar logs API y Services por `requestId`.

## Diagnóstico de incidentes
- `400 validation`: revisar claims `defaulalias/usuarioid` y campos obligatorios.
- `400 feature_disabled`: validar despliegue de configuración.
- `500 error`: revisar logs de UseCase/Orchestrator/phase física.

## Checklist de despliegue
- Confirmar `FeatureFlags:StorageEngineV2` según ambiente.
- Confirmar DI contiene `IFeatureToggleService`.
- Confirmar build de `DocuArchi.Api` exitoso.

