# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-74`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-74`
- Summary: `CREA-REPOSITORY-SOLICITA-ESTRUCTURA-CONFIGURACION-PLANTILLA`
- Coordinator change: `openspec/changes/scrum-74-crea-repository-solicita-estructura-conf/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `MiApp.Repository` | `yes` | Se crea un repository dedicado para consultar `ra_rad_config_plantilla_radicacion` por `idPlantilla`. | `pending` | `pending` | `n/a` | `implementado_local` |
| `DocuArchi.Api` | `yes` | Solo registra el repository en DI para consumo interno. | `pending` | `pending` | `n/a` | `implementado_local` |
| `MiApp.Models` | `no` | Se reutiliza el modelo existente `RaRadConfigPlantillaRadicacion`; no requiere cambios. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `MiApp.DTOs` | `no` | No se agregan DTOs nuevos; la respuesta usa el modelo existente en lista. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `MiApp.Services` | `no` | El ticket prohíbe crear service adicional para esta función. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore` | `yes` | Coordinador: pruebas, docs, spec, tasks y validacion OpenSpec. | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/65` | `pending` | `ready_for_validation` |
| `DocuArchiCore.Web` | `no` | No hay frontend nuevo en alcance; solo documentación de consumo interno. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore.Abstractions` | `no` | No se requiere contrato compartido adicional. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |

## Operating Rule

Run `opsxj:new SCRUM-74` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-74` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
