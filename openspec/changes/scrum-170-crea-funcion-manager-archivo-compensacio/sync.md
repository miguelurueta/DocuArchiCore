# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-170`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-170`
- Summary: `CREA-FUNCION-MANAGER-ARCHIVO-COMPENSACION`
- Coordinator change: `openspec/changes/scrum-170-crea-funcion-manager-archivo-compensacio/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/DocuArchi.Api/pull/85 | pending | in_review |
| DocuArchiCore | yes | implementation_required | orquestador OpenSpec central, pruebas y documentación técnica | done | https://github.com/miguelurueta/DocuArchiCore/pull/228 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | la fase física no requiere nuevos DTOs | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Services/pull/115 | pending | in_review |
| MiApp.Repository | no | no_code_change | esta fase no accede a DB ni agrega repositories de datos | n/a | n/a | n/a | n_a |
| MiApp.Models | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Models/pull/30 | pending | in_review |

## Operating Rule

Run `opsxj:new SCRUM-170` only in rows with `Impacta? = yes`.
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