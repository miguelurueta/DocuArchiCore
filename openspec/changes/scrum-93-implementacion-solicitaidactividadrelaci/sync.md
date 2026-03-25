# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-93`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-93`
- Summary: `IMPLEMENTACION-SolicitaIdActividadRelacionadaGrupo`
- Coordinator change: `openspec/changes/scrum-93-implementacion-solicitaidactividadrelaci/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | el contrato y DI requeridos ya existen en main | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/101 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | el DTO no requiere cambio adicional para esta validacion | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | el servicio valida actividad workflow relacionada al grupo antes del registro | pending | n/a | pending | in_progress |
| MiApp.Repository | no | no_code_change | SolicitaIdActividadRelacionadaGrupo ya existe en main y solo se consume | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | GruposWorkflow ya expone id_Actividad en main | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-93` only in rows with `Impacta? = yes`.
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
