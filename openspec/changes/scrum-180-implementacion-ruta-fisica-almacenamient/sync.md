# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-180`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-180`
- Summary: `IMPLEMENTACION-RUTA-FISICA-ALMACENAMIENTO`
- Coordinator change: `openspec/changes/scrum-180-implementacion-ruta-fisica-almacenamient/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registro DI para path service/policy/repository | done | pending | pending | in_progress |
| DocuArchiCore | yes | implementation_required | orquestador openspec central + pruebas + documentación | done | https://github.com/miguelurueta/DocuArchiCore/pull/238 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | sin cambios funcionales de DTO en SCRUM-180 | pending | n/a | pending | tracked |
| MiApp.Services | yes | implementation_required | implementación path service, policy, resolver y writers | done | pending | pending | in_progress |
| MiApp.Repository | yes | implementation_required | repositorio ruta legacy `system1rut` | done | pending | pending | in_progress |
| MiApp.Models | yes | implementation_required | nuevos modelos físicos de ruta legacy | done | pending | pending | in_progress |

## Operating Rule

Run `opsxj:new SCRUM-180` only in rows with `Impacta? = yes`.
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
