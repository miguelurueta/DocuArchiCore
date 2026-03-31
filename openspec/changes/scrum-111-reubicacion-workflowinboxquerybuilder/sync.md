# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-111`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-111`
- Summary: `REUBICACION-WorkflowInboxQueryBuilder`
- Coordinator change: `openspec/changes/scrum-111-reubicacion-workflowinboxquerybuilder/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | actualizar DI y using para consumir `IWorkflowInboxQueryBuilder` desde `MiApp.Services.Service.Workflow.BandejaCorrespondencia` | done | https://github.com/miguelurueta/DocuArchi.Api/pull/38 | pending | in_review |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/140 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | el contrato `WorkflowInboxDynamicTableRequestDto` no cambia en esta reubicacion | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | reubicar `WorkflowInboxQueryBuilder` e `IWorkflowInboxQueryBuilder` a `Service/Workflow/BandejaCorrespondencia` | done | https://github.com/miguelurueta/MiApp.Services/pull/59 | pending | in_review |
| MiApp.Repository | yes | implementation_required | retirar el builder de `Repositorio/Workflow/BandejaCorrespondencia` y mantener solo `QueryOptions`/repositorio ejecutor | done | https://github.com/miguelurueta/MiApp.Repository/pull/31 | pending | in_review |
| MiApp.Models | no | no_code_change | no hay cambios de modelo asociados al movimiento de capa | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-111` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `no_code_change` stay in `sync.md` without opening empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: yes when DI or namespace references move
- `MiApp.Services`: yes when builders or orchestration components move to service layer
- `MiApp.Repository`: yes when a repository-owned component is removed or replaced
- `MiApp.DTOs`: only if contracts change
- `MiApp.Models`: only if model changes
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
