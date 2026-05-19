## Context

- Jira issue key: SCRUM-205
- Jira summary: CREA-API-LISTA-DOCUMENTOS-RADICADOS
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-205

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se requiere una API backend para listar documentos radicados en dos modos (`hierarchical` y `flatDocuments`) y ejecutar
acciones por fila, migrando la semántica del flujo legacy `lista_documentos_relacionados` sin exponer contratos antiguos
de bootstrap-table ni respuestas tipo string (`YES/NO`).

El cambio debe mantener compatibilidad con el ecosistema actual de AppTable/workflow inbox, reutilizar contratos dinámicos
(`DynamicUiTableDto`) y cumplir estrictamente las reglas de `OPSXJ_BACKEND_RULES` (capas, interfaces, DI, AppResponses,
try/catch, pruebas).

## Legacy Reference

- `D:\imagenesda\GestorDocumental\promp\CORE-API\workflow\Legazy\lista_documentos_relacionados.txt`
- Funciones legacy objetivo:
  - `SolicitaDocumentosRelacionadosRadicadoEnlace`
  - `SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot`
  - `Lista_campos_documentos_relacionados`

## Scope

### In Scope

- Endpoint `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`.
- Endpoint `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`.
- Soporte de `ViewMode`:
  - `hierarchical` con expansión por `ParentRowId` y `Level`.
  - `flatDocuments` con filas de documento plano.
- Migración de consulta legacy con `DapperCrudEngine + QueryOptions` y SQL parametrizado.
- Respuesta con `AppResponses<T>` y contrato dinámico (`DynamicUiTableDto` o `DynamicUiRowsOnlyDto`).
- Integración de acción `ver_documento` reutilizando `/api/gestor-documental/documentos/visualizacion/resolve`.

### Out of Scope

- Cambios de frontend.
- Nuevos endpoints para preview/visualización.
- Reescritura de `workflowInboxgestion` existente.
- Cambios de esquema de base de datos no requeridos por la migración.

## Target Architecture

- Patrón obligatorio: `Controller -> Service -> Repository`.
- Claims obligatorios: `defaulalias`, `usuarioid`.
- Repository sin lógica de presentación; solo acceso de datos.
- Service concentra validación de modo (`hierarchical`/`flatDocuments`), dispatch de acciones y normalización de contrato.

Rutas esperadas:

- Controller: `DocuArchi.Api/Controllers/GestorDocumental/Documentos/ListaDocumentosRadicadoController.cs`
- Service: `MiApp.Services/Service/GestorDocumental/Documentos/ListaDocumentosRadicadoService.cs`
- Repository: `MiApp.Repository/Repositorio/GestorDocumental/Documentos/ListaDocumentosRadicados`
- DTOs: `MiApp.DTOs/DTOs/GestorDocumental/Documentos/ListaDocumentosRadicados`

## Contract Decisions

### AD-01 Query Contract

`ListaDocumentosRadicadosTreeQueryRequestDto` debe soportar paginación, sort, filtros estructurados, `IncludeConfig`,
`ViewMode`, contexto de nodo padre (`ParentRowId`, `ParentNodeType`) y `Level`.

### AD-02 Action Contract

`ListaDocumentosRadicadosTreeActionRequestDto` define `ActionId` y `Payload` para operaciones por fila.
Primer set soportado: `ver_documento`, `agregar_item`, `eliminar_item`.

### AD-03 Row Metadata Normalization

Cada fila debe incluir `Meta` mínimo:
`NodeType`, `ParentId`, `HasChildren`, `CanAddChild`, `CanDelete`, `DocumentId`, `NombreGabinete`.

### AD-04 Action Response Normalization

`ListaDocumentosRadicadosTreeMutationResultDto` unifica resultado:
`Operation`, `AffectedRowId`, `ParentRowId`, `RequiresReloadNode`, `Row`, `DocumentResolveRequest`.

## Data Access Strategy

- Todo acceso DB usa `DapperCrudEngine` y `QueryOptions`.
- Prohibido: concatenación SQL, `QueryAsync` manual directo, `ExecuteAsync` manual directo.
- En `flatDocuments`, la consulta migra campos legacy requeridos (`ID`, `DBT`, `PAG`, `TIPODOCUMENTO`,
  `ESTADO_FIRMA_DIGITAL`) y los proyecta en `Values`/`Meta`.

## Compatibility Rules

No debe romper:

- `POST /api/workflowInboxgestion/inboxgestion`
- `POST /api/workflowInboxgestion/inboxgestion/autocomplete`
- `POST /api/AppTable/export`

## Observability and Error Handling

- `try/catch` obligatorio en Controller, Service y Repository.
- `AppResponses<T>` como contrato único de salida.
- Logs mínimos:
  - `Information`: inicio query/action, `ViewMode`, `ActionId`, `rowCount`.
  - `Warning`: acción inválida, `ParentRowId` inválido, sin resultados.
  - `Error`: excepción controlada de DB/mapping/acción.

## Testing Strategy

- Controller tests: claims inválidos, query success, action success.
- Service tests:
  - Query `hierarchical` (raíz/hijos) y `flatDocuments`.
  - Action `ver_documento`, `agregar_item`, `eliminar_item`, acción inválida.
- Repository integration tests: consulta parametrizada y mapping legacy esperado.
- Contract tests: shape `AppResponses<T>` y compatibilidad de contrato dinámico.

## Documentation Deliverables

- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/SCRUM-205-Arquitectura.md`
- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/SCRUM-205-Implementacion-Detallada.md`
- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/SCRUM-205-Integracion-Frontend.md`
- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/SCRUM-205-Pruebas.md`
- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/SCRUM-205-Observabilidad.md`
- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/SCRUM-205-Seguridad.md`
- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/SCRUM-205-Metadata.md`

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-205-crea-api-lista-documentos-radicados.
