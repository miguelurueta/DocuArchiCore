# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-109`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-109`
- Summary: `CREA-BUILDER-CAMPOS-BANDEJA-GESTION-CORRESPONDENCIA`
- Coordinator change: `openspec/changes/scrum-109-crea-builder-campos-bandeja-gestion-corr/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registrar builder en DI de Program.cs para consumo backend | done | https://github.com/miguelurueta/DocuArchi.Api/pull/36 | pending | in_review |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/136 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | no_code_change | contrato `WorkflowDynamicColumnDefinitionDto` ya existe y cubre la entrada del builder | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | implementar `IWorkflowDynamicUiColumnBuilder` y `WorkflowDynamicUiColumnBuilder` | done | https://github.com/miguelurueta/MiApp.Services/pull/58 | pending | in_review |
| MiApp.Repository | no | no_code_change | el builder no consulta base de datos ni usa repositorios en esta fase | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | no se requiere nuevo modelo ni tabla para una transformacion en memoria | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-109` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `no_code_change` stay only as documented scope, without empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: yes when DI wiring changes
- `MiApp.Services`: yes when implementing service/builders
- `MiApp.Repository`: only if the flow reads or persists data
- `MiApp.DTOs`: only if a new contract is actually missing
- `MiApp.Models`: only if model or table structure changes
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
