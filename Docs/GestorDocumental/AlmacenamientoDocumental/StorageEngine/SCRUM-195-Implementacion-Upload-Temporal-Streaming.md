# SCRUM-195 - Implementación Técnica Upload Temporal Streaming

## 1. Objetivo técnico
Habilitar carga temporal de documentos por streaming/chunks para desacoplar la transferencia binaria del endpoint final de almacenamiento documental.

Resultado esperado:
- El binario se carga primero a un repositorio temporal controlado.
- `POST /almacenamiento` solo recibe referencias (`RutaTemporalId`, `ArchivoTemporalId`).
- El Storage Engine valida que cada referencia esté en estado `COMPLETED` antes de persistir en flujo legacy-equivalente.

## 2. Alcance implementado
- Endpoints `upload-temporal/*` en API.
- Servicios de dominio de upload temporal en `MiApp.Services`.
- Modelos/DTOs de sesión temporal en `MiApp.Models` y `MiApp.DTOs`.
- Validación obligatoria previa en `AlmacenarDocumentoUseCase`.
- Configuración por `appsettings`/variables de entorno para rutas, límites y políticas.
- Cleanup de sesiones temporales con TTL.

Fuera de alcance:
- Reemplazo del Storage Engine final.
- Recepción de URL externa para archivo fuente.
- Carga binaria directa en `POST /almacenamiento`.

## 3. Componentes implementados
### 3.1 API (`DocuArchi.Api`)
- `AlmacenamientoDocumentalController`
  - `POST /upload-temporal/init`
  - `PUT /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
  - `GET /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
  - `POST /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
  - `DELETE /upload-temporal/{rutaTemporalId}/{archivoTemporalId}`
- `Program.cs`
  - Registro DI de servicios upload temporal.
  - Bind de `StoragePaths` y `StorageUpload`.

### 3.2 Servicios (`MiApp.Services`)
- `IStorageLargeUploadService` / `StorageLargeUploadService`
  - Orquesta init, recepción de chunks, estado, completion y cancelación.
- `IStorageUploadPathResolver` / `StorageUploadPathResolver`
  - Resuelve rutas físicas seguras bajo root temporal.
- `IStorageUploadSessionStore` / `StorageUploadSessionStore`
  - Persiste metadata de sesión temporal.
- `IStorageUploadCleanupService` / `StorageUploadCleanupService`
  - Limpieza de sesiones vencidas.
- `IStorageUploadPolicy` / `StorageUploadPolicy`
  - Política explícita de validaciones (size/extension/chunk/hash/estado).

### 3.3 UseCase (`MiApp.Services`)
- `AlmacenarDocumentoUseCase`
  - Invoca `EnsureCompletedAsync(...)` para todos los `ArchivoTemporalId` del request.
  - Si no está `COMPLETED` o no existe archivo físico temporal, retorna error de validación.

### 3.4 Modelos/DTOs
- DTOs request/response para init/status.
- Modelos de sesión temporal, estado, opciones de ruta y opciones de upload.

## 4. Contrato y flujo de integración
1. Cliente inicia sesión temporal (`init`) con metadata de archivo.
2. Cliente envía chunks binarios (`chunk`) por `Request.Body`.
3. API permite consulta de avance (`status`).
4. Cliente cierra carga (`complete`), ensamblando chunks en orden y validando hash/tamaño.
5. Cliente invoca `POST /almacenamiento` con referencias temporales.
6. `AlmacenarDocumentoUseCase` valida estado `COMPLETED` y existencia física del temporal.
7. Si la validación pasa, inicia el orquestador normal del Storage Engine.

## 4.1 Contrato API para frontend
### Endpoint 1: Inicializar upload temporal
`POST /api/gestor-documental/almacenamiento/upload-temporal/init`

Request:
```json
{
  "nombreOriginal": "contrato-2026.pdf",
  "tamanoBytes": 20971520,
  "extension": ".pdf",
  "hashSha256Esperado": "9e107d9d372bb6826bd81d3542a419d6b2e0f2c2c89f9320f7d0fb4c79d3f6b1",
  "numeroChunks": 2
}
```

Response `200`:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "rutaTemporalId": "usr_25_9e7b2c3d0f4e4f6db2732d7d5f9b381f",
    "archivoTemporalId": "af_f3a89df6eac24e6cb734fcf2d45ec6a2.pdf",
    "chunkSizeBytes": 10485760,
    "estado": "IN_PROGRESS"
  }
}
```

### Endpoint 2: Subir chunk
`PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`

Headers mínimos:
- `Content-Length`
- `X-Total-Chunks`

Body:
- `application/octet-stream` (bytes del chunk)

Response `200`:
```json
{
  "success": true,
  "message": "OK"
}
```

### Endpoint 3: Consultar estado
`GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`

