# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-77`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-77`
- Summary: `Desacoplar DocuArchi.Api de DocuArchiCore para corregir publish y dejar DocuArchiCore como repositorio coordinador`
- Coordinator change: `openspec/changes/scrum-77-desacoplar-docuarchi-api-de-docuarchicor/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `yes` | Remove web-to-web `ProjectReference`, relocate `SesionActual*`, update DI, validate build/publish | `done` | `https://github.com/miguelurueta/DocuArchi.Api/pull/28` | `pending` | `in_review` |
| `DocuArchiCore` | `yes` | Coordinator artifacts plus current `SesionActual*` source used as migration input | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/70` | `pending` | `in_review` |
| `DocuArchiCore.Abstractions` | `no` | Contracts already exist and no interface migration is planned | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `no` | No DTO shape changes identified in current analysis | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Models` | `no` | No model changes identified in current analysis | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Repository` | `no` | Repository layer is not part of the publish collision root cause | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Services` | `no` | Service layer is not part of the publish collision root cause | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | Out of scope for this decoupling change | `n/a` | `n/a` | `n/a` | `n_a` |

## Operating Rule

Run `opsxj:new SCRUM-77` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-77` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: yes for this ticket, because the runtime dependency and publish flow must be corrected there.
- `DocuArchiCore`: yes as coordinator and current source location of `SesionActual*`.
- `DocuArchiCore.Abstractions`: no, unless hidden interface gaps appear during migration.
