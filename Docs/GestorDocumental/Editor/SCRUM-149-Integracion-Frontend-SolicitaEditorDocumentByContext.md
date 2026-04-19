# SCRUM-149 — Integración Frontend: SolicitaEditorDocumentByContext (Catálogo)

## Endpoint

- Método: `GET`
- Ruta: `/api/gestor-documental/editor/document/by-context`
- Claim requerido: `defaulalias` (string)

## Querystring

- `contextCode` (string, requerido)
- `entityId` (long, requerido > 0)

Ejemplo:

`GET /api/gestor-documental/editor/document/by-context?contextCode=RAD_GESTION_RESPUESTA&entityId=911`

## Respuesta

Wrapper: `AppResponses<EditorDocumentDetailByContextResponseDto?>`

### Caso 1 — encontrado

- `success = true`
- `message = "OK" | "YES"`
- `data != null`
- `data.images` solo imágenes activas (sin `deleted_at`)

### Caso 2 — sin resultados

- `success = true`
- `message = "Sin resultados"`
- `data = null`

### Caso 3 — error controlado (validación / catálogo inválido)

- `success = false`
- `data = null`
- `errors` con `Validation`

## Notas

- `ContextCode` se normaliza en backend: `Trim().ToUpperInvariant()`.
- Este endpoint es el recomendado cuando el documento pertenece a un proceso funcional y el frontend no debe depender de recordar `DocumentId`.

