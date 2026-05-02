# SCRUM-165 Implementacion Detallada ValidationPipeline

## Archivos creados
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Validation/*`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Preindex/*`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Metadata/*`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Options/*`
- `tests/TramiteDiasVencimiento.Tests/StorageValidationPipelineTests.cs`

## Archivos modificados
- `MiApp.DTOs/.../AlmacenarDocumentoRequest.cs`
- `MiApp.DTOs/.../ExpedienteStorageDto.cs`
- `MiApp.Models/.../AlmacenarDocumentoCommand.cs`
- `MiApp.Models/.../StorageContext.cs`
- `MiApp.Models/.../(StoragePreindexResult, GabineteFieldMetadata, StorageOptionsModel).cs`
- `MiApp.Services/.../AlmacenarDocumentoUseCase.cs`
- `MiApp.Services/.../DocumentStorageOrchestrator.cs`
- `DocuArchi.Api/Program.cs`

## Reglas implementadas
- Batch preindex: `PREINDEX_NOT_FOUND`, `PREINDEX_PATH_INVALID`, `PREINDEX_INVALID_FORMAT`, `PREINDEX_READ_ERROR`.
- Metadata gabinete: `GAB_FIELDS_NOT_FOUND`, `GAB_FIELDS_MISMATCH`, `GAB_FIELD_UNKNOWN`, `GAB_REQUIRED_EMPTY`.
- Opciones legacy/inventario: `INV_REQUIRED`, `INV_USER_REQUIRED`, `INV_EMPRESA_REQUIRED`.
- TRD: validacion de IDs negativos.
- Expediente/Unidad: clase documento requerida cuando aplica.

## Integracion
- Orchestrator corta flujo con `StorageValidationException` cuando `IsValid=false`.
- DI agregada en `Program.cs` bajo Services (L).
