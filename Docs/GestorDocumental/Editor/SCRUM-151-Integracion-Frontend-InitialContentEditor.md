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

Ejemplo real:

`GET /api/gestor-documental/editor/initial-content?idTareaWf=123&contextCode=RAD_GESTION_RESPUESTA&entityId=911`

## Respuesta

Wrapper: `AppResponses<EditorInitialContentResponseDto?>`

Campos relevantes:

- `htmlInicial`: HTML listo para inicializar Tiptap (sin persistir documento todavía)
- `templateCode` / `templateVersion`: información para trazabilidad y para posteriores operaciones (Full Save)
- `tokensResueltos`: mapa de tokens reemplazados en el HTML

