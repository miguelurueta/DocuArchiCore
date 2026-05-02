# SCRUM-167 Pruebas TransactionCoordinator

## Matriz unitaria
- `StorageTransactionCoordinatorTests`:
  - Commit exitoso: reserva identidad + update disco.
  - Falla en update de disco: rollback.
  - Workflow `IdTareaWorkflow > 0`: invoca extensión de log.
- `StorageValidationPipelineTests`:
  - Ajuste constructor orchestrator con `IStorageTransactionCoordinator`.

## Cobertura de comportamiento
- `IdentityAllocator` se ejecuta una sola vez por transacción.
- `UpdateDiskUsageAsync` se ejecuta después de la reserva.
- `WorkflowLogInserted` se marca según condición de workflow.
- `DiskUsageUpdated` se marca solo tras update exitoso.

## Evidencia de rollback
- Al lanzar `StorageTransactionException` desde update de disco:
  - no hay commit,
  - rollback se ejecuta una vez.

## Riesgo residual
- Pruebas de integración real con MySQL para confirmar lock/rollback cross-table quedan para entorno de integración.
- En este entorno local `dotnet build/test` de `MiApp.Services` y del proyecto de pruebas falla en `_GetProjectReferenceTargetFrameworkProperties` sin errores CS reportados; se requiere estabilizar esa condición de SDK/MSBuild para ejecutar suite completa.
