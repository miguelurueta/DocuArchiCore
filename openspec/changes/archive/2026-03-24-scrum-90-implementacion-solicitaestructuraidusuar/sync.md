# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-90`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-90`
- Summary: `IMPLEMENTACION-SolicitaEstructuraIdUsuarioWorkflowId`
- Coordinator change: `openspec/changes/scrum-90-implementacion-solicitaestructuraidusuar/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | <definir alcance> | done | https://github.com/miguelurueta/DocuArchi.Api/pull/32 | done | archived |
| DocuArchiCore | yes | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/91 | done | archived |
| DocuArchiCore.Abstractions | no | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | <definir alcance> | done | https://github.com/miguelurueta/MiApp.DTOs/pull/19 | done | archived |
| MiApp.Services | yes | <definir alcance> | done | https://github.com/miguelurueta/MiApp.Services/pull/43 | done | archived |
| MiApp.Repository | yes | <definir alcance> | done | https://github.com/miguelurueta/MiApp.Repository/pull/22 | done | archived |
| MiApp.Models | yes | <definir alcance> | done | https://github.com/miguelurueta/MiApp.Models/pull/9 | done | archived |

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