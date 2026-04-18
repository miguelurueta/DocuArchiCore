# Ticket — Guardar contexto de documento usando catálogo (`ContextCode`)

## Objetivo
Persistir la relación documento ↔ negocio usando catálogo formal `ra_editor_context_definitions`, evitando strings libres en el request (`entity_name`, `relation_type`).

## Contrato
Endpoint técnico (misma API):
- `POST /api/gestor-documental/editor/document/context`

Request:
- `DocumentId`
- `ContextCode`
- `EntityId`
- `CreatedBy?`

Response:
- `AppResponses<RaEditorDocumentContext?>`

## Componentes esperados
- Controller: `GuardaEditorDocumentContextController`
- Service: `ServiceGuardaEditorDocumentContext`
- Repo catálogo: `SolicitaEditorContextDefinitionRepository`
- Repo contexto: `GuardaEditorDocumentContextRepository`

## Reglas clave (validadas)
- Validar claim `defaulalias`.
- Normalizar `ContextCode` (Trim + Upper, o regla definida por el proyecto).
- Catálogo:
  - `ContextCode` inexistente/inactivo ⇒ error (`success=false`, `data=null`).
- Persistencia:
  - Validar existencia de `document_id` en `ra_editor_documents`.
  - Idempotencia: no duplicar relación activa equivalente:
    - (`document_id`, `context_definition_id`, `entity_id`, `is_active=1`) ⇒ retornar existente.
- Reglas del catálogo se aplican en Service (no en repos):
  - `requires_unique_entity` (mínimo obligatorio).
  - `allow_multiple_documents`, `allow_multiple_contexts_per_document` (si no se implementan completas, documentar deuda técnica).
- Soportar `conn/trans` externas (para Full Save).

## Índices recomendados
- Unicidad/idempotencia activa sobre la clave funcional.
- Índice de consulta por (`context_definition_id`, `entity_id`, `is_active`).

