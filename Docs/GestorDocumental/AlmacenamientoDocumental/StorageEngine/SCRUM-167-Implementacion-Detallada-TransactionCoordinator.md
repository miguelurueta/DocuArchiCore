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
- Se agrega punto de extensión `IWorkflowStorageLogRepository` sin SQL embebido.

## StorageTransactionResult extendido
- `IdentityReservation`
- `Success`
- `Estado`
- `RequestId`
- `FechaEjecucion`
- `DuracionMs`
- `DiskUsageUpdated`
- `WorkflowLogInserted`

## Regla de actualización disco_detalle
- Cálculo aplicado:
  - `NuevoNumeroImagenes = NumeroImagenesActual + NumeroPaginasDocumento`
  - `NuevoNumPagCarp = reservation.NewFolderPages`
- Si `rows != 1`: `StorageTransactionException`.

## Integración de orchestrator
- `DocumentStorageOrchestrator` ahora:
  - mantiene validación pipeline,
  - delega fase DB a `IStorageTransactionCoordinator`,
  - retorna `IdAlmacen` reservado por transacción.

## DapperCrudEngine
- Coordinator no ejecuta SQL directo.
- Todo acceso DB permanece en repositorios (`SystemStorageRepository`, `StorageDiskQuotaRepository`) por `DapperCrudEngine + QueryOptions`.