Response `200`:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "estado": "IN_PROGRESS",
    "chunksRecibidos": [0],
    "chunksPendientes": [1],
    "tamanoRecibidoBytes": 10485760
  }
}
```

### Endpoint 4: Completar upload
`POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`

Response `200`:
```json
{
  "success": true,
  "message": "OK"
}
```

### Endpoint 5: Cancelar upload
`DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`

Response `200`:
```json
{
  "success": true,
  "message": "OK"
}
```

### Endpoint final: Almacenar documento con referencias temporales
`POST /api/gestor-documental/almacenamiento`

Request mínimo:
```json
{
  "nombreGabinete": "GAB_CONTRATOS",
  "rutaTemporalId": "usr_25_9e7b2c3d0f4e4f6db2732d7d5f9b381f",
  "nombreDocumento": "contrato-2026.pdf",
  "requestId": "rq-front-001",
  "documentos": [
    {
      "idDocumento": "DOC-1",
      "archivoTemporalId": "af_f3a89df6eac24e6cb734fcf2d45ec6a2.pdf",
      "nombreOriginal": "contrato-2026.pdf",
      "extension": ".pdf",
      "numeroPaginas": 12
    }
  ],
  "camposIndexacion": []
}
```

Response `200`:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "idAlmacen": 105234,
    "idRegistroProduccionDocumental": 77881,
    "nombreArchivoFinal": "DIG000105234.pdf",
    "requestId": "rq-front-001"
  }
}
```

Errores esperados del contrato final:
- `UPLOAD_NOT_COMPLETED`: archivo temporal no completado o inexistente.
- `DOC_TEMP_ID_REQUIRED`: request sin `archivoTemporalId`.
- `UPLOAD_ACCESS_DENIED`: sesión temporal pertenece a otro usuario.

## 4.2 Ejemplo completo frontend (paso a paso)
1. Llamar `init` y guardar `rutaTemporalId`, `archivoTemporalId`, `chunkSizeBytes`.
2. Partir el archivo local en chunks del tamaño recomendado.
3. Subir cada chunk con `PUT .../chunk/{index}` en orden o paralelo controlado.
4. Consultar `status` para reintentos de chunks faltantes.
5. Ejecutar `complete`.
6. Invocar `POST /almacenamiento` con las referencias temporales.
7. Persistir `idAlmacen` y `requestId` en frontend para trazabilidad.

## 5. Reglas de seguridad y consistencia
- Ownership estricto por `usuarioId` para todas las operaciones de sesión.
- Validación de extensión permitida (`AllowedExtensions`).
- Validación de límites:
  - tamaño total esperado,
  - tamaño máximo de chunk,
  - índice de chunk y cantidad total de chunks.
- Guard de rutas para evitar path traversal.
- Ensamblado con hash SHA-256 incremental.
- Estados de sesión controlados:
  - `IN_PROGRESS`, `COMPLETED`, `FAILED`, `CANCELLED`, `EXPIRED`.

## 6. Códigos de error funcional relevantes
- `UPLOAD_FILE_TOO_LARGE`
- `UPLOAD_EXTENSION_NOT_ALLOWED`
- `UPLOAD_INVALID_CHUNK`
- `UPLOAD_HASH_MISMATCH`
- `UPLOAD_SESSION_NOT_FOUND`
- `UPLOAD_ACCESS_DENIED`
- `UPLOAD_NOT_COMPLETED`
- `DOC_TEMP_ID_REQUIRED`

## 7. Configuración operativa requerida
```json
{
  "StoragePaths": {
    "Temp": "C:\\AppData\\DocuArchiApi\\temp"
  },
  "StorageUpload": {
    "MaxFileSizeBytes": 32212254720,
    "ChunkSizeBytes": 10485760,
    "AllowedExtensions": [".pdf", ".tif", ".tiff", ".jpg", ".jpeg", ".png", ".bmp"],
    "TtlMinutes": 1440
  }
}
```

Variables equivalentes por ambiente:
- `StoragePaths__Temp`
- `StorageUpload__MaxFileSizeBytes`
- `StorageUpload__ChunkSizeBytes`
- `StorageUpload__AllowedExtensions__{n}`
- `StorageUpload__TtlMinutes`

## 8. Evidencia técnica de verificación
Comandos ejecutados:
- `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~StorageLargeUploadServiceTests|FullyQualifiedName~AlmacenarDocumentoUseCaseTests|FullyQualifiedName~AlmacenamientoDocumentalControllerTests" -nologo`
- `openspec.cmd validate scrum-195-implementacion-api-upload-streaming`

Resultado:
- Pruebas focalizadas: **17/17 exitosas**.
- Validación OpenSpec: **válida**.

## 9. Archivos principales tocados
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/*`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoUseCase.cs`
- `DocuArchi.Api/Controllers/GestorDocumental/AlmacenamientoDocumental/AlmacenamientoDocumentalController.cs`
- `DocuArchi.Api/Program.cs`
- `DocuArchi.Api/appsettings.json`
- `DocuArchi.Api/appsettings.Development.json`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/*`
- `MiApp.DTOs/DTOs/GestorDocumental/AlmacenamientoDocumental/TemporaryUpload/*`

## 10. Riesgos y recomendaciones
- Riesgo de almacenamiento temporal saturado si no se ejecuta cleanup TTL.
- Riesgo de configuración incompleta en entornos IIS (path inexistente/permisos AppPool).
- Recomendado:
  - Monitorear volumen de `StoragePaths:Temp`.
  - Ejecutar job periódico de limpieza.
  - Mantener `ChunkSizeBytes` conservador según infraestructura.
