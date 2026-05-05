# SCRUM-186 — Arquitectura Logdocuarchi Workflow

## Objetivo
Restaurar paridad VB en `logdocuarchi` para operaciones de almacenamiento con workflow.

## Componentes
- `WorkflowStorageLogBuilder`
- `WorkflowStorageLogService`
- `WorkflowStorageLogRepository`
- Integración en `StorageTransactionCoordinator`
- Poblado de IP desde `AlmacenamientoDocumentalController` usando `IIpHelper` (`IpHelperL`)

## Regla de activación
- Solo inserta log si `Workflow.IdTareaWorkflow > 0`.
- Si no aplica, estado `NO_WORKFLOW` y no inserta.

## Flujo
1. Controller resuelve IP (`IpHelperL`) y la pasa al use case/contexto.
2. Coordinator (dentro de transacción) arma `naming` + `physicalPath`.
3. Service usa builder para crear modelo legacy.
4. Repository inserta en `logdocuarchi`.
5. Si falla el insert y aplica workflow, el coordinator hace rollback.

