# SCRUM-166 Cobertura Legacy IdentityAllocator

## Mapeo Legacy -> Nuevo componente
- `system1 FOR UPDATE` -> `SystemStorageRepository.LockByGabineteAsync`
- `proxid` legacy -> `StorageIdentityPolicy.Calculate`
- `tamdisc` permitido -> `StorageIdentityPolicy.Calculate`
- `numcarp`/`NUMPAG_CARP` -> `StorageIdentityPolicy.Calculate`
- `disco_detalle FOR UPDATE` -> `StorageDiskQuotaRepository.LockDiskStatusAsync`
- `EstadoDisco = SL` -> `StorageDiskQuotaPolicy.ValidateDiskAvailable`
- update `system1` transaccional -> `SystemStorageRepository.UpdateReservationAsync`

## Cobertura DapperCrudEngine
- Metodo lectura transaccional: `GetAllBeginTransAsync`.
- Metodo update transaccional: `UpdateBeginTransAsync`.
- Construccion por `QueryOptions` con filtros parametrizados.
- Lock aplicado con `LockMode = ForUpdate`.
- Update aplicado con `UpdateValues`.

## Evidencia de cumplimiento de reglas
- No se usa Dapper directo en repositorios nuevos.
- No se construye SQL manual concatenado.
- No se abre/cierra transaccion desde allocator ni repositorios.
