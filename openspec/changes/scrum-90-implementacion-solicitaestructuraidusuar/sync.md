# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-90`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-90`
- Summary: `IMPLEMENTACION-SolicitaEstructuraIdUsuarioWorkflowId`
- Coordinator change: `openspec/changes/scrum-90-implementacion-solicitaestructuraidusuar/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | <definir alcance> | pending | pending | pending | todo |
| DocuArchiCore | yes | <definir alcance> | pending | pending | pending | todo |
| DocuArchiCore.Abstractions | no | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | <definir alcance> | pending | pending | pending | todo |
| MiApp.Services | yes | <definir alcance> | pending | pending | pending | todo |
| MiApp.Repository | yes | <definir alcance> | pending | pending | pending | todo |
| MiApp.Models | yes | <definir alcance> | pending | pending | pending | todo |

## Operating Rule

Run `opsxj:new SCRUM-90` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-90` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
