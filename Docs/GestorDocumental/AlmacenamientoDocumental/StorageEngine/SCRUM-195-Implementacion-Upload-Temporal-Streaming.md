# SCRUM-195 - Implementación Upload Temporal Streaming

## Resumen
Se implementó un flujo de carga temporal por chunks para consumo del Storage Engine sin enviar binario al endpoint final de almacenamiento.

## Cambios por repositorio
- `MiApp.DTOs`
  - `DTOs/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/StorageUploadInitRequestDto.cs`
  - `DTOs/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/StorageUploadInitResponseDto.cs`
  - `DTOs/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/StorageUploadStatusResponseDto.cs`
- `MiApp.Models`
  - `Models/GestorDocumental/AlmacenamientoDocumental/StoragePathOptions.cs`
  - `Models/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/*`
- `MiApp.Services`
  - `Service/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/*`
  - `Service/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoUseCase.cs`
  - `Service/GestorDocumental/AlmacenamientoDocumental/Physical/StoragePathResolver.cs`
- `DocuArchi.Api`
  - `Controllers/GestorDocumental/AlmacenamientoDocumental/AlmacenamientoDocumentalController.cs`
  - `Program.cs`
  - `appsettings.json`
  - `appsettings.Development.json`

## Endpoints agregados
- `POST /api/gestor-documental/almacenamiento/upload-temporal/init`
- `PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- `GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
- `POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- `DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`

## Integración con almacenamiento final
`AlmacenarDocumentoUseCase` valida que cada `ArchivoTemporalId` de `RutaTemporalId` esté en estado `COMPLETED` antes de orquestar persistencia.

## Seguridad aplicada
- Ownership por `usuarioId` (claim) para todas las operaciones de sesión.
- Validación anti path traversal en resolver de rutas temporales.
- Validación de extensión permitida, tamaño total, índice de chunk y consistencia de total chunks.

## Streaming y ensamblado
- Escritura de chunk por `Request.Body` a disco (sin cargar archivo completo en memoria).
- Ensamblado secuencial de chunks a archivo final temporal.
- Hash SHA256 incremental y validación opcional contra hash esperado.

## Configuración
- `StoragePaths:Temp`
- `StorageUpload:MaxFileSizeBytes`
- `StorageUpload:ChunkSizeBytes`
- `StorageUpload:AllowedExtensions`
- `StorageUpload:TtlMinutes`

## Evidencia de compilación
Comando ejecutado:
- `dotnet build D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -nologo`

Resultado:
- Compilación exitosa, sin errores de compilación.
