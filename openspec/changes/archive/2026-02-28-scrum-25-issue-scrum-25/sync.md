# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-25`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-25`
- Summary: `Issue SCRUM-25`
- Coordinator change: `openspec/changes/scrum-25-issue-scrum-25/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `no` | `Fuera de alcance hasta confirmar requerimiento de API` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore` | `yes` | `Coordinacion OpenSpec del ticket SCRUM-25` | `done` | `pending` | `pending` | `todo` |
| `DocuArchiCore.Abstractions` | `no` | `Sin cambios de contratos confirmados` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | `Sin alcance frontend confirmado` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `no` | `Sin requerimientos de DTO definidos` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Services` | `no` | `Sin logica de servicio confirmada` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Repository` | `no` | `Sin cambios de repositorio confirmados` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Models` | `no` | `Sin cambios de modelo confirmados` | `n/a` | `n/a` | `n/a` | `n_a` |

## Operating Rule

Run `opsxj:new SCRUM-25` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-25` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
