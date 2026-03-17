# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-71`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-71`
- Summary: `ACTUALIZA-API-LISTA-RADICADOS-PENDIENTES`
- Coordinator change: `openspec/changes/scrum-71-actualiza-api-lista-radicados-pendientes/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `MiApp.DTOs` | `yes` | Se extiende contrato `DynamicUiActionRequestDto` para acciones de tabla dinamica. | `done` | `https://github.com/miguelurueta/MiApp.DTOs/pull/10` | `n/a` | `merged` |
| `MiApp.Services` | `yes` | `ListaRadicadosPendientesService` ahora construye `DynamicUiTableDto` y define accion `asignacion-tarea`. | `done` | `https://github.com/miguelurueta/MiApp.Services/pull/31` | `n/a` | `merged` |
| `DocuArchi.Api` | `yes` | `TramiteController` deja hardcode y usa claims reales para devolver `DynamicUiTableDto`. | `done` | `https://github.com/miguelurueta/DocuArchi.Api/pull/23` | `n/a` | `merged` |
| `MiApp.Repository` | `no` | La consulta actual parametrizada sigue vigente; no requirio cambios de codigo. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `MiApp.Models` | `no` | El modelo `raradestadosmoduloradicacion` ya cubre la estructura usada por el flujo. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore` | `yes` | Coordinador: tests, docs, spec, tasks y validacion OpenSpec. | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/56`, `https://github.com/miguelurueta/DocuArchiCore/pull/57` | `pending` | `ready_to_archive` |
| `DocuArchiCore.Web` | `no` | El ticket solo ajusta contrato backend/documentacion; no se cambió frontend. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore.Abstractions` | `no` | No se requirio contrato compartido fuera de DTOs existentes. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |

## Operating Rule

Run `opsxj:new SCRUM-71` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-71` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
