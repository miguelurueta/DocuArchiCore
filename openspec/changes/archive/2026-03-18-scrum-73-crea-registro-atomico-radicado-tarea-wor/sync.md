# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-73`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-73`
- Summary: `CREA-REGISTRO-ATOMICO-RADICADO-TAREA-WORKFLOW`
- Coordinator change: `openspec/changes/scrum-73-crea-registro-atomico-radicado-tarea-wor/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `MiApp.DTOs` | `yes` | Se agrega `RegistroTareaWorkflowResultDto` para retornar el `idTareaWorkflow` generado. | `done` | `https://github.com/miguelurueta/MiApp.DTOs/pull/11` | `n/a` | `merged` |
| `MiApp.Models` | `yes` | Se agregan modelos workflow para `INICIO_TAREAS_WORKFLOW` y `ESTADOS_TAREA_WORKFLOW`. | `done` | `https://github.com/miguelurueta/MiApp.Models/pull/6` | `n/a` | `merged` |
| `MiApp.Repository` | `yes` | Se crea `RegistroRadicadoTareaWorkflowRepository` con transaccion atomica y validacion de ruta. | `done` | `https://github.com/miguelurueta/MiApp.Repository/pull/16` | `n/a` | `merged` |
| `DocuArchi.Api` | `yes` | Solo registra el repository en DI. No expone endpoint nuevo. | `done` | `https://github.com/miguelurueta/DocuArchi.Api/pull/25` | `n/a` | `merged` |
| `MiApp.Services` | `no` | El ticket prohíbe crear service adicional para esta función. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore` | `yes` | Coordinador: pruebas, docs, spec, tasks y validacion OpenSpec. | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/63` | `pending` | `ready_to_archive` |
| `DocuArchiCore.Web` | `no` | No hay frontend ni API publica en alcance. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore.Abstractions` | `no` | No se requiere contrato compartido adicional. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |

## Operating Rule

Run `opsxj:new SCRUM-73` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-73` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
