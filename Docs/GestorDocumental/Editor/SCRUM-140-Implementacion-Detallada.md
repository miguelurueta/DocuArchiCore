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

