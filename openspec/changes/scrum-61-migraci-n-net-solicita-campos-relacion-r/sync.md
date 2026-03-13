# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-61`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-61`
- Summary: `MIGRACIÓN .NET solicita_campos_relacion_ruta_plantilla`
- Coordinator change: `openspec/changes/scrum-61-migraci-n-net-solicita-campos-relacion-r/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| `DocuArchi.Api` | `yes` | endpoint/DI de consulta migrada | `done` | `pushed` | `pending` | `in_review` |
| `DocuArchiCore` | `yes` | coordinacion openspec/docs/tests del cambio | `done` | `pushed` | `pending` | `in_review` |
| `DocuArchiCore.Abstractions` | `no` | solo consulta (sin cambios) | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | solo consulta (sin cambios) | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `yes` | DTO de salida de consulta migrada | `done` | `pushed` | `pending` | `in_review` |
| `MiApp.Services` | `yes` | service/mapping de consulta migrada | `done` | `pushed` | `pending` | `in_review` |
| `MiApp.Repository` | `yes` | repository de consulta migrada | `done` | `pushed` | `pending` | `in_review` |
| `MiApp.Models` | `yes` | model de consulta migrada | `done` | `pushed` | `pending` | `in_review` |

## Operating Rule

Run `opsxj:new SCRUM-61` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-61` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
