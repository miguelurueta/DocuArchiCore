# SCRUM-148 — Implementación Detallada: Full Save Editor Document (Catálogo)

## Archivos modificados

- `DocuArchi.Api/Controllers/GestorDocumental/Editor/FullSaveEditorDocumentController.cs`
- `MiApp.Services/Service/GestorDocumental/Editor/ServiceFullSaveEditorDocument.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Editor/FullSaveEditorDocumentRequestDto.cs`

## Flujo transaccional

1. Abrir conexión por `defaulalias` y comenzar transacción.
2. Guardar/actualizar documento con `IGuardaEditorDocumentRepository` → obtener `DocumentId`.
3. Resolver definición activa por `ContextCode` con `ISolicitaEditorContextDefinitionRepository`.
4. Validar reglas mínimas del catálogo (ej: `requires_unique_entity`).
5. Guardar relación documento↔contexto con `IGuardaEditorDocumentContextRepository` (idempotente).
6. Sincronizar imágenes con `ISincronizaEditorDocumentImagesRepository`.
7. `COMMIT` si todo OK, `ROLLBACK` si falla cualquier paso.

## Normalización aplicada

- `ContextCode`: `Trim().ToUpperInvariant()`
- `ImageUids`: `Trim`, remover vacíos, `Distinct` case-insensitive

