# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`ABC-123`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `ABC-123`
- Summary: `<short summary>`
- Coordinator change: `openspec/changes/<change-name>/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<endpoint/controller/DI>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |
| `DocuArchiCore` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<coordinacion openspec/ui>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |
| `DocuArchiCore.Abstractions` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<contracts/interfaces>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |
| `DocuArchiCore.Web` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<frontend/web>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |
| `MiApp.DTOs` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<request/response dto>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |
| `MiApp.Services` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<service rules/interfaces>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |
| `MiApp.Repository` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<query/dapper/mysql>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |
| `MiApp.Models` | `yes/no` | `implementation_required/traceability_only/no_code_change` | `<domain model>` | `pending/done/n/a` | `<url/n/a>` | `pending/done/n/a` | `todo/in_progress/review/merged/tracked/n_a` |

## Operating Rule

Run `opsxj:new ABC-123` only in rows with `Impacta? = yes`.
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
