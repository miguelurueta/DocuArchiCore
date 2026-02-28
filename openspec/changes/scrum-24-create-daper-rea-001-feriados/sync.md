# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-24`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-24`
- Summary: `CREATE-DAPER-rea_001_feriados`
- Coordinator change: `openspec/changes/scrum-24-create-daper-rea-001-feriados/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `yes/no` | `<endpoint/controller/DI>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |
| `DocuArchiCore` | `yes/no` | `<coordinacion openspec/ui>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |
| `DocuArchiCore.Abstractions` | `yes/no` | `<contracts/interfaces>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |
| `DocuArchiCore.Web` | `yes/no` | `<frontend/web>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |
| `MiApp.DTOs` | `yes/no` | `<request/response dto>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |
| `MiApp.Services` | `yes/no` | `<service rules/interfaces>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |
| `MiApp.Repository` | `yes/no` | `<query/dapper/mysql>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |
| `MiApp.Models` | `yes/no` | `<domain model>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/n_a` |

## Operating Rule

Run `opsxj:new SCRUM-24` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-24` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
