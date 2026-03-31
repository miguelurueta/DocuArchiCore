# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-114`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-114`
- Summary: `CREA-API-BANDEJA-WORKFLOW-GESTION-CORRESPONDENCIA`
- Coordinator change: `openspec/changes/scrum-114-crea-api-bandeja-workflow-gestion-corres/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | exponer endpoint y registrar DI del flujo de bandeja workflow | pending | pending | pending | todo |
| DocuArchiCore | yes | implementation_required | coordinar spec, sync y pruebas del flujo end-to-end | pending | pending | pending | todo |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | extender request/context DTOs requeridos por la bandeja workflow | pending | pending | pending | todo |
| MiApp.Services | yes | implementation_required | resolver contexto, construir query y servir la bandeja dinamica workflow | pending | pending | pending | todo |
| MiApp.Repository | yes | traceability_only | reutiliza contratos/repositorios existentes sin diff funcional nuevo | n/a | n/a | pending | tracked |
| MiApp.Models | yes | no_code_change | sin cambios de modelo para este flujo | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-114` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `traceability_only` stay in `sync.md` without opening empty PRs unless a later implementation decision changes the matrix.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
