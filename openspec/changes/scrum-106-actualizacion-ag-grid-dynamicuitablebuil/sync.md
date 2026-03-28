# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-106`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-106`
- Summary: `ACTUALIZACION-AG-GRID-DynamicUiTableBuilder`
- Coordinator change: `openspec/changes/scrum-106-actualizacion-ag-grid-dynamicuitablebuil/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | no requiere cambios funcionales para exponer metadata AG Grid | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | coordinacion OpenSpec, pruebas y documentacion tecnica | done | https://github.com/miguelurueta/DocuArchiCore/pull/128 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | agrega aliases de contrato para AG Grid en DTOs de tabla dinamica | done | https://github.com/miguelurueta/MiApp.DTOs/pull/23 | pending | merged |
| MiApp.Services | yes | implementation_required | normaliza metadata y filtros compatibles con AG Grid en el builder | done | https://github.com/miguelurueta/MiApp.Services/pull/56 | pending | merged |
| MiApp.Repository | no | no_code_change | no requiere persistencia ni consulta adicional para este ajuste de contrato | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | no hay cambios de modelo de dominio para esta compatibilidad | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-106` only in rows with `Impacta? = yes`.
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
