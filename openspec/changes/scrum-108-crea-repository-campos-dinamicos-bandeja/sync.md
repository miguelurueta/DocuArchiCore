# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-108`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-108`
- Summary: `CREA-REPOSITORY-CAMPOS-DINAMICOS-BANDEJA-GESTION-CORRESPO-WORKLOW`
- Coordinator change: `openspec/changes/scrum-108-crea-repository-campos-dinamicos-bandeja/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registrar DI del repository en Program.cs | pending | n/a | pending | local_changes_present |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/134 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | usa contratos creados en SCRUM-107 sin cambios nuevos | pending | n/a | pending | tracked |
| MiApp.Services | yes | no_code_change | sin service en esta fase | n/a | n/a | n/a | n_a |
| MiApp.Repository | yes | implementation_required | nuevo repository y validator para metadata dinamica | pending | n/a | pending | local_changes_present |
| MiApp.Models | yes | no_code_change | reutiliza `ConfiguracionListadoRuta` existente | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-108` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `traceability_only` stay in `sync.md` without opening empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: yes when DI wiring changes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
