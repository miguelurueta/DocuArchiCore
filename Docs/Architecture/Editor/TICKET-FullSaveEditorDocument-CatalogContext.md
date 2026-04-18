# Ticket — Full Save atómico con catálogo de contexto (`ContextCode`)

## Objetivo
Actualizar Full Save para orquestar de forma atómica:
1) documento,
2) resolución de catálogo (`ContextCode` → `context_definition_id`),
3) persistencia de contexto,
4) sincronización de imágenes,
todo bajo una única transacción.

## Contrato
Endpoint:
- `POST /api/gestor-documental/editor/document/full-save`

Request (resumen):
- Documento (HTML + metadatos)
- `ContextCode`
- `EntityId`
- `ImageUids[]`

Response:
- `AppResponses<RaEditorDocument?>`

## Repos reutilizados (sin repo nuevo para Full Save)
- `IGuardaEditorDocumentRepository`
- `ISolicitaEditorContextDefinitionRepository`
- `IGuardaEditorDocumentContextRepository`
- `ISincronizaEditorDocumentImagesRepository`

## Reglas clave (validadas)
- Controller no maneja transacciones ni catálogo.
- Service Full Save:
  - abre una sola `conn/trans`,
  - no ejecuta SQL directo,
  - pasa `conn/trans` a repos,
  - normaliza `ContextCode` y `ImageUids`,
  - `ROLLBACK` ante cualquier `success=false` o excepción.
- Catálogo: `ContextCode` inexistente/inactivo ⇒ error + rollback.
- Atomicidad: no dejar estado parcial (documento/contexto/imágenes deben persistir juntos o ninguno).

## Pruebas mínimas
- Unit tests del orquestador: éxito y rollback en cada falla (documento, catálogo, contexto, imágenes).
- Integración (o Skip explícito si no Docker): validar persistencia/rollback real.

