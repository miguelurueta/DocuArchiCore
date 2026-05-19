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
| DocuArchiCore (coordinador) | OpenSpec, documentacion y pruebas SCRUM-205 completadas |
| DocuArchi.Api | Implementado (Controller + DI) |
| MiApp.DTOs | Implementado (DTOs query/action/mutation/resolve) |
| MiApp.Services | Implementado (Service query/action) |
| MiApp.Repository | Implementado (Repository con `DapperCrudEngine + QueryOptions`) |
| MiApp.Models | Sin cambios requeridos para SCRUM-205 |

## 4. PRs relacionados
- PR coordinador: `https://github.com/miguelurueta/DocuArchiCore/pull/270`
- PRs satelite: pendientes al momento de esta version.

## 5. Criterios de aceptacion
| Criterio | Estado |
|---|---|
| hierarchical funcional | Implementado y cubierto por pruebas unitarias |
| flatDocuments funcional | Implementado y cubierto por pruebas unitarias |
| Semantica unica backend | Implementado (`AppResponses<T>` + contrato dinamico) |
| DapperCrudEngine obligatorio | Implementado |
| QueryOptions obligatorio | Implementado |
| Legacy migrado | Implementado para flujo objetivo de `lista_documentos_relacionados` |
| AppTable intacto | Regresion cubierta por pruebas de contrato |
| Export intacto | Regresion cubierta por pruebas de contrato sobre ruta/endpoint |
| Autocomplete intacto | Regresion cubierta por pruebas de contrato sobre ruta/endpoint |
| Tests completos | Ejecutados para alcance SCRUM-205 (incluye `Skip` de integracion Docker) |
| Documentacion completa | Completada y actualizada con evidencia de ejecucion |

## 6. Validaciones realizadas
- `openspec.cmd validate scrum-205-crea-api-lista-documentos-radicados` -> valido (2026-05-19).

## 7. Proximos hitos
- Merge multi-repo.
- Archivo final (`orchestrate:archive`) posterior al merge (paso 9.3 del OpenSpec).
