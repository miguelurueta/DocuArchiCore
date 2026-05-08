## 1. Scope and architecture baseline

- [ ] 1.1 Confirmar alcance final: upload temporal chunked hasta 30GB + integración por referencias con `AlmacenarDocumento`.
- [ ] 1.2 Confirmar no alcance: sin binario en endpoint final, sin URL externa, sin reemplazo del Storage Engine.
- [ ] 1.3 Confirmar repos de implementación: `DocuArchi.Api`, `MiApp.Services`, `MiApp.Repository`, `MiApp.DTOs`, `MiApp.Models`, `DocuArchiCore`.

## 2. OpenSpec artifacts

- [x] 2.1 Refinar `proposal.md` con objetivo técnico y out-of-scope.
- [x] 2.2 Refinar `design.md` con endpoints, sesión persistente, seguridad, concurrencia y cleanup.
- [x] 2.3 Refinar `specs/jira-scrum-195/spec.md` con requisitos y escenarios testables.
- [x] 2.4 Mantener `sync.md` consistente con impacto real por repo.

## 3. DTOs and models

- [x] 3.1 Crear DTOs request/response para `init/status/complete/cancel`.
- [x] 3.2 Crear modelos de dominio para sesión temporal (`StorageTemporaryUploadMetadata` y estado).
- [x] 3.3 Definir enums/constantes de estado (`IN_PROGRESS`, `COMPLETED`, `FAILED`, `CANCELLED`, `EXPIRED`).

## 4. Repository and persistence

- [x] 4.1 Implementar `IStorageUploadSessionStore` persistente (DB/Redis según decisión aprobada).
- [x] 4.2 Implementar operaciones: create session, upsert chunk status, get status, complete, cancel, expire.
- [x] 4.3 Implementar control de ownership por `usuarioId` y validaciones de transición de estado.

## 5. Services

- [x] 5.1 Implementar `IStorageLargeUploadService` (init/chunk/status/complete/cancel).
- [x] 5.2 Implementar escritura streaming de chunk (`Request.Body`) sin cargar archivo completo.
- [x] 5.3 Implementar ensamble final de chunks en orden y hash SHA256 incremental.
- [x] 5.4 Implementar `IStorageUploadPathResolver` con root guard bajo `StoragePaths:Temp`.
- [ ] 5.5 Implementar `IStorageUploadPolicy` (size/extension/chunk/quota/rate constraints).

## 6. API controllers and DI

- [x] 6.1 Agregar endpoints `upload-temporal/*` en controller de almacenamiento o controller dedicado.
- [x] 6.2 Mantener `AppResponses<T>` + manejo de errores consistente.
- [x] 6.3 Integrar validación previa en `POST /almacenamiento` para exigir sesión `COMPLETED`.
- [x] 6.4 Registrar interfaces y servicios en `Program.cs`.

## 7. Operations and cleanup

- [x] 7.1 Implementar `IStorageUploadCleanupService` con TTL configurable.
- [x] 7.2 Agregar configuración requerida (`StoragePaths__Temp`, `StorageUpload__*`) y validación fail-fast.
- [ ] 7.3 Documentar guía IIS + permisos de AppPool para carpeta temporal.

## 8. Tests and verification

- [ ] 8.1 Unit tests: init invalid, chunk invalid, ownership invalid, complete con faltantes/hash mismatch.
- [ ] 8.2 Integration tests: flujo init->chunk->status->complete->almacenamiento con referencia válida.
- [ ] 8.3 Pruebas de concurrencia/idempotencia de reintento de chunks.
- [x] 8.4 Documentar evidencia de pruebas ejecutadas y resultados.

## 9. Documentation and closure

- [ ] 9.1 Actualizar documentación técnica en `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`.
- [ ] 9.2 Actualizar `Docs/GestorDocumental/AlmacenamientoDocumental/Arquitectura-Final/` con diseño y operación upload 30GB.
- [x] 9.3 Ejecutar `openspec.cmd validate scrum-195-implementacion-api-upload-streaming`.
- [ ] 9.4 Continuar flujo `opsxj:orchestrate:publish` y `opsxj:orchestrate:archive`.
