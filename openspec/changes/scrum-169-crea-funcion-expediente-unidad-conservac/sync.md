# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-169`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-169`
- Summary: `CREA-FUNCION-EXPEDIENTE-UNIDAD-CONSERVACION-ALMACENAMINETO`
- Coordinator change: `openspec/changes/scrum-169-crea-funcion-expediente-unidad-conservac/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registrar DI de nuevos servicios/repositorios fase expediente/indice | done | https://github.com/miguelurueta/DocuArchi.Api/pull/83 | pending | in_review |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/226 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | DTOs de entrada ya cubren IdExpediente/IdUnidadConservacion; validar sin cambios funcionales | pending | n/a | pending | tracked |
| MiApp.Services | yes | implementation_required | calculator/builder indice + integracion en StorageTransactionCoordinator | done | https://github.com/miguelurueta/MiApp.Services/pull/113 | pending | in_review |
| MiApp.Repository | yes | implementation_required | lock/update expediente y unidad + insert indice con DapperCrudEngine | done | https://github.com/miguelurueta/MiApp.Repository/pull/59 | pending | in_review |
| MiApp.Models | yes | implementation_required | modelos de expediente/unidad/indice/calculo requeridos por la fase | done | https://github.com/miguelurueta/MiApp.Models/pull/28 | pending | in_review |

## Operating Rule

Run `opsxj:new SCRUM-169` only in rows with `Impacta? = yes`.
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
