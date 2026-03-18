# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-77`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-77`
- Summary: `Desacoplar DocuArchi.Api de DocuArchiCore para corregir publish y dejar DocuArchiCore como repositorio coordinador`
- Coordinator change: `openspec/changes/scrum-77-desacoplar-docuarchi-api-de-docuarchicor/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |
| $repo | $impact | $motivo | $opsNew | $pr | $opsArchive | $status |

## Operating Rule

Run `opsxj:new SCRUM-77` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-77` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
