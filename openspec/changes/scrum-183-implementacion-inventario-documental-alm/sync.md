# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-183`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-183`
- Summary: `IMPLEMENTACION-INVENTARIO-DOCUMENTAL-ALMACENAMIENTO`
- Coordinator change: `openspec/changes/scrum-183-implementacion-inventario-documental-alm/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registro DI de builder inventario | done | pending | pending | in_progress |
| DocuArchiCore | yes | implementation_required | pruebas + documentacion + openspec | done | https://github.com/miguelurueta/DocuArchiCore/pull/244 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | ampliacion de contratos inventario/expediente | done | pending | pending | in_progress |
| MiApp.Services | yes | implementation_required | builder inventario + integracion coordinator | done | pending | pending | in_progress |
| MiApp.Repository | yes | implementation_required | insercion legacy completa en registro_producion_documental | done | pending | pending | in_progress |
| MiApp.Models | yes | implementation_required | modelos de insercion/build de inventario | done | pending | pending | in_progress |

## Operating Rule

Run `opsxj:new SCRUM-183` only in rows with `Impacta? = yes`.
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
