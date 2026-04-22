# SCRUM-154 — Integración Frontend: Resolve Editor Document

## Endpoint

- Método: `GET`
- Ruta: `/api/gestor-documental/editor/document/resolve`
- Claim requerido: `defaulalias` (string)

## Objetivo

Exponer un único endpoint para que el frontend obtenga el HTML del editor en 2 modos:

- `existing`: devuelve el documento existente por contexto + entidad.
- `initial`: genera y devuelve el HTML inicial (plantilla hidratada con tokens) cuando no existe documento, o cuando se fuerza `prefer=initial` y el contexto lo permite.

## Parámetros (querystring)

- `contextCode` (string, requerido)
  - Código de contexto del editor (`ra_editor_context_definitions`).
- `entityId` (long, requerido > 0)
  - Identificador de la entidad de negocio objetivo (ej: id_Radicado).
- `idTareaWf` (long, opcional)
  - Requerido **solo** cuando el resultado deba ser `Mode=initial` (ya sea porque no existe documento o porque se fuerza `prefer=initial`).
  - Fuente: tarea workflow actual (pantalla/flujo desde el que se solicita el editor).
- `templateDefinitionId` (long, opcional)
  - Override explícito de plantilla (más preciso que `templateCode`). Se aplica en el flujo `initial`.
- `templateCode` (string, opcional)
  - Override de plantilla por código. Se aplica en el flujo `initial`.
- `prefer` (string, opcional)
  - Valores permitidos: `existing` | `initial`.
  - Default: `existing`.

## Reglas de comportamiento (alto nivel)

- Si `prefer` es inválido → `400 BadRequest` (Validation).
- Si existe más de un documento activo para (contexto, entidad) y no hay criterio determinístico → `409 Conflict`.
- Si el resultado es `Mode=initial` y `idTareaWf <= 0` → `400 BadRequest` (Validation).

## Ejemplos

### 1) Default recomendado: traer documento si existe; si no, crear initial

`GET /api/gestor-documental/editor/document/resolve?contextCode=RAD_GESTION_RESPUESTA&entityId=911&idTareaWf=123`

Notas:
- El backend intentará `existing` primero.
- Si no existe documento: retorna `Mode=initial` con `Html` listo para cargar en Tiptap.

### 2) Forzar `initial` (crear nuevo) cuando el contexto lo permite

`GET /api/gestor-documental/editor/document/resolve?contextCode=RAD_GESTION_RESPUESTA&entityId=911&idTareaWf=123&prefer=initial`

Notas:
- Si ya existe un documento y el contexto **no** permite múltiples → retorna `409 Conflict`.
- Si permite múltiples → retorna `Mode=initial`.

### 3) Override de plantilla en modo `initial`

`GET /api/gestor-documental/editor/document/resolve?contextCode=RAD_GESTION_RESPUESTA&entityId=911&idTareaWf=123&templateDefinitionId=1`

## Respuesta

Wrapper: `AppResponses<EditorResolveDocumentResponseDto?>`

Campos relevantes de `data`:

- `mode`: `existing` | `initial`
- `contextCode`, `entityId`
- `documentId`:
  - `null` en `initial`
  - valor en `existing`
- `templateDefinitionId`, `templateCode`
- `html`: HTML listo para inicializar el editor (Tiptap)
- `images`: lista de imágenes asociadas (en `initial` se retorna `[]`)
- `tokensResueltos`:
  - `null` en `existing`
  - diccionario en `initial`

## Manejo de errores recomendado en frontend

- `400 BadRequest`: mostrar error de validación (parámetros/claim/contexto).
- `409 Conflict`: mostrar mensaje de conflicto (ej: múltiples documentos). Recomendación UX: ofrecer acción al usuario (seleccionar documento) si la UI lo soporta.
