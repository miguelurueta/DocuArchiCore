# SCRUM-195 - Arquitectura Upload Temporal Streaming

## Objetivo arquitectónico
Desacoplar la recepción de binario del endpoint de almacenamiento final mediante una fase previa de carga temporal por chunks.

## Principio aplicado
- Endpoint final `POST /almacenamiento` conserva contrato por referencia (`RutaTemporalId`, `ArchivoTemporalId`).
- Carga binaria se mueve a endpoints especializados `upload-temporal/*`.

## Componentes
- `IStorageLargeUploadService` / `StorageLargeUploadService`
  - Orquesta init/chunk/status/complete/cancel.
- `IStorageUploadSessionStore` / `StorageUploadSessionStore`
  - Persistencia de metadata de sesión en disco (`session.json`).
- `IStorageUploadPathResolver` / `StorageUploadPathResolver`
  - Resolución segura de rutas bajo raíz temporal configurable.
- `IStorageUploadCleanupService` / `StorageUploadCleanupService`
  - Limpieza TTL de sesiones vencidas.

## Flujo resumido
1. Cliente inicia sesión de upload.
2. Cliente envía chunks `application/octet-stream`.
3. Servicio registra chunks recibidos y tamaño acumulado.
4. Cliente solicita `complete`; el servicio ensambla y valida hash/tamaño.
5. Cliente invoca `POST /almacenamiento` con referencias.
6. UseCase valida estado `COMPLETED` y continúa con Storage Engine.

## Decisiones
- Se mantiene compatibilidad con flujo actual del orchestrator.
- Se reutiliza resolución de archivo temporal por `rutaTemporalId + archivoTemporalId`.
- Se incorpora `StoragePaths:Temp` para despliegues configurables (IIS/entornos productivos).

## Riesgos abiertos
- Faltan pruebas unitarias/integración dedicadas a `upload-temporal`.
- Política separada de rate-limit y quotas por usuario aún pendiente (task 5.5).
- Limpieza TTL está implementada, pero requiere operación programada para ejecución periódica.
