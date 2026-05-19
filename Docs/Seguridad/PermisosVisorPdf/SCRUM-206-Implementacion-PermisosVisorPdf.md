# SCRUM-206 Implementacion Tecnica Permisos Visor PDF

## 1. Resumen
Se implementa una API backend para exponer permisos efectivos del visor PDF por implementacion y usuario, con soporte de overrides, perfil y default.

## 2. Rutas y artefactos objetivo

### 2.1 Controller
- `DocuArchi.Api/Controllers/GestorDocumental/PermisosVisorPdf/PermisosVisorPdfController.cs`

### 2.2 Service
- `MiApp.Services/Service/GestorDocumental/PermisosVisorPdf/IPermisosVisorPdfService.cs`

### 2.3 Repository
- `MiApp.Repository/Repositorio/GestorDocumental/PermisosVisorPdf/IPermisosVisorPdfRepository.cs`

### 2.4 DTOs
- `MiApp.DTOs/DTOs/GestorDocumental/PermisosVisorPdf/VisorPdfPermissionsResponseDto.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/PermisosVisorPdf/UpsertUserOverridesRequestDto.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/PermisosVisorPdf/PermissionOverrideItemDto.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/PermisosVisorPdf/SimpleOperationResultDto.cs`

## 3. Contratos detallados

### 3.1 GET mis permisos
`GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`

Reglas:
- Usuario origen: claim `usuarioid`.
- No requiere body.
- `codigoImpl` debe cumplir `^[A-Za-z0-9_]+$`.

Response success:
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
    },
    "sources": {
      "pdf.view": "default_implementacion",
      "pdf.print": "usuario_override",
      "pdf.download": "perfil_activo"
    },
    "generatedAt": "2026-05-19T18:45:22Z"
  },
  "errors": []
}
```

### 3.2 GET permisos de usuario objetivo (admin)
`GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`

Reglas:
- Requiere autorizacion administrativa.
- `idUsuario` debe ser entero positivo.

### 3.3 PUT overrides (admin)
`PUT /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`

Request:
```json
{
  "overrides": [
    {
      "codigoPermiso": "pdf.download",
      "permitido": 0,
      "motivo": "Restringido por politica temporal"
    },
    {
      "codigoPermiso": "pdf.print",
      "permitido": 1,
      "motivo": "Habilitado por aprobacion de jefe"
    }
  ]
}
```

Response success:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "codigoImplementacion": "gestion_correspondencia",
    "idUsuario": 141,
    "procesados": 2
  },
  "errors": []
}
```

### 3.4 DELETE override (admin)
`DELETE /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`

Response success:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "codigoImplementacion": "workflow",
    "idUsuario": 141,
    "procesados": 1
  },
  "errors": []
}
```

## 4. Contrato de errores
Error de validacion (ejemplo: claim faltante):
```json
{
  "success": false,
  "message": "Error de validacion",
  "data": null,
  "errors": [
    "Claim defaulalias es obligatorio"
  ]
}
```

Error de autorizacion (ejemplo endpoint admin):
```json
{
  "success": false,
  "message": "No autorizado",
  "data": null,
  "errors": [
    "No cuenta con permisos administrativos"
  ]
}
```

## 5. Regla de contrato por usuario (sin codiperfil)
- El frontend nunca envia `codiperfil`.
- El backend resuelve perfil internamente usando:
  - `ra_vis_per_usuario_perfil`
  - vigencia (`fecha_inicio`, `fecha_fin`)
  - estado de asignacion.

## 6. Integracion de DI
Registrar en `Program.cs`:
- Service en bloque `// Services (L)`.
- Repository en bloque `// Repositories (R)`.

## 7. Politicas de calidad
- `try/catch` por capa.
- Sin SQL manual concatenado.
- Todo acceso por `DapperCrudEngine + QueryOptions`.

