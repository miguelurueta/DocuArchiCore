# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-116`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-116`
- Summary: `ACTUALIZACION-NORMALIZACION-SALIDA-WorkflowInboxRepository`
- Coordinator change: `openspec/changes/scrum-116-actualizacion-normalizacion-salida-workf/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | no_code_change | sin cambios de controller ni contrato publico del API | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | alinear spec, sync y pruebas del flujo workflow inbox normalizado | done | https://github.com/miguelurueta/DocuArchiCore/pull/145 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | no_code_change | sin cambios de contratos DTO para la normalizacion de salida | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | normalizar filas en WorkflowInboxRepository y consumir diccionarios tipados en service | done | https://github.com/miguelurueta/MiApp.Services/pull/64 | pending | in_review |
| MiApp.Repository | yes | no_code_change | DapperCrudEngine se reutiliza sin cambios funcionales | n/a | n/a | n/a | n_a |
| MiApp.Models | yes | no_code_change | sin cambios de modelo para la normalizacion de salida | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-116` only in rows with `Impacta? = yes`.
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
