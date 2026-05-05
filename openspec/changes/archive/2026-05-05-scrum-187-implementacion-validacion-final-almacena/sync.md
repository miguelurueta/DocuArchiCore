# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-187`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-187`
- Summary: `IMPLEMENTACION-VALIDACION-FINAL-ALMACENAMIENTO`
- Coordinator change: `openspec/changes/scrum-187-implementacion-validacion-final-almacena/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/DocuArchi.Api/pull/99 | done | archived |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/248 | done | archived |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.DTOs/pull/73 | done | archived |
| MiApp.Services | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Services/pull/129 | done | archived |
| MiApp.Repository | yes | traceability_only | sin diff real publicado | n/a | n/a | done | archived |
| MiApp.Models | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Models/pull/42 | done | archived |

## Operating Rule

Run `opsxj:new SCRUM-187` only in rows with `Impacta? = yes`.
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