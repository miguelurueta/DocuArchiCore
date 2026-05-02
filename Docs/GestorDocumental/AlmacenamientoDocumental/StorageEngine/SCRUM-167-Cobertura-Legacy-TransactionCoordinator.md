# SCRUM-167 Cobertura Legacy TransactionCoordinator

## Mapeo Legacy -> Componente actual
- `commit/rollback` central legacy -> `StorageTransactionCoordinator`
- update `disco_detalle` (`NUMERO_IMAGENES`, `NUMPAG_CARP`) -> `IStorageDiskQuotaRepository.UpdateDiskUsageAsync`
- `logdocuarchi` condicionado por workflow -> `IWorkflowStorageLogRepository` (extensión preparada)
- lock fuerte previo de identidad/disco -> `IStorageIdentityAllocator` (Prompt 5)

## Cobertura transaccional
- Una sola conexión por request.
- Una sola transacción serializable.
- Sin commit parcial.
- Rollback integral en cualquier falla.

## Cobertura DapperCrudEngine
- Coordinator no usa SQL ni Dapper directo.
- Repositorios de `system1` y `disco_detalle` mantienen `QueryOptions` + `LockMode/UpdateValues`.
- Contexto transaccional se propaga vía `IDbConnection + IDbTransaction`.
