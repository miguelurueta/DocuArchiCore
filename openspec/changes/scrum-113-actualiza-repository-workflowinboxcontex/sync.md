# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-113`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-113`
- Summary: `ACTUALIZA-REPOSITORY-WorkflowInboxContextResolverService`
- Coordinator change: `openspec/changes/scrum-113-actualiza-repository-workflowinboxcontex/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | el DI del resolvedor ya existe desde SCRUM-112 y no cambia en esta actualización | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/142 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | los DTOs creados en SCRUM-112 siguen vigentes sin cambio de contrato para esta corrección | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | ajustar `WorkflowInboxContextResolverService` a firma simplificada y resolución multi-alias desde claims | done | https://github.com/miguelurueta/MiApp.Services/pull/61 | pending | in_review |
| MiApp.Repository | yes | traceability_only | se reutilizan los repositories existentes sin crear contratos nuevos ni cambiar su implementación | pending | n/a | pending | tracked |
| MiApp.Models | no | no_code_change | se reutilizan los modelos existentes sin cambio de entidad | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-113` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `traceability_only` stay in `sync.md` without opening empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: only if controller o DI cambian
- `MiApp.Services`: yes when alias orchestration or workflow backend logic changes
- `MiApp.Repository`: only if repository contracts or implementation must change
- `MiApp.DTOs`: only if service contracts change
- `MiApp.Models`: only if entity shape changes
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
