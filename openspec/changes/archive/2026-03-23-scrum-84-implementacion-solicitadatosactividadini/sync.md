# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-84`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-84`
- Summary: `IMPLEMENTACION-SolicitaDatosActividadInicioFlujoAsync`
- Coordinator change: `openspec/changes/scrum-84-implementacion-solicitadatosactividadini/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| DocuArchiCore | yes | Repo coordinador: OpenSpec, pruebas y documentacion tecnica de SCRUM-84. | done | https://github.com/miguelurueta/DocuArchiCore/pull/77 | pending | in_progress |
| MiApp.Services | yes | `RegistrarRadicacionEntranteService` integra `SolicitaDatosActividadInicioFlujoAsync`, valida actividad inicial y encapsula la prevalidacion workflow en metodos privados. | done | https://github.com/miguelurueta/MiApp.Services/pull/35 | pending | pr_open |
| DocuArchi.Api | yes | `Program.cs` requiere registrar `ISolicitaDatosActividadInicioFlujoRepository` para resolver DI del nuevo flujo. | done | https://github.com/miguelurueta/DocuArchi.Api/pull/29 | pending | pr_open |
| MiApp.Repository | no | No requirio cambios nuevos; la consulta `SolicitaDatosActividadInicioFlujoAsync` ya existia desde SCRUM-83. | n/a | n/a | n/a | n/a |
| MiApp.DTOs | no | No requirio cambios nuevos para este alcance. | n/a | n/a | n/a | n/a |
| MiApp.Models | no | No requirio cambios nuevos; el model ya existia desde SCRUM-83. | n/a | n/a | n/a | n/a |
| DocuArchiCore.Web | no | Sin impacto UI para este ticket. | n/a | n/a | n/a | n/a |
| DocuArchiCore.Abstractions | no | No hubo cambios de contratos compartidos. | n/a | n/a | n/a | n/a |

## Operating Rule

Run `opsxj:new SCRUM-84` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-84` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Current Status Notes

- `DocuArchiCore` ya tiene cambio OpenSpec coordinador y PR abierto.
- `MiApp.Services` ya tiene `opsxj:new` ejecutado, commit funcional empujado (`97d9534`) y PR apilado abierto: `https://github.com/miguelurueta/MiApp.Services/pull/35`. La base correcta del PR es `origin/scrum-82-actualiza-dynamic-ui-table-ant-desing` porque `HEAD^ = 38c10c3` coincide con esa rama.
- `DocuArchi.Api` ya tiene `opsxj:new` ejecutado, commit minimo de `SCRUM-84` empujado (`01591be`) y PR apilado abierto: `https://github.com/miguelurueta/DocuArchi.Api/pull/29`. Para destrabarlo fue necesario publicar primero la base remota `scrum-75-actualiza-registro-radicacion-entrante`. El working tree local sigue con cambios no relacionados, pero quedaron fuera del commit/PR de `SCRUM-84`.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
