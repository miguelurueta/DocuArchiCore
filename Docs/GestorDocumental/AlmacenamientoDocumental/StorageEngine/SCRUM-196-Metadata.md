# SCRUM-196 - Metadata

## Identificación
- Ticket: `SCRUM-196`
- Tema: `Validación backend por metadata real de gabinete`
- Estado documental: `Implementación técnica y pruebas focales registradas`

## Documento técnico principal
- `SCRUM-196-Implementacion-Validacion-Metadata-Gabinete.md`

## Documento de contrato API actualizado
- `SCRUM-171-Integracion-API-Frontend.md`

## Documento de pruebas
- `SCRUM-196-Pruebas-Validacion-Metadata-Gabinete.md`

## Repositorios impactados
- `MiApp.Models`
- `MiApp.Repository`
- `MiApp.Services`
- `DocuArchi.Api`
- `DocuArchiCore` (OpenSpec + pruebas + documentación)

## Componentes clave
- `IStorageGabineteMetadataRepository` / `StorageGabineteMetadataRepository`
- `IStorageGabineteMetadataProvider` / `StorageGabineteMetadataProvider`
- `GabineteRequiredFieldsValidator`
- `StorageMetadataOptions`

## Configuración nueva
- `StorageMetadata:CacheTtlMinutes`
- `StorageMetadata:ValidatePhysicalSchema`

## Códigos de error funcional incorporados
- `GAB_FIELD_UNKNOWN`
- `GAB_REQUIRED_EMPTY`
- `GAB_FIELD_TYPE_INVALID`
- `GAB_FIELD_LENGTH_INVALID`
- `GAB_TYPE_UNSUPPORTED`
- `GAB_SCHEMA_MISMATCH`

## Evidencia de verificación
- `dotnet build DocuArchiCore.sln --no-restore` en verde.
- `StorageValidationPipelineTests` en verde (`9/9`).
