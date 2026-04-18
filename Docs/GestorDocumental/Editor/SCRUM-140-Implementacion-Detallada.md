# SCRUM-140 — Implementación Detallada — Guardar documento (Editor)

## Archivos

- `DocuArchi.Api/Controllers/GestorDocumental/Editor/GuardaEditorDocumentController.cs`
- `MiApp.Services/Service/GestorDocumental/Editor/ServiceGuardaEditorDocument.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/Editor/GuardaEditorDocumentRepository.cs`
- `MiApp.Models/Models/GestorDocumental/Editor/RaEditorDocument.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Editor/GuardaEditorDocumentRequestDto.cs`

## Reglas

- `documentHtml` requerido (no vacío)
- `formatCode` fijo: `html`
- `statusCode` default: `saved`
- Create cuando `documentId` es null/<=0, update cuando `documentId > 0`

## Uso como repositorio base dentro del Full Save

- El repositorio `GuardaEditorDocumentRepository` es **puro** (solo `ra_editor_documents`) y no orquesta flujo completo.
- El orquestador `Full Save` abre `conn/trans` y llama a este repositorio pasando la **misma** transacción para mantener atomicidad con:
  - contexto de negocio (catálogo + `context_definition_id`)
  - sincronización de imágenes
- En update, el repositorio debe validar existencia del `DocumentId` antes de reportar éxito y no debe modificar `CreatedAt`.

