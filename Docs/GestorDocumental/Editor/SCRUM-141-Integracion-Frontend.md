# SCRUM-141 — Integración Frontend — Guardar imagen (Tiptap)

## API

- Método: `POST`
- Ruta: `/api/gestor-documental/editor/guardar-imagen`
- Content-Type: `multipart/form-data`
- Form field: `file`
- Claim requerido: `defaulalias`

## Response

`AppResponses<GuardaEditorImageResponseDto?>`

Campos importantes:

- `data.imageId`
- `data.imageUid`
- `data.publicUrl` (puede ser `null` si storage es DB sin endpoint público)

## Ejemplo (frontend)

- Enviar `FormData` con `file`.
- En éxito, insertar la URL retornada o construir una ruta con `imageUid` (si se define endpoint público).

