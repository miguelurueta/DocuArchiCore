# SCRUM-140 — Integración Frontend — Guardar documento (Editor)

## API

- Método: `POST`
- Ruta: `/api/gestor-documental/editor/guardar-documento`
- Claim requerido: `defaulalias`
- Body (JSON): `GuardaEditorDocumentRequestDto`

## Body

Campos:

- `documentId` (opcional): `null`/omitido para crear, `> 0` para actualizar
- `templateId` (opcional)
- `templateVersion` (opcional)
- `documentTitle` (opcional)
- `documentHtml` (requerido)
- `statusCode` (opcional, default `saved`)
- `createdBy` / `updatedBy` (opcional)

## Response

`AppResponses<RaEditorDocument?>`

- `success = true` y `data` con documento (create/update)
- `success = false` y `data = null` para errores/validaciones

## UX recomendaciones

- Mostrar guardado exitoso con timestamp (si aplica) y permitir “reintentar” en error.
- En validación (HTML vacío), bloquear botón de guardar y mostrar mensaje.

