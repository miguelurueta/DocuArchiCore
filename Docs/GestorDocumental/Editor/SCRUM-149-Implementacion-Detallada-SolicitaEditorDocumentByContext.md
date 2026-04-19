# SCRUM-149 — Implementación Detallada: SolicitaEditorDocumentByContext (Catálogo)

## Archivos (por capa)

- API
  - `DocuArchi.Api/Controllers/GestorDocumental/Editor/SolicitaEditorDocumentByContextController.cs`
  - `DocuArchi.Api/Program.cs` (DI)
- Services
  - `MiApp.Services/Service/GestorDocumental/Editor/ServiceSolicitaEditorDocumentByContext.cs`
- Repository
  - `MiApp.Repository/Repositorio/GestorDocumental/Editor/SolicitaEditorContextDefinitionRepository.cs` (reutilizado)
  - `MiApp.Repository/Repositorio/GestorDocumental/Editor/SolicitaEditorDocumentByContextRepository.cs`
- DTO
  - `MiApp.DTOs/DTOs/GestorDocumental/Editor/EditorDocumentDetailByContextResponseDto.cs`

## Endpoint

`GET /api/gestor-documental/editor/document/by-context?contextCode=XXX&entityId=NNN`

## Reglas implementadas

### Validación de entrada (Controller)

- Claim `defaulalias` requerido (string)
- `contextCode` requerido
- `entityId > 0`

### Normalización (Service)

- `contextCode = contextCode.Trim().ToUpperInvariant()`

### Catálogo (Service)

- `ContextCode` debe existir y estar activo (`is_active = 1`)
- obtiene `context_definition_id`, `entity_name`, `relation_type`

### Resolución de relación activa (Repository)

- Busca en `ra_editor_document_context` con:
  - `context_definition_id = @contextDefinitionId`
  - `entity_id = @entityId`
  - `is_active = 1`
  - `ORDER BY created_at DESC, context_id DESC LIMIT 1`

### Consulta de documento + imágenes (Repository)

- Documento: `ra_editor_documents` por `document_id`
- Imágenes: join `ra_editor_document_image_links` + `ra_editor_document_images` filtrando `deleted_at IS NULL`

### Enriquecimiento de contexto (Service)

Completa `data.Context` con:

- `ContextCode`
- `EntityName`
- `RelationType`
- `ContextDefinitionId`
- `ContextId` (desde relación activa)
- `EntityId`

## Semántica de respuesta

- `ContextCode` inválido/inactivo → `success=false`
- Contexto no encontrado → `success=true`, `message="Sin resultados"`, `data=null`
- Contexto encontrado + documento encontrado → `success=true`, `data!=null`
- Contexto encontrado + documento inexistente → `success=false` (inconsistencia)

