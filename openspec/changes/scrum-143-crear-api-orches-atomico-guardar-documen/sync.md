# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-143`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-143`
- Summary: `CREAR-API-ORCHES-ATOMICO-GUARDAR-DOCUMENTO-EDITOR`
- Coordinator change: `openspec/changes/scrum-143-crear-api-orches-atomico-guardar-documen/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | New Controller and DI registration | pending | n/a | pending | tracked |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/189 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | New Request DTO | pending | n/a | pending | tracked |
| MiApp.Services | yes | implementation_required | New Service and Interface | pending | n/a | pending | tracked |
| MiApp.Repository | yes | implementation_required | Transaction support updates | pending | n/a | pending | tracked |
| MiApp.Models | no | no_code_change | reuse existing models | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-143` only in rows with `Impacta? = yes`.
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