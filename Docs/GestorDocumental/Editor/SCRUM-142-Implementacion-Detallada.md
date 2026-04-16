# SCRUM-142 — Implementación Detallada

## Archivos principales

- `..\DocuArchi.Api\Controllers\GestorDocumental\Editor\SincronizaEditorDocumentImagesController.cs`
- `..\MiApp.Services\Service\GestorDocumental\Editor\ServiceSincronizaEditorDocumentImages.cs`
- `..\MiApp.Repository\Repositorio\GestorDocumental\Editor\SincronizaEditorDocumentImagesRepository.cs`
- `..\MiApp.DTOs\DTOs\GestorDocumental\Editor\SincronizaEditorDocumentImagesRequestDto.cs`
- `..\MiApp.Models\Models\GestorDocumental\Editor\RaEditorDocumentImageLink.cs`

## SQL aplicado por el repositorio

- Resolver `ImageId` desde `ra_editor_document_images` filtrando `DeletedAt IS NULL`.
- `DELETE` de links obsoletos por `DocumentId`.
- `INSERT` de links faltantes en `ra_editor_document_image_links`.

