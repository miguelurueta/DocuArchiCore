# SCRUM-195 - Metadata

## Identificación
- Ticket: `SCRUM-195`
- Tema: `Upload temporal streaming/chunked para Storage Engine`
- Estado documental: `Implementación técnica registrada`

## Documento técnico principal
- `SCRUM-195-Implementacion-Upload-Temporal-Streaming.md`

## Documento de arquitectura asociado
- `../Arquitectura-Final/SCRUM-195-Diagramas-Upload-Temporal-StorageEngine.md`

## Repositorios impactados
- `MiApp.DTOs`
- `MiApp.Models`
- `MiApp.Services`
- `DocuArchi.Api`
- `DocuArchiCore` (OpenSpec + pruebas)

## Componentes clave
- `IStorageLargeUploadService` / `StorageLargeUploadService`
- `IStorageUploadPolicy` / `StorageUploadPolicy`
- `IStorageUploadSessionStore` / `StorageUploadSessionStore`
- `IStorageUploadPathResolver` / `StorageUploadPathResolver`
- `IStorageUploadCleanupService` / `StorageUploadCleanupService`
- `AlmacenarDocumentoUseCase` (validación de `COMPLETED`)

## Evidencia de verificación
- OpenSpec válido para cambio `scrum-195-implementacion-api-upload-streaming`.
- Pruebas focalizadas del flujo upload/usecase/controller ejecutadas en verde.

## Riesgos operativos controlados
- Limpieza TTL obligatoria en ruta temporal.
- Configuración de permisos de carpeta temporal en IIS/AppPool.
