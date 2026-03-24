## Repo Impact Plan

Ticket: SCRUM-91  
Summary: ACTUALIZACION-ValidarUsuarioWorkflowInternoAsync  
Change: openspec/changes/scrum-91-actualizacion-validarusuarioworkflowinte/

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | opsxj:new | PR | opsxj:archive | Estado |
|---|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `yes` | `traceability_only` | `trazabilidad centralizada sin diff funcional` | `done` | `https://github.com/miguelurueta/DocuArchi.Api/pull/33` | `pending` | `tracked` |
| `DocuArchiCore` | `yes` | `implementation_required` | `orquestador openspec central y pruebas/documentacion del ticket` | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/94` | `pending` | `merged` |
| `DocuArchiCore.Abstractions` | `no` | `no_code_change` | `solo consulta (sin cambios)` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | `no_code_change` | `solo consulta (sin cambios)` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `yes` | `traceability_only` | `trazabilidad centralizada sin diff funcional` | `done` | `https://github.com/miguelurueta/MiApp.DTOs/pull/20` | `pending` | `tracked` |
| `MiApp.Services` | `yes` | `implementation_required` | `cambio funcional en RegistrarRadicacionEntranteService` | `done` | `https://github.com/miguelurueta/MiApp.Services/pull/44` | `pending` | `merged` |
| `MiApp.Repository` | `yes` | `traceability_only` | `trazabilidad centralizada sin diff funcional` | `done` | `https://github.com/miguelurueta/MiApp.Repository/pull/23` | `pending` | `tracked` |
| `MiApp.Models` | `yes` | `traceability_only` | `trazabilidad centralizada sin diff funcional` | `done` | `https://github.com/miguelurueta/MiApp.Models/pull/10` | `pending` | `tracked` |

## Rule

- `implementation_required` exige branch, commit, PR y merge para ese repo.
- `traceability_only` mantiene la trazabilidad del ticket sin requerir merge funcional para archivar.
- `no_code_change` deja el repo explicitamente fuera de cambios.
