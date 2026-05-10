## Context

- Jira issue key: `SCRUM-195`
- Jira summary: `IMPLEMENTACION-API-UPLOAD-STREAMING`
- Change: `scrum-195-implementacion-api-upload-streaming`

Referencias:
- `openspec/context/multi-repo-context.md`
- `openspec/context/OPSXJ_BACKEND_RULES.md`
- `Docs/Publicacion/*` (IIS deployment and env vars)

## Problem

El endpoint actual de almacenamiento recibe JSON con referencias temporales y no puede recibir binario directo en forma robusta para archivos grandes.  
Integraciones externas requieren subir documentos de alto tamaño (hasta 30GB) sin agotar memoria ni romper la paridad funcional del motor de almacenamiento.

## Architectural Decision

Implementar flujo de **2 pasos**:

1. **Upload temporal chunked** (streaming a disco por chunk).
2. **Persistencia documental final** usando el endpoint actual con `RutaTemporalId + ArchivoTemporalId`.

### Constraints

- No usar request monolítica de 30GB.
- No usar `IFormFile` como mecanismo principal para archivo completo.
- No aceptar URL externa en endpoint final.
- No cambiar semántica del Storage Engine final.

## High-Level Flow

1. Cliente inicia sesión de upload (`init`).
2. Cliente envía chunks (`PUT` octet-stream).
3. API persiste chunks y actualiza estado.
4. Cliente consulta estado (`status`) y reintenta chunks fallidos.
5. Cliente completa upload (`complete`): ensamble, hash, tamaño, estado `COMPLETED`.
6. Cliente llama `POST /almacenamiento` con referencias temporales.
7. API de almacenamiento valida sesión/ownership/estado y ejecuta flujo existente.

## API Surface

### Upload

- `POST /api/gestor-documental/almacenamiento/upload-temporal/init`
- `PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- `GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
- `POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- `DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`

### Storage final

- Se mantiene `POST /api/gestor-documental/almacenamiento` sin binario.

## Data and Session Model

### Upload session metadata

Campos mínimos:
- `RutaTemporalId`
- `ArchivoTemporalId`
- `UsuarioId`
- `TamanoBytesEsperado`
- `TamanoBytesRecibido`
- `NumeroChunks`
- `ChunksRecibidos` / bitmap equivalente
- `HashSha256Esperado` (opcional)
- `HashSha256Calculado`
- `Estado` (`IN_PROGRESS|COMPLETED|FAILED|CANCELLED|EXPIRED`)
- `CreatedAt`, `CompletedAt`, `ExpiresAt`

### Persistence choice

Requerido estado persistente (no solo memoria).  
Opciones permitidas:
- Tabla MySQL dedicada.
- Redis con TTL + snapshot/metadata en DB.

## File System Strategy

Ruta base:
- `StoragePaths:Temp` (desde `StoragePaths__Temp` en IIS/env).

Estructura:
- `{Temp}\usr_{usuarioId}\{rutaTemporalId}\chunks\{chunkIndex}.part`
- `{Temp}\usr_{usuarioId}\{rutaTemporalId}\final\{archivoTemporalId}`

Reglas:
- `rutaTemporalId` y `archivoTemporalId` generados por servidor (GUID/ULID).
- No usar nombre original en ruta física.
- Bloquear traversal (`GetFullPath` + root guard).

## IIS and Runtime Considerations

- Soporte 30GB se logra por **múltiples requests chunk**.
- Cada request debe respetar límite seguro de chunk.
- Configuración obligatoria:
  - `StoragePaths__Temp`
  - `StorageUpload__MaxFileSizeBytes`
  - `StorageUpload__ChunkSizeBytes`
  - `StorageUpload__AllowedExtensions`
  - `StorageUpload__TtlMinutes`
  - límites de concurrencia/cuota por usuario

## Security

Validaciones:
- tamaño total y por chunk
- extensión permitida
- ownership usuario
- índice de chunk dentro de rango
- coherencia de headers de chunk
- hash final (si se envía esperado)
- estado permitido para transición

Prohibiciones:
- URL externa como fuente de archivo
- rutas provistas por cliente
- escritura fuera de `StoragePaths:Temp`

## Concurrency and Idempotency

- Lock por `(rutaTemporalId, archivoTemporalId)` en `complete/cancel`.
- Reintento del mismo chunk permitido e idempotente.
- `complete` rechaza si faltan chunks.
- Paralelismo de chunks controlado por límites configurables.

## Observability

Logs estructurados:
- `requestId`, `usuarioId`, `rutaTemporalId`, `archivoTemporalId`
- `chunkIndex`, `bytes`, `estado`, `duracionMs`

Métricas:
- `uploads_iniciados`, `uploads_completados`, `uploads_fallidos`
- `chunks_recibidos`, `bytes_recibidos`, `uploads_expirados`

## Cleanup

`IStorageUploadCleanupService` (job programado):
- limpia sesiones `IN_PROGRESS` expiradas
- limpia `FAILED/CANCELLED/EXPIRED`
- limpia chunks huérfanos
- registra auditoría operativa

## Integration with Existing Storage Engine

Antes de ejecutar `AlmacenarDocumento`:
- verificar que referencia temporal existe
- verificar ownership
- verificar estado `COMPLETED`
- verificar archivo final presente y consistente

No se modifica el pipeline central de persistencia final fuera de esa validación de precondición.

## Repository Scope

Implementación requerida:
- `DocuArchi.Api`
- `MiApp.Services`
- `MiApp.Repository`
- `MiApp.DTOs`
- `MiApp.Models`
- `DocuArchiCore` (OpenSpec + docs)

No alcance:
- `DocuArchiCore.Web`
- `DocuArchiCore.Abstractions` (salvo necesidad explícita detectada durante implementación)

