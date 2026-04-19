# SCRUM-150 — Integración Frontend: Catálogo de Plantillas (TemplateDefinitions)

## Base

- Claim requerido: `defaulalias` (string)
- Base path: `/api/gestor-documental/editor/templates`

## Crear definición

- `POST /definitions`

Body:

```json
{
  "templateCode": "RAD_GESTION_RESPUESTA",
  "templateName": "Respuesta radicado",
  "moduleName": "radicacion",
  "description": "Plantilla base para respuesta",
  "createdBy": "usuario"
}
```

## Crear versión

- `POST /versions`

Body:

```json
{
  "templateCode": "RAD_GESTION_RESPUESTA",
  "versionNumber": 1,
  "templateHtml": "<p>...</p>",
  "isPublished": true,
  "createdBy": "usuario"
}
```

## Consultar por código (definición + versiones)

- `GET /definitions/{templateCode}`

Ejemplo:

`GET /api/gestor-documental/editor/templates/definitions/RAD_GESTION_RESPUESTA`

