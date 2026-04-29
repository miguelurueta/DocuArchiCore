# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-160`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-160`
- Summary: `CREA-API-LISTA-FIRMAS-AUTORIZADAS`
- Coordinator change: `openspec/changes/scrum-160-crea-api-lista-firmas-autorizadas/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | nuevo controller y DI | done | por publicar | pending | in_progress |
| DocuArchiCore | yes | implementation_required | tests + docs + openspec coordinador | done | https://github.com/miguelurueta/DocuArchiCore/pull/207 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |
| MiApp.Services | yes | implementation_required | service de negocio firmas autorizadas | done | por publicar | pending | in_progress |
| MiApp.Repository | yes | implementation_required | repository QueryOptions firmas autorizadas | done | por publicar | pending | in_progress |
| MiApp.Models | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |

## Operating Rule

Run `opsxj:new SCRUM-160` only in rows with `Impacta? = yes`.
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
