# SCRUM-141 — Implementación Detallada — Guardar imagen (Tiptap)

## Archivos

- `DocuArchi.Api/Controllers/GestorDocumental/Editor/GuardaEditorImageController.cs`
- `MiApp.Services/Service/GestorDocumental/Editor/ServiceGuardaEditorImage.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/Editor/GuardaEditorImageRepository.cs`
- `MiApp.Models/Models/GestorDocumental/Editor/RaEditorDocumentImage.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Editor/GuardaEditorImageResponseDto.cs`

## Controller

- Recibe `IFormFile file` por `multipart/form-data`.
- Valida claim + archivo (length > 0).
- Convierte a `byte[]` y delega a service.

## Service

- Valida alias, bytes, tamaño máximo, content-type permitido.
- Genera `image_uid` (GUID) y construye entidad para repository.

## Repository

- Inserta en `ra_editor_document_images` y consulta por `image_id` para retornar entidad.

