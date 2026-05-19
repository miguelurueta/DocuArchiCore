# SCRUM-206 Integracion Frontend Permisos Visor PDF

## 1. Objetivo
Consumir permisos del visor PDF por implementacion y aplicar comportamiento UI por usuario autenticado.

## 2. Seguridad
- Header obligatorio: `Authorization: Bearer {jwt}`.
- Claims minimos en JWT:
  - `defaulalias`
  - `usuarioid`

## 3. API recomendada para frontend
Consumo principal por implementacion y usuario autenticado:
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`

Uso administrativo:
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
- `PUT .../overrides`
- `DELETE .../overrides/{codigoPermiso}`

## 4. Ejemplo de consumo (workflow)
Request:
```http
GET /api/gestor-documental/permisos-visorpdf/implementaciones/workflow/mis-permisos
Authorization: Bearer eyJ...
```

Response:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "codigoImplementacion": "workflow",
    "idUsuario": 141,
    "permissions": {
      "pdf.view": true,
      "pdf.print": false,
      "pdf.download": true,
      "pdf.annotate.open_signature_modal": true,
      "pdf.annotate.signature.draw": false,
      "pdf.annotate.signature.upload": true,
      "pdf.annotate.signature.personal": false,
      "pdf.annotate.signature.place": true,
      "pdf.annotate.signature.delete": false,
      "pdf.annotate.signature.lock": true,
      "pdf.annotate.signature.unlock": true,
      "pdf.rotate": true,
      "pdf.zoom": true
    }
  },
  "errors": []
}
```

## 5. Matriz FE de permisos

| Permiso | Accion UI | Comportamiento FE |
|---|---|---|
| `pdf.view` | abrir visor | si `false`, no renderizar documento |
| `pdf.print` | boton Print | ocultar o deshabilitar `onPrint` |
| `pdf.download` | boton Export | ocultar o deshabilitar `onExport` |
| `pdf.annotate.open_signature_modal` | boton Signature | bloquear apertura modal |
| `pdf.annotate.signature.draw` | tab Dibujar firma | ocultar tab |
| `pdf.annotate.signature.upload` | tab Subir firma | ocultar tab |
| `pdf.annotate.signature.personal` | tab Firma personal | ocultar tab y no llamar API firma temporal |
| `pdf.annotate.signature.place` | Usar firma | deshabilitar placement |
| `pdf.annotate.signature.delete` | eliminar firma | deshabilitar |
| `pdf.annotate.signature.lock` | candado bloquear | deshabilitar lock |
| `pdf.annotate.signature.unlock` | candado desbloquear | deshabilitar unlock |
| `pdf.rotate` | rotar | deshabilitar |
| `pdf.zoom` | zoom in/out/reset | deshabilitar |

## 6. Pseudocodigo de aplicacion FE
```ts
const perms = response.data.permissions;

if (!perms["pdf.view"]) {
  return <NoAutorizado />;
}

<PdfViewer
  canPrint={!!perms["pdf.print"]}
  canDownload={!!perms["pdf.download"]}
  canRotate={!!perms["pdf.rotate"]}
  canZoom={!!perms["pdf.zoom"]}
  canOpenSignatureModal={!!perms["pdf.annotate.open_signature_modal"]}
  canDrawSignature={!!perms["pdf.annotate.signature.draw"]}
  canUploadSignature={!!perms["pdf.annotate.signature.upload"]}
  canUsePersonalSignature={!!perms["pdf.annotate.signature.personal"]}
  canPlaceSignature={!!perms["pdf.annotate.signature.place"]}
  canDeleteSignature={!!perms["pdf.annotate.signature.delete"]}
  canLockSignature={!!perms["pdf.annotate.signature.lock"]}
  canUnlockSignature={!!perms["pdf.annotate.signature.unlock"]}
/>
```

## 7. Checklist de integracion
1. Identificar `codigoImpl` por modulo (`workflow`, `gestion_correspondencia`, etc.).
2. Consultar `mis-permisos` al cargar el visor.
3. Guardar permisos en estado local/global del modulo.
4. Evaluar UI por permiso antes de renderizar acciones.
5. No hardcodear reglas; usar solo el mapa `permissions`.

## 8. Notas de contrato
- `codiperfil` no se envia desde frontend.
- El backend resuelve perfil activo internamente.

