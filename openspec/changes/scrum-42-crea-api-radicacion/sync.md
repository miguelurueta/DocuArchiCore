# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-42`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-42`
- Summary: `CREA-API-RADICACION`
- Coordinator change: `openspec/changes/scrum-42-crea-api-radicacion/`

## Impact Matrix

| Repo | Impacta? | Motivo | opsxj:new | PR | opsxj:archive | Estado |
|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | Endpoint/controller de radicacion + integracion API | blocked (local changes) | pending | pending | in-progress |
| DocuArchiCore | yes | Repo orquestador de Jira/Git/OpenSpec para SCRUM-42 | done | existing | pending | in-progress |
| DocuArchiCore.Abstractions | no | Fuera de alcance del ticket actual | n/a | n/a | n/a | n/a |
| DocuArchiCore.Web | yes | No tiene `Tools/jira-open`; aplicar por flujo orquestado | blocked (no tooling) | pending | pending | in-progress |
| MiApp.DTOs | yes | Contratos DTO de request/response de radicacion | blocked (orquestado en DocuArchiCore) | pending | pending | in-progress |
| MiApp.Services | yes | Servicios de validacion/registro/flujo | blocked (orquestado en DocuArchiCore) | pending | pending | in-progress |
| MiApp.Repository | yes | Repositorios Q01-Q09 y transaccion | blocked (orquestado en DocuArchiCore) | pending | pending | in-progress |
| MiApp.Models | yes | Modelos persistidos relacionados con radicacion | blocked (orquestado en DocuArchiCore) | pending | pending | in-progress |

## Operational Decision (SCRUM-42)

1. `DocuArchiCore` se define como repositorio orquestador para `opsxj:new` y conexion Jira/Git.
2. Los repos impactados se atienden por implementacion coordinada desde el cambio orquestador, preservando sus working trees locales.
3. No ejecutar limpieza destructiva en repos hijos con trabajo activo; usar stash temporal solo cuando sea estrictamente necesario y restaurar inmediatamente.

## Operating Rule

Run `opsxj:new SCRUM-42` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-42` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
