# SCRUM-167 Implementacion Detallada TransactionCoordinator

## Archivos creados
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Transaction/IStorageTransactionCoordinator.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Transaction/StorageTransactionCoordinator.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Transaction/IWorkflowStorageLogRepository.cs`

## Archivos modificados
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageTransactionResult.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/DocumentStorageOrchestrator.cs`
- `DocuArchi.Api/Program.cs`

## Cambios funcionales principales
- Se introduce `StorageTransactionCoordinator` con:
  - conexión única (`IDbConnectionFactory`),
  - transacción única `Serializable`,
  - rollback protegido en cualquier excepción.
- Se integra `IStorageIdentityAllocator` como primera fase transaccional.
- Se agrega actualización final de `disco_detalle` en misma transacción.
- Se integra inserción obligatoria de gabinete dinámico (`context.NombreGabinete`) antes del `Commit`.
- Se integra inserción condicional de inventario, expediente/unidad, índice lógico y workflow en la misma transacción.
- Se agrega punto de extensión `IWorkflowStorageLogRepository` sin SQL embebido.

## Secuencia transaccional vigente
1. Reserva de identidad (`system1`).
2. Lock + actualización de cuota (`disco_detalle`).
3. Inserción de fila en gabinete dinámico (`ID`, `DISC`, `PAG`, `DBT`, `IDEX`, `USER`, `DATE1`, `TIME1` + dinámicos).
4. Inserción de `registro_producion_documental` cuando aplica.
5. Ejecución de reglas expediente/unidad cuando aplica.
6. Inserción de `ra_cert_indice_expediente` cuando aplica expediente e inventario válido.
7. Inserción de workflow cuando aplica.
8. `Commit`.

## Reglas funcionales añadidas
- `DBT` de gabinete se toma de `DA_EXTENSION.ESTADO_NORMAL` (no de `TRD.IdTipoDocumento`).
- Si faltan descriptivos de tipología, el coordinator puede recuperar `TIPODOCUMENTO` desde gabinete por `IdAlmacen` para evitar `NA`.
- El `UNIDADCONSERVA` en inventario se persiste `NULL` cuando no hay código válido para evitar violación FK contra `unidad_conservacion.CODIGO_UNICO`.

## StorageTransactionResult extendido
- `IdentityReservation`
- `Success`
- `Estado`
- `RequestId`
- `FechaEjecucion`
- `DuracionMs`
- `DiskUsageUpdated`
- `WorkflowLogInserted`
- `IdRegistroProduccionDocumental` (cuando aplica inventario)
- resultados de expediente/índice para trazabilidad de rama

## Regla de actualización disco_detalle
- Cálculo aplicado:
  - `NuevoNumeroImagenes = NumeroImagenesActual + NumeroPaginasDocumento`
  - `NuevoNumPagCarp = reservation.NewFolderPages`
- Si `rows != 1`: `StorageTransactionException`.

## Integración de orchestrator
- `DocumentStorageOrchestrator` ahora:
  - mantiene validación pipeline,
  - delega fase DB a `IStorageTransactionCoordinator`,
  - retorna `IdAlmacen` reservado por transacción,
  - ejecuta compensación DB cuando falla fase física post-commit.

## DapperCrudEngine
- Coordinator no ejecuta SQL directo.
- Todo acceso DB permanece en repositorios (`SystemStorageRepository`, `StorageDiskQuotaRepository`) por `DapperCrudEngine + QueryOptions`.
