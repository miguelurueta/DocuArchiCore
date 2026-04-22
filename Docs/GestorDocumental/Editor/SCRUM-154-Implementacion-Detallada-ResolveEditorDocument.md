# SCRUM-154 — Implementación Detallada: Resolve Editor Document

## Resumen

Se implementó un endpoint único `GET /api/gestor-documental/editor/document/resolve` que resuelve:

- documento existente (`Mode=existing`) si ya existe para `contextCode + entityId`, o
- HTML inicial (`Mode=initial`) generado por el caso de uso de contenido inicial.

## Archivos principales

- Controller: `..\DocuArchi.Api\Controllers\GestorDocumental\Editor\ResolveEditorDocumentController.cs`
- Service: `..\MiApp.Services\Service\GestorDocumental\Editor\ServiceResolveEditorDocument.cs`
- DTO: `..\MiApp.DTOs\DTOs\GestorDocumental\Editor\EditorResolveDocumentResponseDto.cs`
- Registro DI: `..\DocuArchi.Api\Program.cs` (registro de `IServiceResolveEditorDocument`)

## Flujo en Controller

1. Valida claim `defaulalias` usando `IClaimValidationService`.
2. Valida parámetros de request:
   - `contextCode` requerido (no vacío),
   - `entityId > 0`,
   - `prefer` si viene, debe ser `existing|initial`.
3. Llama al service:
   - `_service.ResolveAsync(idTareaWf, contextCode, entityId, templateDefinitionId, templateCode, prefer, defaulalias)`
4. Mapea respuesta:
   - `success=true` → `200 OK`
   - `success=false` + error `Conflict` → `409 Conflict`
   - `success=false` (otros) → `400 BadRequest`

## Flujo en Service

1. Validaciones defensivas:
   - `defaultDbAlias` requerido
   - `contextCode` requerido
   - `entityId > 0`
   - `prefer` ∈ {`existing`, `initial`}
2. Normaliza `contextCode` a `Trim().ToUpperInvariant()`.
3. Valida el contexto en catálogo:
   - `_contextCatalog.SolicitaPorContextCodeAsync(contextCode, alias)`
4. Obtiene conteo de documentos activos por contexto:
   - `_docByContextRepo.CountActiveByContextAsync(contextDefinitionId, entityId, alias)`
5. Maneja casos:
   - `count > 1` → `Conflict` (no se decide “último activo”).
   - `count == 1 && prefer=existing` → delega a `IServiceSolicitaEditorDocumentByContext` y retorna `Mode=existing`.
   - `count == 1 && prefer=initial`:
     - si contexto no permite múltiples → `Conflict`
     - si permite múltiples → continúa con `initial`
   - `count == 0` → continúa con `initial`
6. En `initial`:
   - exige `idTareaWf > 0`
   - llama a `IServiceInitialContentEditor.GetInitialContentAsync(...)`
   - construye `EditorResolveDocumentResponseDto` con:
     - `Mode=initial`
     - `Html = htmlInicial`
     - `TokensResueltos` del caso de uso de initial content
     - `Images = []`

## Contrato de salida (DTO)

`EditorResolveDocumentResponseDto` expone:

- `Mode`: `existing|initial`
- `ContextCode`, `EntityId`, `DocumentId`
- `TemplateDefinitionId`, `TemplateCode`
- `Html`, `Images`, `TokensResueltos`

## Notas de compatibilidad

- `idTareaWf` es opcional en el endpoint, pero funcionalmente obligatorio si la resolución termina en `Mode=initial`.

