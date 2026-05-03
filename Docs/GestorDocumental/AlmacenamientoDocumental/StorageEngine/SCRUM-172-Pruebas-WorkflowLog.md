# SCRUM-172 Pruebas Workflow Log

## Suite objetivo
- `WorkflowStorageLogBuilderTests`
- `WorkflowStorageLogRepositoryTests`
- `StorageTransactionCoordinatorTests` (rama workflow)
- `StorageTransactionCoordinatorIntegrationTests` (commit/rollback con workflow)

## Matriz de cobertura esperada
- Builder:
  - workflow null o `IdTareaWorkflow <= 0` no genera insercion.
  - mapeo de `IdAlmacen`, `usuario`, `radicado`, `ruta`, `tipologia`.
  - serializacion de `Campos` controlada.
- Repository:
  - validaciones de entrada (`model`, `connection`, `transaction`, campos obligatorios).
  - `QueryOptions.TableName = logdocuarchi`.
  - propagacion de contexto transaccional.
  - fallo cuando `rows != 1`.
- Coordinator:
  - no inserta workflow para `IdTareaWorkflow <= 0`.
  - inserta workflow para `IdTareaWorkflow > 0`.
  - rollback total cuando falla insercion workflow.

## Estado actual
- En este repositorio se actualizo la trazabilidad de pruebas en OpenSpec y documentacion.
- Ejecucion consolidada de `dotnet test` para SCRUM-172: pendiente de correr y anexar evidencia final en el cierre multi-repo.

