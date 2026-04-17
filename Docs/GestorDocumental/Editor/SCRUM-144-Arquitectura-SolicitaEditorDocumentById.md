# SCRUM-144 — Arquitectura: `SolicitaEditorDocumentById`

## Flujo

Controller → Service → Repository → DB

## Componentes

- Controller: `DocuArchi.Api/Controllers/GestorDocumental/Editor/SolicitaEditorDocumentByIdController.cs`
- Service: `MiApp.Services/Service/GestorDocumental/Editor/ServiceSolicitaEditorDocumentById.cs`
- Repository: `MiApp.Repository/Repositorio/GestorDocumental/Editor/SolicitaEditorDocumentByIdRepository.cs`

## Decisiones

- Operación **read-only** (no modifica datos).
- Consulta 1: documento (`ra_editor_documents`).
- Consulta 2: imágenes asociadas activas (join `ra_editor_document_image_links` + `ra_editor_document_images` con `DeletedAt IS NULL`).
- DTO de respuesta no incluye `ImageBytes`.

