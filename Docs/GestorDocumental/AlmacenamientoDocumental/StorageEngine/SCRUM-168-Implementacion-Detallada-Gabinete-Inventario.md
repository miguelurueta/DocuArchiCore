# SCRUM-168 Implementacion Detallada Gabinete Inventario

## Alcance
Documento tecnico de consolidacion para la fase de insercion en gabinete e inventario documental bajo flujo orquestado multi-repo.

## PRs y repos involucrados
- DocuArchiCore (coordinador): PR 225.
- DocuArchi.Api: PR 82.
- MiApp.DTOs: PR 60.
- MiApp.Services: PR 112.
- MiApp.Models: PR 27.
- MiApp.Repository: sin diff publicado en esta corrida (`traceability_only`).

## Archivos impactados por repositorio (resumen)
### DocuArchi.Api
- `Controllers/Radicacion/Tramite/SolicitaEstructuraTipoDocEntranteController.cs`
- `Controllers/Radicacion/Tramite/TramiteController.cs`
- `Program.cs`

### MiApp.DTOs
- `DTOs/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoRequest.cs`
- `DTOs/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoResponse.cs`
- `DTOs/GestorDocumental/AlmacenamientoDocumental/InventarioDocumentalDto.cs`
- y DTOs auxiliares de almacenamiento/indexacion.

### MiApp.Services
- `Service/GestorDocumental/AlmacenamientoDocumental/Transaction/IStorageTransactionCoordinator.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/Transaction/StorageTransactionCoordinator.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/Identity/StorageIdentityAllocator.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/DocumentStorageOrchestrator.cs`
- y componentes de validacion/options/metadata relacionados.

### MiApp.Models
- `Models/GestorDocumental/AlmacenamientoDocumental/StorageContext.cs`
- `Models/GestorDocumental/AlmacenamientoDocumental/StorageIdentityReservationResult.cs`
- `Models/GestorDocumental/AlmacenamientoDocumental/StorageTransactionResult.cs`
- excepciones y enums de estado/fase.

## Integracion transaccional esperada
- El `StorageTransactionCoordinator` mantiene una sola transaccion para:
  - reserva de identidad,
  - insercion en gabinete,
  - insercion en inventario,
  - commit unico.
- Ante error en cualquiera de las fases, el flujo debe ejecutar rollback total.

## DapperCrudEngine y QueryOptions
- Regla obligatoria del cambio: operaciones de DB via `DapperCrudEngine + QueryOptions`.
- Estado de evidencia en esta corrida:
  - `MiApp.Repository` quedo `traceability_only` (sin diff funcional publicado).
  - La evidencia detallada de metodos `InsertValues/ReturnGeneratedIdentity` debe quedar en el PR del repo repository cuando se promueva a `implementation_required`.

## DI y contratos
- `Program.cs` en API fue actualizado en PR satelite para registrar dependencias de la cadena storage.
- Interfaces y contratos de coordinacion/transaccion ya estaban consolidados en SCRUM-166/167 y extendidos en los PRs de SCRUM-168.

## Limitaciones actuales documentadas
- No hay PR de repository para este ticket en la corrida actual.
- Validacion/implementacion concreta de insercion dinamica en tabla gabinete queda pendiente de confirmacion en repo de acceso a datos.
