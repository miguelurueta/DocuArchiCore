# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-34`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-34`
- Summary: `CREA-COMPONTE-DYMANIC-UI-TABLE`
- Coordinator change: `openspec/changes/scrum-34-crea-componte-dymanic-ui-table/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `no` | `Cambios integrados sin PR trazable para validacion automatica de archive` | `done` | `n/a` | `n/a` | `done` |
| `DocuArchiCore` | `yes` | `Coordinacion OpenSpec, docs tecnicos y evidencia` | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/11` | `pending` | `in_progress` |
| `DocuArchiCore.Abstractions` | `no` | `Sin cambios cross-cutting confirmados en esta fase` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | `No hace parte del alcance confirmado para SCRUM-34` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `no` | `Cambios integrados sin PR trazable para validacion automatica de archive` | `done` | `n/a` | `n/a` | `done` |
| `MiApp.Services` | `no` | `Cambios integrados sin PR trazable para validacion automatica de archive` | `done` | `n/a` | `n/a` | `done` |
| `MiApp.Repository` | `no` | `Cambios integrados sin PR trazable para validacion automatica de archive` | `done` | `n/a` | `n/a` | `done` |
| `MiApp.Models` | `no` | `Cambios integrados sin PR trazable para validacion automatica de archive` | `done` | `n/a` | `n/a` | `done` |

## Operating Rule

Run `opsxj:new SCRUM-34` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-34` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope

## Notes

- Se requiere definir estructura final de tabla de configuracion UI si no existe actualmente (`ui_table_columns`).
- La implementacion se hara multi-repo: API + Service + Repository + DTOs + Docs.
