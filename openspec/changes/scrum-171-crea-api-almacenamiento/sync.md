# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-171`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-171`
- Summary: `CREA-API-ALMACENAMIENTO`
- Coordinator change: `openspec/changes/scrum-171-crea-api-almacenamiento/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | creación de `AlmacenamientoDocumentalController`, feature flag `StorageEngineV2` y registro DI | done | pending | pending | in_progress |
| DocuArchiCore | yes | implementation_required | pruebas unitarias del controller + alineación OpenSpec + documentación técnica SCRUM-171 | done | https://github.com/miguelurueta/DocuArchiCore/pull/229 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | contratos de request/response ya existentes desde SCRUM-163 | done | n/a | pending | tracked |
| MiApp.Services | no | no_code_change | `IAlmacenarDocumentoUseCase` ya integrado desde cambios previos (SCRUM-164..170) | done | n/a | pending | tracked |
| MiApp.Repository | no | no_code_change | sin cambios requeridos para este cierre de API | done | n/a | pending | tracked |
| MiApp.Models | no | no_code_change | modelos/estados ya definidos en cambios previos del Storage Engine | done | n/a | pending | tracked |

## Operating Rule

Run `opsxj:new SCRUM-171` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `traceability_only` stay in `sync.md` without opening empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
