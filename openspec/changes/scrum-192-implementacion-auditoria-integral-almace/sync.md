# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-192`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-192`
- Summary: `IMPLEMENTACION-AUDITORIA-INTEGRAL-ALMACENAMIENTO`
- Coordinator change: `openspec/changes/scrum-192-implementacion-auditoria-integral-almace/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | ticket de auditoria/documentacion sin cambios en API | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/256 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | ticket de auditoria/documentacion sin cambios DTO | n/a | n/a | n/a | n_a |
| MiApp.Services | no | no_code_change | ticket de auditoria/documentacion sin cambios de servicio | n/a | n/a | n/a | n_a |
| MiApp.Repository | no | no_code_change | ticket de auditoria/documentacion sin cambios de repositorio | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | ticket de auditoria/documentacion sin cambios de modelos | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-192` only in rows with `Impacta? = yes`.
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
