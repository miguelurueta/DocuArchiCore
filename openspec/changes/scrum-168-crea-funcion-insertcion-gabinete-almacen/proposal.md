## Why

Se requiere implementar la persistencia transaccional principal del StorageEngine para SCRUM-168:
- insercion segura en gabinete dinamico,
- insercion en `registro_producion_documental`,
- integracion de ambas operaciones en la transaccion existente del `StorageTransactionCoordinator`.

## What Changes

- Implementar modelos de insercion (`GabineteInsertModel`, `InventarioInsertModel`).
- Implementar repositorios (`IGabineteStorageRepository`, `IInventarioDocumentalRepository`) en `MiApp.Repository`.
- Integrar la ejecucion de ambos repositorios en `StorageTransactionCoordinator`.
- Endurecer validaciones de SQL dinamico (identificadores y columnas dinamicas).
- Completar pruebas unitarias/integracion/concurrencia y documentacion tecnica obligatoria.

## Capabilities

### New Capabilities
- jira-scrum-168: Persistencia transaccional de gabinete e inventario documental con reglas DapperCrudEngine.

### Modified Capabilities
- None.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-168
- OpenSpec change path: openspec/changes/scrum-168-crea-funcion-insertcion-gabinete-almacen/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Repositorio critico pendiente de diff funcional: `MiApp.Repository`.
