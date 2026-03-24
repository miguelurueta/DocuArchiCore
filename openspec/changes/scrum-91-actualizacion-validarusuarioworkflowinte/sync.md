# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-91`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-91`
- Summary: `ACTUALIZACION-ValidarUsuarioWorkflowInternoAsync`
- Coordinator change: `openspec/changes/scrum-91-actualizacion-validarusuarioworkflowinte/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | <definir alcance> | pending | pending | pending | todo |
| DocuArchiCore | yes | implementation_required | <definir alcance> | pending | pending | pending | todo |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | <definir alcance> | pending | pending | pending | todo |
| MiApp.Services | yes | implementation_required | <definir alcance> | pending | pending | pending | todo |
| MiApp.Repository | yes | implementation_required | <definir alcance> | pending | pending | pending | todo |
| MiApp.Models | yes | implementation_required | <definir alcance> | pending | pending | pending | todo |

## Operating Rule

Run `opsxj:new SCRUM-91` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-91` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
