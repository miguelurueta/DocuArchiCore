# SCRUM-144 — Integración Frontend: `SolicitaEditorDocumentById`

## Objetivo

Consultar un documento previamente guardado (HTML + metadatos) y su lista de imágenes asociadas activas para re-edición en Tiptap.

## Endpoint

- **Método**: `GET`
- **Ruta**: `api/gestor-documental/editor/document/{documentId}`
- **Parámetro**: `documentId` (route, `long`)
- **Auth**: Requiere `[Authorize]`
- **Claim requerido**: `defaulalias` (para resolver `defaultDbAlias`)

## Respuesta

`AppResponses<EditorDocumentDetailResponseDto>`

- `success`: `true` si encontró el documento
- `message`: `"OK"` en éxito
- `data`: objeto con el documento y `Images`
- `errors`: lista de errores (vacía en éxito)

### `data` (resumen)

- `DocumentId`
- `TemplateId`, `TemplateVersion`, `DocumentTitle`
- `FormatCode`, `DocumentHtml`, `StatusCode`
- `CreatedBy`, `UpdatedBy`, `CreatedAt`, `UpdatedAt`
- `Images[]`: lista de imágenes asociadas (sin bytes)

## Casos

- **Documento existe con imágenes**: retorna `Images` con `ImageId` + `ImageUid` (y metadatos opcionales).
- **Documento existe sin imágenes**: retorna `Images = []`.
- **Documento no existe**: retorna `success = false` con error `NotFound` en `errors`.

## Ejemplos (pseudo)

Request:

`GET /api/gestor-documental/editor/document/123`

Response OK:

`{ success: true, message: "OK", data: { DocumentId: 123, DocumentHtml: "<p>...</p>", Images: [{ ImageId: 10, ImageUid: "..." }] }, errors: [] }`

