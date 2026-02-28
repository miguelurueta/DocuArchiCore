# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-27`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-27`
- Summary: `ACTUALIZA-DAPER-DIASVENCIMIENTO`
- Coordinator change: `openspec/changes/scrum-27-actualiza-daper-diasvencimiento/`

## Impact Matrix

| $(Repo) | $(no) | $(fuera de alcance) | $(n/a) | $(n/a) | $(n/a) | $(n_a) |
| $(---) | $(no) | $(fuera de alcance) | $(n/a) | $(n/a) | $(n/a) | $(n_a) |
| $(DocuArchi.Api) | $(no) | $(fuera de alcance) | $(n/a) | $(n/a) | $(n/a) | $(n_a) |
| $(DocuArchiCore) | $(no) | $(fuera de alcance) | $(n/a) | $(n/a) | $(n/a) | $(n_a) |
| $(DocuArchiCore.Abstractions) | $(no) | $(fuera de alcance) | $(n/a) | $(n/a) | $(n/a) | $(n_a) |
| $(DocuArchiCore.Web) | $(yes) | $(<definir alcance>) | $(pending) | $(pending) | $(pending) | $(todo) |
| $(MiApp.DTOs) | $(yes) | $(<definir alcance>) | $(pending) | $(pending) | $(pending) | $(todo) |
| $(MiApp.Services) | $(no) | $(fuera de alcance) | $(n/a) | $(n/a) | $(n/a) | $(n_a) |
| $(MiApp.Repository) | $(yes) | $(<definir alcance>) | $(pending) | $(pending) | $(pending) | $(todo) |
| $(MiApp.Models) | $(no) | $(fuera de alcance) | $(n/a) | $(n/a) | $(n/a) | $(n_a) |

## Operating Rule

Run `opsxj:new SCRUM-27` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-27` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
