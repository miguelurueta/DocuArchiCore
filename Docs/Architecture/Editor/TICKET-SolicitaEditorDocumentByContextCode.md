# Ticket — Obtener documento por catálogo de contexto (`ContextCode` + `EntityId`)

## Objetivo
Recuperar documento del editor sin depender de `DocumentId`, usando una referencia estable:
- `ContextCode` (catálogo)
- `EntityId` (negocio)

Debe retornar:
- documento (`ra_editor_documents`)
- imágenes activas (sin `image_bytes`, excluye `DeletedAt`)
- contexto asociado (incluye datos del catálogo)

## Contrato
Endpoint:
- `GET /api/gestor-documental/editor/document/by-context?contextCode=RAD_GESTION_RESPUESTA&entityId=911`

Response:
- `AppResponses<EditorDocumentDetailResponse?>`

## Componentes esperados
- Controller: `SolicitaEditorDocumentByContextController`
- Service: `ServiceSolicitaEditorDocumentByContext`
- Repo catálogo: `SolicitaEditorContextDefinitionRepository`
- Repo principal: `SolicitaEditorDocumentByContextRepository`

## Reglas clave (validadas)
- Validar claim `defaulalias`.
- Normalizar `ContextCode` (Trim + Upper, o regla definida).
- Catálogo:
  - `ContextCode` inexistente/inactivo ⇒ error (`success=false`).
- Contexto activo:
  - buscar en `ra_editor_document_context` por (`context_definition_id`, `entity_id`, `is_active=1`).
  - Si no existe → `success=true`, `"Sin resultados"`, `data=null`.
  - Si hay múltiples activos → comportamiento definido:
    - recomendado: tomar el más reciente `ORDER BY created_at DESC, context_id DESC LIMIT 1`.
- Imágenes:
  - join links + images,
  - excluir `deleted_at`,
  - no exponer `image_bytes`.
- Inconsistencia:
  - contexto apunta a documento inexistente ⇒ error controlado.

