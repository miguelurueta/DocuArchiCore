# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-115`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-115`
- Summary: `ACTUALIZACION-REFACTOR-API-SolicitaBandejaWorkflow`
- Coordinator change: `openspec/changes/scrum-115-actualizacion-refactor-api-solicitabande/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | refactorizar el contrato publico del controller hacia WorkflowInboxApiRequestDto | pending | pending | pending | todo |
| DocuArchiCore | yes | implementation_required | alinear specs, sync y pruebas del refactor del contrato publico | done | https://github.com/miguelurueta/DocuArchiCore/pull/144 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | crear DTO publico WorkflowInboxApiRequestDto y completar contratos internos de inbox | pending | pending | pending | todo |
| MiApp.Services | yes | implementation_required | construir request interno enriquecido desde el DTO publico y preservar flujo actual | pending | pending | pending | todo |
| MiApp.Repository | yes | no_code_change | el query final reutiliza contratos existentes sin cambios de repositorio | n/a | n/a | n/a | n_a |
| MiApp.Models | yes | no_code_change | sin cambios de modelo para el refactor del contrato publico | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-115` only in rows with `Impacta? = yes`.
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
