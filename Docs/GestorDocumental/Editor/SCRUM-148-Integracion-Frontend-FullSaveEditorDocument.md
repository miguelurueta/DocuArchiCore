# SCRUM-148 — Integración Frontend: Full Save Editor Document (Catálogo)

## Endpoint

- Método: `POST`
- Ruta: `/api/gestor-documental/editor/document/full-save`
- Claim requerido: `defaulalias` (string)

## Objetivo

Ejecutar un guardado completo atómico que persiste:

- documento HTML (`ra_editor_documents`)
- relación documento ↔ contexto usando catálogo (`ra_editor_context_definitions` → `ra_editor_document_context`)
- sincronización de imágenes (`ra_editor_document_image_links`)

## Request (body)

```json
{
  "documentId": null,
  "templateId": 1,
  "templateVersion": 1,
  "documentTitle": "Respuesta",
  "documentHtml": "<p>...</p>",
  "statusCode": "saved",
  "createdBy": "usuario",
  "updatedBy": "usuario",
  "contextCode": "RAD_GESTION_RESPUESTA",
  "entityId": 911,
  "imageUids": ["img-uid-1", "img-uid-2"]
}
```

## Respuesta

Wrapper: `AppResponses<RaEditorDocument?>`

- `success=true` → guardado completo OK (documento persistido)
- `success=false` → fallo y rollback (sin persistencia parcial)

## Notas

- `ContextCode` debe existir y estar activo en el catálogo.
- `imageUids` puede ir vacío (en ese caso, se limpian relaciones previas según la lógica del sincronizador).

