# SCRUM-171 Deuda Técnica StorageEngine

## Deuda media
- Adapter legacy para `StorageEngineV2=false` no implementado.
- Cobertura E2E/concurrencia del endpoint final pendiente por estabilidad de suite de pruebas.
- Idempotencia persistente (`StorageIdempotencyModel`) sin repositorio dedicado.

## Deuda menor
- Métricas y tracing (OpenTelemetry) no instrumentados en controller final.
- Normalización de warnings de nulabilidad existentes en solución.

## Plan propuesto
- SCRUM siguiente: crear `IAlmacenarDocumentoLegacyAdapter` e implementación.
- SCRUM siguiente: estabilizar tests rotos no relacionados y habilitar E2E del endpoint.
- SCRUM siguiente: instrumentar métricas y spans por fase.

