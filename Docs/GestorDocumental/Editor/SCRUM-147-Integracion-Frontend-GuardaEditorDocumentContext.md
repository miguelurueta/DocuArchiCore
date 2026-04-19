# SCRUM-147 — Integración Frontend: GuardaEditorDocumentContext (Catálogo)

## Endpoint

- Método: `POST`
- Ruta: `/api/gestor-documental/editor/document/context`
- Claim requerido: `defaulalias` (string) — alias de conexión

## Objetivo

Registrar (idempotente) la asociación entre un documento del editor (`DocumentId`) y un contexto de negocio, usando un catálogo controlado por `ContextCode` (sin `entity_name` / `relation_type` libres en el request).

## Request (body)

```json
{
  "documentId": 150,
  "contextCode": "RAD_GESTION_RESPUESTA",
  "entityId": 911,
  "createdBy": "usuario"
}
```

Reglas de validación:

- `documentId > 0`
- `contextCode` requerido (no vacío / no whitespace)
- `entityId > 0`
- el `ContextCode` debe existir y estar activo en `ra_editor_context_definitions`
- el documento debe existir en `ra_editor_documents`

## Respuesta

Wrapper: `AppResponses<RaEditorDocumentContext?>`

### Caso OK (creado o ya existente)

- `success = true`
- `data != null`
- `data` contiene la fila activa en `ra_editor_document_context`

### Caso error controlado

- `success = false`
- `data = null`
- `errors` incluye `Validation` o `Error` según corresponda

## Idempotencia (comportamiento esperado)

Si ya existe una relación activa equivalente (mismo `document_id + context_definition_id + entity_id`), el endpoint debe devolver la existente y no crear duplicados.

## Recomendación de uso con Full Save

En el flujo final, el Full Save (orquestador) debe invocar este caso de uso dentro de la misma transacción para garantizar atomicidad documento + contexto + imágenes.

