# SCRUM-145 — Implementación Detallada — LimpiezaEditorImages

## Archivos

- `DocuArchi.Api/Controllers/GestorDocumental/Editor/LimpiezaEditorImagesController.cs`
- `MiApp.Services/Service/GestorDocumental/Editor/ServiceLimpiezaEditorImages.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/Editor/LimpiezaEditorImagesRepository.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Editor/LimpiezaEditorImagesRequestDto.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Editor/LimpiezaEditorImagesResponseDto.cs`
- Registro DI: `DocuArchi.Api/Program.cs`

## SQL (criterio huérfana)

- Imagen candidata: `ra_editor_document_images.DeletedAt IS NULL`
- Huérfana: `LEFT JOIN ra_editor_document_image_links` con `l.ImageId IS NULL`
- Antigüedad: `CreatedAt IS NULL OR CreatedAt <= cutoff`

## Manejo de errores

- Respuestas estándar `AppResponses<T>`
- `try/catch` en Service y Repository
