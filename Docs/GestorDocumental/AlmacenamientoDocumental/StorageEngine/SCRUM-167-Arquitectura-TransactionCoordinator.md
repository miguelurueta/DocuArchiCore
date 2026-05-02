# SCRUM-167 Arquitectura TransactionCoordinator

## Objetivo
Extender el flujo transaccional del StorageEngine para coordinar reserva de identidad y actualización de cuotas de disco en una sola transacción serializable.

## Componentes
- `IStorageTransactionCoordinator` / `StorageTransactionCoordinator`
- `IStorageIdentityAllocator`
- `IStorageDiskQuotaRepository`
- `IWorkflowStorageLogRepository` (punto de extensión)
- `IDbConnectionFactory`

## Flujo extendido
1. Orchestrator valida request con `IStorageValidationPipeline`.
2. Coordinator abre conexión y transacción `IsolationLevel.Serializable`.
3. Ejecuta `IStorageIdentityAllocator` (lock `system1` + lock `disco_detalle` + update `system1`).
4. Relee `disco_detalle` dentro de la misma transacción para calcular cuota final.
5. Ejecuta `UpdateDiskUsageAsync`.
6. Ejecuta extensión de workflow log si `IdTareaWorkflow > 0`.
7. Commit final único.
8. Ante cualquier error: rollback total.

## Orden de locks recomendado
- `system1`
- `disco_detalle`
- (futuro) `gabinete/inventario/expediente/unidad/indice/workflow`

## Relación con legacy
- Preserva lock fuerte y commit único.
- Conserva regla de rollback integral frente a falla posterior a reserva.
- Mantiene actualización de `disco_detalle` antes del commit.

## Relación con Prompt 11
- Se deja interfaz de integración `IWorkflowStorageLogRepository`.
- El SQL de `logdocuarchi` se implementará en prompt posterior sin romper el coordinador.

## SOLID
- SRP: Coordinator solo coordina transacción/orden.
- DIP: depende de interfaces (`allocator`, `repository`, `workflow log`).
- OCP: fases futuras se agregan por nuevas dependencias sin romper contrato actual.
