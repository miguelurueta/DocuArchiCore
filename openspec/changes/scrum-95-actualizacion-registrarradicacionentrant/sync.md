# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-95`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-95`
- Summary: `ACTUALIZACION-RegistrarRadicacionEntranteAsync`
- Coordinator change: `openspec/changes/scrum-95-actualizacion-registrarradicacionentrant/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | no requiere cambio funcional para esta actualizacion | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/106 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | no requiere ajuste de contratos para esta actualizacion | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | reemplaza las condiciones workflow restantes por util_tipo_modulo_envio | pending | n/a | pending | in_progress |
| MiApp.Repository | no | no_code_change | el repositorio ya existe y no requiere cambio adicional | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | el modelo ya existe y no requiere cambio adicional | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-95` only in rows with `Impacta? = yes`.
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
