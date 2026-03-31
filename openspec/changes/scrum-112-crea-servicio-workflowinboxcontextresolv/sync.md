# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-112`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-112`
- Summary: `CREA-SERVICIO-WorkflowInboxContextResolverService`
- Coordinator change: `openspec/changes/scrum-112-crea-servicio-workflowinboxcontextresolv/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registrar `IWorkflowInboxContextResolverService` en DI del API usando un worktree limpio del ticket | pending | n/a | pending | pending |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/141 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | agregar `WorkflowInboxResolvedContextDto` y extender request base con `IdUsuarioGestion` para resolver contexto sin depender del frontend | pending | n/a | pending | pending |
| MiApp.Services | yes | implementation_required | crear `WorkflowInboxContextResolverService` consumiendo repositorios existentes y retornando `AppResponses<WorkflowInboxResolvedContextDto>` | pending | n/a | pending | pending |
| MiApp.Repository | yes | traceability_only | los repositorios requeridos ya existen (`RemitDestInternoR`, `UsuarioWorkflowR`, `SolicitaEstructuraRutaWorkflowRepository`, `GruposWorkflowR`) y se consumen sin crear nuevos contratos | pending | n/a | pending | tracked |
| MiApp.Models | no | no_code_change | se reutilizan `RemitDestInterno`, `UsuarioWorkflow`, `RutasWorkflow` y `GruposWorkflow` sin cambios de modelo | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-112` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `traceability_only` stay in `sync.md` without opening empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: yes when DI or endpoint integration is needed
- `MiApp.Services`: yes when backend orchestration moves to service layer
- `MiApp.Repository`: only if a new repository or repository contract is required
- `MiApp.DTOs`: yes when service contracts change
- `MiApp.Models`: only if entity shape changes
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
