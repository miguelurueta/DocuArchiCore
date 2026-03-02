# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-30`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-30`
- Summary: `CREA-API-LISTA-RADICADOS-PEDIENTES`
- Coordinator change: `openspec/changes/scrum-30/`
- Archived change: `openspec/changes/archive/2026-03-02-scrum-30/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `yes` | `Crear endpoint ApListaRadicadosPendientes y DI de servicio/repositorio` | `done` | `https://github.com/miguelurueta/DocuArchi.Api/pull/4` | `n/a` | `merged` |
| `DocuArchiCore` | `yes` | `Coordinacion OpenSpec + documentacion tecnica/diagramas` | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/9` | `done` | `merged` |
| `DocuArchiCore.Abstractions` | `no` | `Sin cambios de contratos cross-cutting solicitados` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | `No se solicitan cambios de UI, solo guia de integracion frontend` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `yes` | `Crear DTO salida ListaRadicadosPendientes para tabla MUI` | `done` | `https://github.com/miguelurueta/MiApp.DTOs/pull/3` | `n/a` | `merged` |
| `MiApp.Services` | `yes` | `Implementar ServiceListaRadicadosPendientes y mapping` | `done` | `https://github.com/miguelurueta/MiApp.Services/pull/4` | `n/a` | `merged` |
| `MiApp.Repository` | `yes` | `Implementar SolicitaListaRadicadosPendientes y consultas dependientes` | `done` | `https://github.com/miguelurueta/MiApp.Repository/pull/4` | `n/a` | `merged` |
| `MiApp.Models` | `yes` | `Crear modelo raradestadosmoduloradicacion en Radicacion/Tramite` | `done` | `https://github.com/miguelurueta/MiApp.Models/pull/2` | `n/a` | `merged` |

## Operating Rule

Run `opsxj:new SCRUM-30` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-30` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Blockers

- Ninguno: implementacion, PRs y archivado completados.
