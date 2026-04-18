# SCRUM-145 — LimpiezaEditorImages (validado)

## Propósito
Eliminación lógica y limpieza de imágenes huérfanas del editor Tiptap (sin borrado físico en esta fase), incluyendo endpoint técnico, dry-run y job.

## Componentes (patrón del repo)
- Controller: `DocuArchi.Api/Controllers/GestorDocumental/Editor/LimpiezaEditorImagesController.cs`
- Service: `MiApp.Services/Service/GestorDocumental/Editor/ServiceLimpiezaEditorImages.cs`
- Repository: `MiApp.Repository/Repositorio/GestorDocumental/Editor/LimpiezaEditorImagesRepository.cs`
- Job: `MiApp.Services/Jobs/GestorDocumental/Editor/EditorImageCleanupJob.cs`

## Reglas clave
- Validar claim `defaulalias`.
- Identificar huérfanas como imágenes sin link activo en `ra_editor_document_image_links`.
- Filtrar por antigüedad (`OlderThanMinutes`) y soportar `DryRun`.
- Marcar `DeletedAt` (borrado lógico). No borrar bytes/objetos de storage en esta fase.

## Notas
- Este ticket fue verificado y alineado con el contrato/rutas del módulo Editor y la convención de `AppResponses<T>`.

