# SCRUM-154 — Implementación Detallada: ResolveEditorDocument

## Archivos y cambios

### DocuArchi.Api
- `Controllers/GestorDocumental/Editor/ResolveEditorDocumentController.cs`
  - Agrega `GET document/resolve`
  - Valida claim `defaulalias`
  - Valida `contextCode`, `entityId`, `prefer`
  - Mapea `Conflict` → HTTP 409
- `Program.cs`
  - Registra `IServiceResolveEditorDocument`

### MiApp.Services
- `Service/GestorDocumental/Editor/ServiceResolveEditorDocument.cs`
  - Normaliza `contextCode`
  - Valida `prefer` y alias
  - Resuelve contexto activo por catálogo
  - Cuenta documentos activos por contexto (para 409 múltiples)
  - Orquesta:
    - existente: `IServiceSolicitaEditorDocumentByContext`
    - inicial: `IServiceInitialContentEditor`

### MiApp.Repository
- `Repositorio/GestorDocumental/Editor/SolicitaEditorDocumentByContextRepository.cs`
  - Nuevo método `CountActiveByContextAsync(contextDefinitionId, entityId, alias)`
  - Consulta: `COUNT(*)` sobre `ra_editor_document_context` con `is_active=1`

### MiApp.DTOs
- `DTOs/GestorDocumental/Editor/EditorResolveDocumentResponseDto.cs`

### Tests (DocuArchiCore)
- `tests/TramiteDiasVencimiento.Tests/ResolveEditorDocumentControllerTests.cs`

## Comportamiento detallado

1) Normaliza `contextCode = Trim().ToUpperInvariant()`.
2) Obtiene definición activa de contexto (si falla → 400).
3) Cuenta documentos activos (si `>1` → 409).
4) Si `count==1`:
   - `prefer=existing` → retorna documento guardado.
   - `prefer=initial` → valida si el contexto permite múltiples; si no permite → 409.
5) Si se necesita inicial:
   - exige `idTareaWf > 0` (porque initial-content lo requiere) → 400 si falta.
   - retorna plantilla + tokens resueltos.

