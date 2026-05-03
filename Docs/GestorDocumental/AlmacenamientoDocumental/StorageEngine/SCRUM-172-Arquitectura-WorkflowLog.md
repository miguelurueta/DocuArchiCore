# SCRUM-172 Arquitectura Workflow Log

## Objetivo arquitectonico
Agregar una fase transaccional de trazabilidad workflow en Storage Engine para registrar en `logdocuarchi` los documentos almacenados cuando `IdTareaWorkflow > 0`, manteniendo compatibilidad legacy y rollback total ante fallos.

## Componentes
- `StorageTransactionCoordinator` (`MiApp.Services`): decide si aplica fase workflow y orquesta commit/rollback.
- `IWorkflowStorageLogBuilder` + `WorkflowStorageLogBuilder` (`MiApp.Services`): mapeo desde `StorageContext` y resultado transaccional.
- `IWorkflowStorageLogRepository` + `WorkflowStorageLogRepository` (`MiApp.Repository`): persistencia en `logdocuarchi`.
- `WorkflowStorageLogModel` (`MiApp.Models`): contrato tipado de escritura.

## Flujo transaccional
1. Coordinator ejecuta identidad + gabinete + inventario.
2. Evalua `context.Command.Workflow?.IdTareaWorkflow`.
3. Si `IdTareaWorkflow <= 0`: no registra workflow log.
4. Si `IdTareaWorkflow > 0`:
   - construye `WorkflowStorageLogModel`,
   - inserta en `logdocuarchi` dentro de la misma transaccion.
5. Commit unico global o rollback total.

## Reglas de arquitectura
- Repository exclusivamente con `DapperCrudEngine + QueryOptions`.
- Prohibido SQL concatenado o Dapper directo en repositories.
- Builder sin acceso a BD ni filesystem.
- Coordinator no delega `commit/rollback` fuera de su capa.

## Integracion con SCRUM previos
- SCRUM-166: utiliza `IdAlmacen` reservado por Identity Allocator.
- SCRUM-167: extiende el coordinator transaccional.
- SCRUM-168/169: workflow log es fase posterior a gabinete/inventario y compatible con fase archivistica.

