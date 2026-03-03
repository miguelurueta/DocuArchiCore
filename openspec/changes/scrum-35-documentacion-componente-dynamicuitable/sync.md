# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-35`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-35`
- Summary: `DOCUMENTACION-COMPONENTE-DynamicUiTable-SCRUM-34`
- Coordinator change: `openspec/changes/scrum-35-documentacion-componente-dynamicuitable/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `DocuArchiCore` | `yes` | `Documentacion tecnica y reubicacion de docs DynamicUiTable` | `done` | `pending` | `pending` | `in_progress` |
| `DocuArchi.Api` | `no` | `Sin cambios de runtime/API para SCRUM-35` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Abstractions` | `no` | `Sin cambios de contratos transversales` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | `No se requiere cambio de codigo web, solo guia documental` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `no` | `Sin cambios de DTO en este ticket` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Services` | `no` | `Componente ya implementado en SCRUM-34; se documenta en DocuArchiCore` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Repository` | `no` | `Sin cambios de repositorio para este ticket` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Models` | `no` | `Sin cambios de modelo para este ticket` | `n/a` | `n/a` | `n/a` | `n_a` |

## Operating Rule

Run `opsxj:new SCRUM-35` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-35` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchiCore`: yes (scope documental)
- Demas repos: no (sin cambios de codigo en SCRUM-35)
