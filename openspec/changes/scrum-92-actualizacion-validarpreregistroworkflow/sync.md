# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-92`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-92`
- Summary: `ACTUALIZACION-ValidarPreRegistroWorkflowAsync`
- Coordinator change: `openspec/changes/scrum-92-actualizacion-validarpreregistroworkflow/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | sin cambios funcionales para SCRUM-92 | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/99 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | contrato ValidaPreRegistroWorkflowResultDto ahora incluye RutaWorkflow | done | https://github.com/miguelurueta/MiApp.DTOs/pull/21 | pending | in_review |
| MiApp.Services | yes | implementation_required | preregistro workflow y validacion de registro consumen RutaWorkflow y bloquean el flujo si falta | done | https://github.com/miguelurueta/MiApp.Services/pull/45 | pending | in_review |
| MiApp.Repository | no | no_code_change | sin cambios de repositorio requeridos en SCRUM-92 | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | sin cambios de modelos persistentes requeridos en SCRUM-92 | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-92` only in rows with `Impacta? = yes`.
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
