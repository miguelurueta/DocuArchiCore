# SCRUM-151 — Implementación Detallada: Initial Content Editor

## Archivos

- API
  - `DocuArchi.Api/Controllers/GestorDocumental/Editor/InitialContentEditorController.cs`
  - `DocuArchi.Api/Program.cs` (DI)
- Service
  - `MiApp.Services/Service/GestorDocumental/Editor/ServiceInitialContentEditor.cs`
- DTO
  - `MiApp.DTOs/DTOs/GestorDocumental/Editor/EditorInitialContentResponseDto.cs`

## Contrato

`GET /api/gestor-documental/editor/initial-content?idTareaWf={id}&contextCode={code}&entityId={entityId}`

## Selección de plantilla (MVP)

- `TemplateCode = ContextCode` (normalizado)
- Versión vigente:
  - filtra `IsActive = true`
  - prioriza `IsPublished = true`
  - mayor `VersionNumber`

## Tokens

Formato de token reconocido:

- `{{TOKEN}}` (alfanumérico y `_`)

Resolución:

- Se construye un diccionario desde la primera fila retornada por `SolicitaEstructuraRespuestaIdTarea`.
- Si falta algún token requerido por la plantilla, se retorna error `Validation` con la lista.

## No alcance

- No persiste documento.
- No resuelve reglas avanzadas contexto→plantilla (se deja para ticket de reglas).

