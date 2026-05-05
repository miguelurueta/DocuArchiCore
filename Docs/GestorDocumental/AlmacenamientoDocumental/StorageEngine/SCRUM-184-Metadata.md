# SCRUM-184 — Metadata

- Ticket: `SCRUM-184`
- Cambio OpenSpec: `openspec/changes/scrum-184-implementacion-expediente-almacenamiento`
- Prompt base: `PROMPT 18 — Expediente / Unidad Legacy-Compatible`
- Estado Jira esperado del flujo: `En revisión` -> `Done` tras merge + archive

## Repos impactados (orquestación)
- `DocuArchiCore`: coordinación OpenSpec (implementation_required)
- `DocuArchi.Api`: traceability_only
- `MiApp.Services`: traceability_only en este repo coordinador (implementación funcional se publica desde su repo)
- `MiApp.Repository`: traceability_only en este repo coordinador
- `MiApp.Models`: traceability_only en este repo coordinador
- `MiApp.DTOs`: traceability_only

## Artefactos técnicos creados
- `SCRUM-184-Arquitectura-ExpedienteUnidadLegacy.md`
- `SCRUM-184-Implementacion-Detallada-ExpedienteUnidadLegacy.md`
- `SCRUM-184-Pruebas-ExpedienteUnidadLegacy.md`
- `SCRUM-184-Observabilidad-ExpedienteUnidadLegacy.md`
- `SCRUM-184-Regresion-Legacy-ExpedienteUnidad.md`
- `SCRUM-184-Metadata.md`

## Criterio de cierre
- Tasks OpenSpec sin pendientes.
- Review gate OpenSpec confirmado.
- PRs requeridos mergeados.
- `opsxj:orchestrate:archive` ejecutado sin errores.
