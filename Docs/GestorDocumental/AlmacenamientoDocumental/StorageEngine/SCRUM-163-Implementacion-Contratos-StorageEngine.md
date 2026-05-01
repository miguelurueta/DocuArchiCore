# SCRUM-163 Implementacion de Contratos Storage Engine

## Repos y rutas
- `MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/*`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/*`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Builders/*`

## DTOs creados
- `AlmacenarDocumentoRequest`
- `AlmacenarDocumentoResponse`
- `DocumentoEntradaDto`
- `CampoIndexacionDto`
- `InventarioDocumentalDto`
- `TrdStorageDto`
- `ExpedienteStorageDto`
- `WorkflowStorageDto`

## Models y Enums creados
- `AlmacenarDocumentoCommand`, `StorageContext`, `StorageIdentityModel`, `StorageMetadataModel`, `StoragePlanModel`
- `AlmacenarDocumentoResult`, `StorageError`, `StorageValidationResult`
- `StorageIdempotencyModel`, `StoragePhysicalStatusModel`, `StorageFilePlanModel`, `StorageXmlModel`, `StorageCompensationPlan`, `StorageTransactionResult`
- `TipoAlmacenamientoEnum`, `StorageDocumentState`, `StoragePhase`

## Exceptions creadas
- `StorageException`
- `StorageValidationException`
- `StorageTransactionException`
- `StoragePhysicalException`

## Decisiones
- Sin `ByRef` y sin parametros sueltos.
- `DocumentoEntradaDto` usa `ArchivoTemporalId` (no `RutaArchivo`).
- `RequestId` presente en contratos clave para idempotencia.
