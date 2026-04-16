# SCRUM-142 — Integración Frontend (Tiptap)

## Endpoint

- `POST /api/gestor-documental/editor/document/images/sync`

## Request

Content-Type: `application/json`

```json
{
  "DocumentId": 123,
  "ImageUids": ["<image_uid_1>", "<image_uid_2>"]
}
```

Notas:
- `ImageUids` puede ser una lista vacía para limpiar relaciones del documento.
- `ImageUids` NO puede ser `null`.

## Response

`AppResponses<bool>`

- OK: `success=true`, `data=true`
- Error validación: `success=false`, `errors[]`

