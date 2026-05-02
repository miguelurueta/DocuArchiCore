# SCRUM-168 Implementacion Detallada Gabinete Inventario

## Alcance
Documento tecnico de consolidacion para la fase de insercion en gabinete e inventario documental bajo flujo orquestado multi-repo.

## PRs y repos involucrados
- DocuArchiCore (coordinador): PR 225.
- DocuArchi.Api: PR 82.
- MiApp.DTOs: PR 60.
- MiApp.Services: PR 112.
- MiApp.Models: PR 27.
- MiApp.Repository: implementacion funcional en PR 58 (https://github.com/miguelurueta/MiApp.Repository/pull/58).

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
- `Models/GestorDocumental/AlmacenamientoDocumental/GabineteInsertModel.cs`
- `Models/GestorDocumental/AlmacenamientoDocumental/InventarioInsertModel.cs`
- excepciones y enums de estado/fase.

### MiApp.Repository
- `Repositorio/GestorDocumental/AlmacenamientoDocumental/Gabinete/IGabineteStorageRepository.cs`
- `Repositorio/GestorDocumental/AlmacenamientoDocumental/Inventario/IInventarioDocumentalRepository.cs`
- ambos repositories usan `IDapperCrudEngine` con `InsertBeginTrandAsync`.

## Integracion transaccional esperada
- El `StorageTransactionCoordinator` mantiene una sola transaccion para:
  - reserva de identidad,
  - insercion en gabinete,
  - insercion en inventario,
  - actualizacion de estructura fisica/workflow,
  - commit unico.
- Ante error en cualquiera de las fases, el flujo debe ejecutar rollback total.

## Implementacion realizada en coordinator
- Se inyectan `IGabineteStorageRepository` e `IInventarioDocumentalRepository`.
- Se construyen los modelos con:
  - `BuildGabineteInsertModel(...)`
  - `BuildInventarioInsertModel(...)`
- Se ejecuta secuencia:
  1. `ReserveAsync(...)`
  2. `GabineteStorageRepository.InsertAsync(...)`
  3. `InventarioDocumentalRepository.InsertAsync(...)`
  4. commit final.
- `StorageTransactionResult.IdRegistroProduccionDocumental` retorna el id generado en inventario.

## DapperCrudEngine y QueryOptions
- Regla obligatoria del cambio: operaciones de DB via `DapperCrudEngine + QueryOptions`.
- Estado de evidencia:
  - repositorio implementado con `InsertBeginTrandAsync` para mantener inserciones dentro de la misma transaccion.
  - validacion estricta de identificadores dinamicos para tabla y columnas.
  - parametros de negocio enviados como parametros (sin interpolar valores de usuario en SQL).

## DI y contratos
- `Program.cs` en API fue actualizado en PR satelite para registrar dependencias de la cadena storage.
- Interfaces y contratos de coordinacion/transaccion ya estaban consolidados en SCRUM-166/167 y extendidos en los PRs de SCRUM-168.

## Limitaciones actuales documentadas
- Pruebas de integracion/concurrencia agregadas en coordinator; validacion E2E con infraestructura completa sigue pendiente de cierre.
- `dotnet test` integral del proyecto de pruebas actualmente bloqueado por resolucion de referencias MSBuild (`_GetProjectReferenceTargetFrameworkProperties`) fuera del alcance funcional puntual de SCRUM-168.
- Sin archive final mientras no se complete merge de PRs satelite.
