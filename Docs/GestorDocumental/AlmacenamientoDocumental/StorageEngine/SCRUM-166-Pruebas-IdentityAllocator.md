# SCRUM-166 Pruebas IdentityAllocator

## Matriz unitaria
- `StorageIdentityPolicy`:
  - `ProxId <= 0` invalido.
  - `Disco <= 0` invalido.
  - `NumCarp <= 0` invalido.
  - `NumPagCarp < 0` invalido.
  - `numeroPaginasDocumento <= 0` invalido.
  - `TamDisc` invalido bloquea.
  - `IdAlmacen = ProxId + 1`.
  - Rotacion de carpeta al superar 230 paginas.
- `StorageDiskQuotaPolicy`:
  - `status = null` bloquea.
  - `numero_imagenes null` bloquea.
  - `numero_imagenes = 0` bloquea.
  - Umbrales `tamdisc + numero_imagenes` bloquean o continúan según regla legacy.
- `StorageIdentityAllocator`:
  - valida contexto, conexion abierta y transaccion no nula.
  - valida lock de `system1` y `disco_detalle`.
  - falla si update de `system1` no afecta una fila.

## Matriz repositorio/integracion aislada
- `SystemStorageRepositoryTests` valida construccion de `QueryOptions` con lock y update.
- `StorageDiskQuotaRepositoryTests` valida filtros, lock y update via `DapperCrudEngine`.
- Se valida ausencia de uso directo de `ExecuteAsync/QueryAsync/ExecuteScalarAsync` en repositorios nuevos.

## Evidencia de ejecucion
- Tests agregados en `tests/TramiteDiasVencimiento.Tests`:
  - `StorageIdentityPolicyTests.cs`
  - `StorageDiskQuotaPolicyTests.cs`
  - `StorageIdentityAllocatorTests.cs`
  - `SystemStorageRepositoryTests.cs`
  - `StorageDiskQuotaRepositoryTests.cs`
- Build y test se ejecutan en el flujo SCRUM-166 para validar compilacion y cobertura minima del prompt.

## Riesgo residual
- Validacion de integracion multi-hilo real y lock behavior depende de entorno MySQL de integracion.
