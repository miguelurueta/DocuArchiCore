# SCRUM-154 — Integración Frontend: ResolveEditorDocument

## Endpoint

`GET /api/gestor-documental/editor/document/resolve`

## Objetivo

Resolver en una sola llamada si el editor debe cargar:

- `mode=existing`: documento ya guardado (carga secundaria)
- `mode=initial`: contenido inicial (plantilla + tokens)

## Query Params

- `contextCode` (string, requerido)
- `entityId` (long, requerido)
- `idTareaWf` (long, opcional)
  - Requerido **solo** cuando el resultado sea `mode=initial` (porque la carga inicial consulta estructura/tokens).
- `templateDefinitionId` (long, opcional) override de plantilla
- `templateCode` (string, opcional) override de plantilla
- `prefer` (string, opcional) valores: `existing|initial` (default `existing`)

## Respuesta

Wrapper: `AppResponses<EditorResolveDocumentResponseDto?>`

`EditorResolveDocumentResponseDto`:
- `mode`: `existing|initial`
- `contextCode`
- `entityId`
- `documentId` (nullable)
- `templateDefinitionId`
- `templateCode`
- `html`
- `images`
- `tokensResueltos` (solo en `mode=initial`)

## Flujo recomendado en UI

1) Llamar siempre a `/document/resolve` al abrir el editor.
2) Si `data.mode == "existing"`:
   - Cargar el editor con `data.html`
   - Renderizar imágenes desde `data.images`
3) Si `data.mode == "initial"`:
   - Cargar el editor con `data.html`
   - Usar `data.tokensResueltos` como fuente de hidratación inicial / variables visibles
4) Si responde `409 Conflict`:
   - Mostrar mensaje de conflicto.
   - Caso típico: existen múltiples documentos para el mismo `contextCode + entityId` sin criterio determinístico.

## Ejemplos

### 1) Resolver y cargar existente (default)

`GET /api/gestor-documental/editor/document/resolve?contextCode=RAD_GESTION_RESPUESTA&entityId=911`

### 2) Forzar carga inicial (crear nuevo) cuando el contexto lo permite

`GET /api/gestor-documental/editor/document/resolve?contextCode=RAD_GESTION_RESPUESTA&entityId=911&prefer=initial&idTareaWf=123`

### 3) Forzar una plantilla específica

`GET /api/gestor-documental/editor/document/resolve?contextCode=RAD_GESTION_RESPUESTA&entityId=911&idTareaWf=123&templateDefinitionId=1`

