# SCRUM-166 Arquitectura IdentityAllocator

## Objetivo
Implementar la reserva de identidad documental con compatibilidad legacy para `system1` y validacion de capacidad en `disco_detalle`, bajo transaccion externa y locking fuerte.

## Componentes
- `IStorageIdentityAllocator` / `StorageIdentityAllocator`
- `IStorageIdentityPolicy` / `StorageIdentityPolicy`
- `ISystemStorageRepository` / `SystemStorageRepository`
- `IStorageDiskQuotaRepository` / `StorageDiskQuotaRepository`
- `IStorageDiskQuotaPolicy` / `StorageDiskQuotaPolicy`
- `IDapperCrudEngine` extendido con `LockMode` y `UpdateValues`

## Diagrama de clases (texto)
- Allocator depende de repositorios y politicas.
- Repositorios dependen de `IDapperCrudEngine`.
- Politicas encapsulan reglas legacy (`proxid`, `tamdisc`, `numero_imagenes`, pagina por carpeta).

## Diagrama de secuencia (texto)
1. `StorageIdentityAllocator.ReserveAsync` valida contexto/connection/transaccion.
2. `SystemStorageRepository.LockByGabineteAsync` bloquea `system1` (`FOR UPDATE`).
3. `StorageIdentityPolicy.Calculate` calcula identidad/folder/pages.
4. `StorageDiskQuotaRepository.LockDiskStatusAsync` bloquea `disco_detalle`.
5. `StorageDiskQuotaPolicy.ValidateDiskAvailable` valida umbrales y estado de sincronizacion por `numero_imagenes`.
6. `SystemStorageRepository.UpdateReservationAsync` persiste `proxid/numcarp/NUMPAG_CARP`.
7. Retorna `StorageIdentityReservationResult`.

## Reglas legacy criticas
- `IdAlmacen = ProxId + 1` y `NewProxId = IdAlmacen`.
- `tamdisc` permitido solo en: `572523149`, `4310948432`.
- Rotacion de carpeta si `NumPagCarp + paginasDocumento > 230`.
- Bloqueo por disco sobre limite con reglas `tamdisc + numero_imagenes`.

## Integracion con TransactionCoordinator
- El allocator no abre ni cierra transaccion.
- Usa `IDbConnection` y `IDbTransaction` provistos por capa superior (Prompt 6).

## Riesgos y mitigaciones
- Concurrencia por proxid: mitigada con `FOR UPDATE` + filtro por `proxid` previo en update.
- Riesgo de lock prolongado: minimizado por orden de bloqueo fijo `system1 -> disco_detalle`.
- Inconsistencia intermedia: mitigada por unidad transaccional externa.

## Validacion SOLID
- SRP: politica separada de acceso a datos.
- OCP: nuevas reglas via politicas sin alterar repositorios.
- DIP: servicios dependen de interfaces.
