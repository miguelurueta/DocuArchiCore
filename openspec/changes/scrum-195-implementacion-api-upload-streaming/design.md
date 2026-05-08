## Context

- Jira issue key: SCRUM-195
- Jira summary: IMPLEMENTACION-API-UPLOAD-STREAMING
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-195

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / API PROMPT 24 (REV C) — Upload Temporal Enterprise para Archivos Grandes hasta 30GB (IIS + Streaming + Chunked Upload + Integración Storage Engine) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: Core Web API IIS / Windows Server streaming y carga chunked resiliencia de red y reintentos seguridad documental operación enterprise y observabilidad integración con Storage Engine DocuArchi sin regresiones ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar carga temporal para archivos de hasta 30GB sin romper el flujo actual: Upload temporal chunked seguro. AlmacenarDocumento consume RutaTemporalId + ArchivoTemporalId . El endpoint final de almacenamiento NO recibe binario ni URL externa. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DECISIÓN ARQUITECTÓNICA CLAVE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Para 30GB: NO usar upload monolítico. NO cargar archivo completo en memoria. NO usar IFormFile como mecanismo principal para carga grande. SÍ usar application/octet-stream + Request.Body por chunk. SÍ usar sesión de upload con estado persistente. SÍ usar ensamblaje final controlado + hash final. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ACLARACIÓN TÉCNICA IIS (OBLIGATORIA) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Aunque el objetivo funcional sea 30GB, IIS no debe recibir una sola request de 30GB. Regla: El archivo total (30GB) se soporta por múltiples chunks . Cada request de chunk debe estar dentro de límites seguros (ej. 10MB–50MB). No depender de maxAllowedContentLength=30GB para request única. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ FLUJO GENERAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Cliente externo: Init upload session. Enviar chunks ( PUT ) en paralelo o secuencial. Consultar estado/reintentar chunks fallidos. Completar upload (ensamble + validación hash/tamaño). Invocar POST /almacenamiento con referencias temporales. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENDPOINTS REQUERIDOS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 1) Iniciar upload POST /api/gestor-documental/almacenamiento/upload-temporal/init Request: {
  "nombreOriginal": "documento.pdf",
  "tamanoBytes": 32212254720,
  "extension": ".pdf",
  "hashSha256Esperado": "opcional",
  "numeroChunks": 3200
}

Response:

{
  "rutaTemporalId": "rt_xxxxx",
  "archivoTemporalId": "af_xxxxx.pdf",
  "chunkSizeBytes": 10485760,
  "estado": "IN_PROGRESS"
}

### 2) Subir chunk

PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}

Headers:

- Content-Length
- X-Chunk-Index
- X-Chunk-Size
- X-Total-Chunks
- X-Upload-Session-Id

Body:

- application/octet-stream

### 3) Estado

GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status

### 4) Completar

POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete

Validar:

- chunks completos
- tamaño final
- hash final
- extensión permitida
- ownership usuario

### 5) Cancelar

DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## INTEGRACIÓN CON ALMACENARDOCUMENTO

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

POST /api/gestor-documental/almacenamiento no cambia contrato principal.

Antes de almacenar validar:

- sesión existe
- pertenece al usuario
- estado = COMPLETED
- archivo final existe
- tamaño/hash válidos (si aplica)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CONFIGURACIÓN OBLIGATORIA

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Variables (IIS/env):

- StoragePaths__Temp
- StorageUpload__MaxFileSizeBytes = 32212254720
- StorageUpload__ChunkSizeBytes = 10485760 (ajustable)
- StorageUpload__AllowedExtensions = .pdf,.tif,.tiff,.jpg,.jpeg,.png,.bmp
- StorageUpload__TtlMinutes = 1440
- StorageUpload__MaxParallelChunksPerSession (recomendado)
- StorageUpload__MaxConcurrentSessionsPerUser (recomendado)
- StorageUpload__MaxDiskQuotaPerUserBytes (recomendado)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RUTAS MULTIUSUARIO

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Ruta base:
{StoragePaths:Temp}\usr_{usuarioId}\{rutaTemporalId}\

Estructura:

- chunks\00000001.part
- final\af_xxxxx.ext

Reglas:

- rutaTemporalId GUID/ULID
- archivoTemporalId GUID/ULID + extensión
- no usar nombre original en path físico
- bloqueo traversal
- todo bajo StoragePaths:Temp

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PERSISTENCIA DE SESIÓN (CRÍTICO)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar IStorageUploadSessionStore persistente:

