# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-110`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-110`
- Summary: `CREA-QUERYBUILDER-RESPOSITORY-BANDEJA-WORKFLOW`
- Coordinator change: `openspec/changes/scrum-110-crea-querybuilder-respository-bandeja-wo/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registrar `IWorkflowInboxQueryBuilder` en DI para que el componente quede disponible en runtime | done | https://github.com/miguelurueta/DocuArchi.Api/pull/37 | pending | in_review |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/139 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | crear contrato `WorkflowInboxDynamicTableRequestDto` faltante para el querybuilder | done | https://github.com/miguelurueta/MiApp.DTOs/pull/26 | pending | in_review |
| MiApp.Services | no | no_code_change | el componente construye QueryOptions dentro de repository, sin service nuevo | n/a | n/a | n/a | n_a |
| MiApp.Repository | yes | implementation_required | implementar `IWorkflowInboxQueryBuilder`, `WorkflowInboxQueryBuilder` y policy en `Workflow/BandejaCorrespondencia` | done | https://github.com/miguelurueta/MiApp.Repository/pull/30 | pending | in_review |
| MiApp.Models | no | no_code_change | no se requiere nuevo modelo ni tabla; solo composicion de QueryOptions | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-110` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `no_code_change` stay only as documented scope, without empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: only if DI or endpoint surface changes
- `MiApp.Services`: only if business orchestration changes
- `MiApp.Repository`: yes when QueryOptions/query builders are built here
- `MiApp.DTOs`: yes if the request/response contract is missing
- `MiApp.Models`: only if model or schema changes
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
