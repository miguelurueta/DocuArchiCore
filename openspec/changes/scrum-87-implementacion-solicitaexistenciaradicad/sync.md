# Repo Impact Plan

## Ticket

- Jira key: `SCRUM-87`
- Summary: `IMPLEMENTACION-SolicitaExistenciaRadicadoRutaWorkflowAsync`
- Coordinator change: `openspec/changes/scrum-87-implementacion-solicitaexistenciaradicad/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| DocuArchiCore | yes | Coordinador con tests, spec, tasks, sync y documentación técnica del flujo actualizado de `RegistrarRadicacionEntranteAsync`. | done | https://github.com/miguelurueta/DocuArchiCore/pull/81 | pending | in-review |
| MiApp.Services | yes | El servicio `RegistrarRadicacionEntranteAsync` integra `SolicitaExistenciaRadicadoRutaWorkflowAsync` como paso interno del flujo workflow. | done | https://github.com/miguelurueta/MiApp.Services/pull/40 | pending | in-review |
| MiApp.Repository | no | El repositorio de consulta workflow ya existe y no requiere cambios para esta integración. | n/a | n/a | n/a | out-of-scope |
| MiApp.DTOs | no | No se modifica el contrato público; el resultado de la consulta workflow permanece interno al flujo. | n/a | n/a | n/a | out-of-scope |
| MiApp.Models | no | No hay cambios de modelo para esta integración. | n/a | n/a | n/a | out-of-scope |
| DocuArchi.Api | no | El endpoint standalone ya existe; el cambio solicitado ocurre en la orquestación interna del servicio. | n/a | n/a | n/a | out-of-scope |
| DocuArchiCore.Abstractions | no | Sin impacto en contratos compartidos. | n/a | n/a | n/a | out-of-scope |
| DocuArchiCore.Web | no | Sin impacto frontend para este ticket. | n/a | n/a | n/a | out-of-scope |

## Operating Rule

- Ejecutar `opsxj:new SCRUM-87` solo en repos con `Impacta? = yes`.
- Ejecutar `opsxj:archive SCRUM-87` solo cuando cada PR impactado esté en estado `MERGED`.
- El resultado de `SolicitaExistenciaRadicadoRutaWorkflowAsync` debe mantenerse dentro del flujo interno y no ampliar el contrato público del endpoint.
