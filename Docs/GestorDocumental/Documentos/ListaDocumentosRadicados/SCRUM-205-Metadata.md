# SCRUM-205 Metadata ListaDocumentosRadicados

## 1. Identificacion
- Ticket Jira: `SCRUM-205`
- Titulo: `CREA-API-LISTA-DOCUMENTOS-RADICADOS`
- Change OpenSpec: `scrum-205-crea-api-lista-documentos-radicados`
- Fecha documento: `2026-05-19`
- Version documento: `v1.0`

## 2. Autoria
- Autor tecnico: equipo backend DocuArchiCore (orquestacion Codex)
- Revisor: pendiente asignacion

## 3. Repos impactados
| Repositorio | Estado |
|---|---|
| DocuArchiCore (coordinador) | Refinamiento OpenSpec y documentacion completados |
| DocuArchi.Api | Pendiente implementacion |
| MiApp.DTOs | Pendiente implementacion |
| MiApp.Services | Pendiente implementacion |
| MiApp.Repository | Pendiente implementacion |
| MiApp.Models | Pendiente validacion de necesidad |

## 4. PRs relacionados
- PR coordinador: `https://github.com/miguelurueta/DocuArchiCore/pull/270`
- PRs satelite: pendientes al momento de esta version.

## 5. Criterios de aceptacion
| Criterio | Estado |
|---|---|
| hierarchical funcional | Pendiente implementacion |
| flatDocuments funcional | Pendiente implementacion |
| Semantica unica backend | Pendiente implementacion |
| DapperCrudEngine obligatorio | Definido en diseno/spec; pendiente verificacion en codigo |
| QueryOptions obligatorio | Definido en diseno/spec; pendiente verificacion en codigo |
| Legacy migrado | Pendiente implementacion |
| AppTable intacto | Pendiente regresion |
| Export intacto | Pendiente regresion |
| Autocomplete intacto | Pendiente regresion |
| Tests completos | Pendiente ejecucion |
| Documentacion completa | Base documental creada en esta version |

## 6. Validaciones realizadas
- `openspec.cmd validate scrum-205-crea-api-lista-documentos-radicados` -> valido (2026-05-19).

## 7. Proximos hitos
- Implementacion multi-repo.
- Ejecucion de pruebas.
- Publicacion (`orchestrate:publish`) y archivo final (`orchestrate:archive`) tras merge.