- DB o Redis (preferido para multi-instancia).
- No depender solo de memoria local.

Guardar:

- estado
- chunks recibidos
- tamaño acumulado
- hash esperado/calculado
- usuario propietario
- timestamps/TTL

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ESTADOS DE UPLOAD

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- IN_PROGRESS
- COMPLETED
- FAILED
- CANCELLED
- EXPIRED

Solo COMPLETED puede pasar a AlmacenarDocumento.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ENSAMBLAJE E INTEGRIDAD

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

En complete:

1. validar chunks requeridos
2. ensamblar en orden
3. calcular SHA256 incremental
4. validar tamaño final
5. validar hash esperado (si viene)
6. marcar COMPLETED

Si falla:

- marcar FAILED
- limpiar resultado parcial si aplica

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SEGURIDAD

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Validar:

- tamaño por chunk y total
- extensión whitelist
- ownership usuario
- índices de chunk válidos
- no sobrescritura fuera de sesión

Agregar:

- rate limiting por IP/usuario
- control de cuotas por usuario/sesión
- rechazo explícito de rutas/URLs cliente

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CONCURRENCIA E IDEMPOTENCIA

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- lock por (rutaTemporalId, archivoTemporalId)
- reintento de chunk permitido (idempotente)
- no completar si faltan chunks
- tolerar reenvío del mismo chunk validando tamaño/hash

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## LIMPIEZA TTL

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

IStorageUploadCleanupService (BackgroundService):

- limpia IN_PROGRESS expirados
- limpia FAILED, CANCELLED, EXPIRED
- limpia huérfanos en disco
- registra auditoría de limpieza

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBSERVABILIDAD Y ERRORES

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Respuesta estándar:

- AppResponses<T>

Códigos:

- UPLOAD_FILE_TOO_LARGE
- UPLOAD_EXTENSION_NOT_ALLOWED
- UPLOAD_INVALID_CHUNK
- UPLOAD_HASH_MISMATCH
- UPLOAD_SESSION_NOT_FOUND
- UPLOAD_NOT_COMPLETED
- UPLOAD_ACCESS_DENIED
- UPLOAD_STORAGE_UNAVAILABLE

Logs:

- requestId, usuarioId, ip
- rutaTemporalId, archivoTemporalId
- chunkIndex, bytes
- estado, duración

No loguear binario/secretos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PRUEBAS

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Unitarias:

- init inválido
- chunk inválido
- hash mismatch
- ownership inválido
- complete con faltantes

Integración:

- upload real por chunks
- ensamblaje y hash real
- AlmacenarDocumento consume referencia COMPLETED

Carga:

- simulación multiusuario
- reintentos de red
- chunks en paralelo

Seguridad:

- traversal bloqueado
- extensión peligrosa bloqueada
- cuota/rate limit

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SWAGGER / DEPURACIÓN

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Swagger:

- solo pruebas funcionales con chunks pequeños.

Para 30GB reales:

- usar Postman/curl/cliente dedicado de carga chunked.

Local debug:

- configurar StoragePaths__Temp en launchSettings.json
- crear carpeta local con permisos
- ejecutar flujo: init -> chunk(s) -> status -> complete -> almacenar

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DOCUMENTACIÓN OBLIGATORIA

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actualizar:
Docs/GestorDocumental/AlmacenamientoDocumental/Arquitectura-Final/

Crear:

- SCRUM-189-Arquitectura-Upload-Temporal-30GB.md
- SCRUM-189-Implementacion-Upload-Temporal-30GB.md
- SCRUM-189-Pruebas-Upload-Temporal-30GB.md
- SCRUM-189-Operacion-IIS-Upload-Temporal.md
- SCRUM-189-Metadata.md

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CRITERIOS DE ACEPTACIÓN

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✔ Soporte 30GB por chunked upload
✔ Streaming sin carga completa en memoria
✔ Estado persistente de sesión
✔ Integridad por hash/tamaño final
✔ Integración intacta con AlmacenarDocumento
✔ TTL/cleanup operativo
✔ Pruebas en verde
✔ Sin regresiones en Storage Engine actual

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## INSTRUCCIÓN FINAL

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar upload temporal enterprise de gran tamaño (hasta 30GB) basado en streaming/chunks, con estado persistente, seguridad
operacional y compatibilidad total con el flujo actual de almacenamiento por referencias temporales.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-195-implementacion-api-upload-streaming.