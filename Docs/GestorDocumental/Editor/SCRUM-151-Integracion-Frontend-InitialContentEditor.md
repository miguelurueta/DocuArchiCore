# SCRUM-151 — Integración Frontend: Initial Content Editor

## Endpoint

- Método: `GET`
- Ruta: `/api/gestor-documental/editor/initial-content`
- Claim requerido: `defaulalias` (string)

## Parámetros (querystring)

- `idTareaWf` (long, requerido > 0)
  - Identificador de la tarea del workflow desde la cual se solicita la estructura de negocio de la respuesta.
- `contextCode` (string, requerido)
  - Código de contexto del editor (catálogo `ra_editor_context_definitions`). Define el tipo de contenido/relación.
- `entityId` (long, requerido > 0)
  - Identificador de la entidad de negocio objetivo (ej: id_Radicado).
- `templateDefinitionId` (long, opcional > 0)
  - **Override explícito** de la plantilla a usar por `template_definition_id` (más preciso que `templateCode`).
  - Si viene informado, el backend usa esa plantilla (si existe y está activa).
  - Si no se envía, el backend resuelve la plantilla por configuración (reglas de contexto) o por el fallback vigente del servicio.
- `templateCode` (string, opcional)
  - Override explícito de la plantilla a usar por `template_code`.
  - Se recomienda usar `templateDefinitionId` cuando sea posible.

## Ejemplos

### 1) Flujo normal (recomendado): `contextCode` estable

El frontend **siempre** debe enviar el `contextCode` del catálogo (sin sufijos de versión) y el `entityId`.

`GET /api/gestor-documental/editor/initial-content?idTareaWf=123&contextCode=RAD_GESTION_RESPUESTA&entityId=911`

En este modo, la plantilla se selecciona por configuración:
- `ra_editor_context_definitions` (valida que el contexto exista y esté activo)
- `ra_editor_template_context_rules` (elige la plantilla activa por `priority_order`)
- `ra_editor_template_versions` (elige la versión vigente: activa/publicada + mayor versión)

### 2) Override de plantilla por ID (más preciso)

Útil para pruebas, QA o escenarios donde quieres forzar una plantilla específica (por ejemplo, `RAD_GESTION_RESPUESTA_V1`)
sin cambiar reglas en base de datos.

`GET /api/gestor-documental/editor/initial-content?idTareaWf=123&contextCode=RAD_GESTION_RESPUESTA&entityId=911&templateDefinitionId=1`

Regla: si `templateDefinitionId` viene informado, el backend **lo prioriza** y no necesita que `templateCode == contextCode`.

### 3) Override de plantilla por código (alternativa)

`GET /api/gestor-documental/editor/initial-content?idTareaWf=123&contextCode=RAD_GESTION_RESPUESTA&entityId=911&templateCode=RAD_GESTION_RESPUESTA_V1`

## Nota importante (evitar errores de configuración)

- `contextCode` **NO** es el código de la plantilla.
- `contextCode` sale del catálogo `ra_editor_context_definitions`.
- La plantilla (y su versión) sale de `ra_editor_template_definitions` / `ra_editor_template_versions`.
- Si usas plantillas versionadas (`..._V1`, `..._V2`), mantén el `contextCode` estable (sin versión) y selecciona la plantilla por:
  - `ra_editor_template_context_rules` (recomendado), o
  - `templateDefinitionId`/`templateCode` (override).

## Respuesta

Wrapper: `AppResponses<EditorInitialContentResponseDto?>`

Campos relevantes:

- `htmlInicial`: HTML listo para inicializar Tiptap (sin persistir documento todavía)
- `templateCode` / `templateVersion`: información para trazabilidad y para posteriores operaciones (Full Save)
- `tokensResueltos`: mapa de tokens reemplazados en el HTML
