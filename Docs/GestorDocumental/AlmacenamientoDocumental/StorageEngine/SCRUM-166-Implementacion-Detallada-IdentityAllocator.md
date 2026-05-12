# SCRUM-166 Implementacion Detallada IdentityAllocator

## Archivos creados
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/SystemStorageRow.cs`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageIdentityReservationResult.cs`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/DiskQuotaStatusModel.cs`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/DiskQuotaUpdateModel.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/SystemStorage/SystemStorageRepository.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Disk/StorageDiskQuotaRepository.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Identity/IStorageIdentityAllocator.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Identity/StorageIdentityAllocator.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Identity/IStorageIdentityPolicy.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Identity/StorageIdentityPolicy.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Identity/StorageDiskQuotaPolicy.cs`

## Archivos modificados
- `MiApp.Repository/Repositorio/DataAccess/DapperCrudEngine.cs`
- `DocuArchi.Api/Program.cs`

## Extension DapperCrudEngine
- Se agrega `QueryOptions.LockMode` con enum `QueryLockMode`.
- Se agrega `QueryOptions.UpdateValues`.
- Se agrega `UpdateBeginTransAsync(...)` para update transaccional tipado.
- `GetAllBeginTransAsync(...)` incorpora `LockMode.ForUpdate`.

## SQL conceptual modelado en QueryOptions
- `system1` lock:
  - select `disco, proxid, tamdisc, numcarp, NUMPAG_CARP`
  - filtro `nombre = @gabinete`
  - `LockMode = ForUpdate`
- `system1` update:
  - update `proxid`, `numcarp`, `NUMPAG_CARP`
  - filtros `nombre = @gabinete` y `proxid = @previousProxId`
- `disco_detalle` lock:
  - select `disco, gabinete, NUMERO_IMAGENES, NUMPAG_CARP`
  - filtros `disco` y `gabinete`
  - `LockMode = ForUpdate`

## Logica de calculo aplicada
- Regla proxid legacy: `IdAlmacen = ProxId + 1`.
- Regla tamdisc legacy: solo dos valores permitidos.
- Regla de carpeta: umbral maximo `230` paginas.
- Regla disco detalle: bloquea por umbrales legacy y por `numero_imagenes` null/0.

## Concurrencia
- Orden de lock: primero `system1`, luego `disco_detalle`.
- Update de `system1` exige `rows == 1`, si no lanza excepcion de concurrencia.

## Deuda tecnica controlada
- `tamdisc` sigue hardcodeado por compatibilidad legacy.
- Actualizacion de `NUMERO_IMAGENES`/`NUMPAG_CARP` en `disco_detalle` queda preparada para integracion completa con Prompt 6.
