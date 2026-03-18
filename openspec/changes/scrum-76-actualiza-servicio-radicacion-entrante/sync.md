# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-76`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-76`
- Summary: `ACTUALIZA-SERVICIO-RADICACION-ENTRANTE`
- Coordinator change: `openspec/changes/scrum-76-actualiza-servicio-radicacion-entrante/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `MiApp.DTOs` | `yes` | El request `RegistrarRadicacionEntranteRequestDto` deja de exponer `IdPlantilla` en el contrato JSON. | `pending` | `pending` | `n/a` | `implementando` |
| `MiApp.Services` | `yes` | `RegistrarRadicacionEntranteAsync` resuelve la plantilla default y propaga el id interno al flujo de registro. | `pending` | `pending` | `n/a` | `implementando` |
| `MiApp.Repository` | `no` | Ya existia `SolicitaEstructuraPlantillaRadicacionDefault`; no fue necesario cambio fuente adicional. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchi.Api` | `no` | El endpoint mantiene firma; el cambio contractual llega por el DTO compartido sin editar controller. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `MiApp.Models` | `no` | No se requiere nuevo modelo. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore` | `yes` | Coordinador: tests, docs, spec, tasks y validacion OpenSpec. | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/68` | `pending` | `implementando` |
| `DocuArchiCore.Web` | `no` | No hay cambio de frontend dentro del repo web. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore.Abstractions` | `no` | No se requiere contrato compartido adicional. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |

## Operating Rule

Run `opsxj:new SCRUM-76` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-76` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
