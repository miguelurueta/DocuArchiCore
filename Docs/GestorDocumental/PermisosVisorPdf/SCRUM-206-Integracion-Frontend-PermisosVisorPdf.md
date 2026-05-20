# SCRUM-206 Integracion Frontend Permisos Visor PDF

## 1. Objetivo Frontend
Consumir permisos efectivos del visor PDF por implementación para controlar renderizado y acciones del componente sin reglas hardcodeadas.

## 2. Seguridad
- Header obligatorio: `Authorization: Bearer {jwt}`
- Claims mínimos en JWT:
  - `defaulalias`
  - `usuarioid`

## 3. APIs del módulo
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`
- `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos` (admin)
- `PUT /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides` (admin)
- `DELETE /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}` (admin)

## 4. Ejemplo claro con valores específicos (guía front)
Este ejemplo usa el contexto de tablas `ra_vis_per_*` y usuario interno en `remit_dest_interno.id_Remit_Dest_Int`.

### 4.1 Contexto de datos (caso ejemplo)
- `codigoImpl`: `workflow`
- `idUsuario`: `141`
- Perfil asignado: `FIRMANTE`
- Overrides activos para el usuario:
  - `pdf.print = 1`
  - `pdf.download = 1`

Referencia de seed:
- Implementaciones: `workflow`, `gestion_correspondencia`
- Catálogo: `pdf.view`, `pdf.print`, `pdf.download`, `pdf.annotate.*`, `pdf.rotate`, `pdf.zoom`
- Perfil `FIRMANTE`: habilita firma/navegación y deja `print/download` en `0` por defecto.

### 4.2 Solicitar permisos de un usuario específico (admin)
Request:
```http
GET /api/gestor-documental/permisos-visorpdf/implementaciones/workflow/usuarios/141/permisos
Authorization: Bearer eyJ... (token admin)
```

Response OK esperado para el contexto anterior:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "CodigoImplementacion": "workflow",
    "IdUsuario": 141,
    "Permissions": {
      "pdf.view": true,
      "pdf.print": true,
      "pdf.download": true,
      "pdf.annotate.open_signature_modal": true,
      "pdf.annotate.signature.draw": true,
      "pdf.annotate.signature.upload": true,
      "pdf.annotate.signature.personal": true,
      "pdf.annotate.signature.place": true,
      "pdf.annotate.signature.delete": true,
      "pdf.annotate.signature.lock": true,
      "pdf.annotate.signature.unlock": true,
      "pdf.rotate": true,
      "pdf.zoom": true
    },
    "Sources": {
      "pdf.view": "perfil_activo",
      "pdf.print": "usuario_override",
      "pdf.download": "usuario_override",
      "pdf.annotate.open_signature_modal": "perfil_activo",
      "pdf.annotate.signature.draw": "perfil_activo",
      "pdf.annotate.signature.upload": "perfil_activo",
      "pdf.annotate.signature.personal": "perfil_activo",
      "pdf.annotate.signature.place": "perfil_activo",
      "pdf.annotate.signature.delete": "perfil_activo",
      "pdf.annotate.signature.lock": "perfil_activo",
      "pdf.annotate.signature.unlock": "perfil_activo",
      "pdf.rotate": "perfil_activo",
      "pdf.zoom": "perfil_activo"
    },
    "GeneratedAt": "2026-05-20T14:25:00Z"
  },
  "meta": { "Status": "success", "Total": 13 },
  "errors": []
}
```

Response no autorizado (`401`):
```json
{
  "success": false,
  "message": "No cuenta con permisos administrativos",
  "data": {},
  "meta": { "Status": "validation" },
  "errors": [
    {
      "Type": "Validation",
      "Field": "authorization",
      "Message": "No cuenta con permisos administrativos"
    }
  ]
}
```

### 4.3 Solicitar mis permisos (usuario autenticado)
Request:
```http
GET /api/gestor-documental/permisos-visorpdf/implementaciones/workflow/mis-permisos
Authorization: Bearer eyJ... (token del usuario 141)
```

Response OK:
- Misma estructura del contrato anterior.
- `IdUsuario` sale del claim `usuarioid` del JWT.
### 4.4 Segundo escenario claro: gestion_correspondencia + perfil LECTOR (sin overrides)
Contexto de datos:
- `codigoImpl`: `gestion_correspondencia`
- `idUsuario`: `205`
- Perfil asignado: `LECTOR`
- Overrides activos: ninguno

Request:
```http
GET /api/gestor-documental/permisos-visorpdf/implementaciones/gestion_correspondencia/usuarios/205/permisos
Authorization: Bearer eyJ... (token admin)
```

Response OK esperado:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "CodigoImplementacion": "gestion_correspondencia",
    "IdUsuario": 205,
    "Permissions": {
      "pdf.view": true,
      "pdf.print": false,
      "pdf.download": false,
      "pdf.annotate.open_signature_modal": false,
      "pdf.annotate.signature.draw": false,
      "pdf.annotate.signature.upload": false,
      "pdf.annotate.signature.personal": false,
      "pdf.annotate.signature.place": false,
      "pdf.annotate.signature.delete": false,
      "pdf.annotate.signature.lock": false,
      "pdf.annotate.signature.unlock": false,
      "pdf.rotate": false,
      "pdf.zoom": true
    },
    "Sources": {
      "pdf.view": "perfil_activo",
      "pdf.zoom": "perfil_activo",
      "pdf.print": "perfil_activo",
      "pdf.download": "perfil_activo",
      "pdf.annotate.open_signature_modal": "perfil_activo",
      "pdf.annotate.signature.draw": "perfil_activo",
      "pdf.annotate.signature.upload": "perfil_activo",
      "pdf.annotate.signature.personal": "perfil_activo",
      "pdf.annotate.signature.place": "perfil_activo",
      "pdf.annotate.signature.delete": "perfil_activo",
      "pdf.annotate.signature.lock": "perfil_activo",
      "pdf.annotate.signature.unlock": "perfil_activo",
      "pdf.rotate": "perfil_activo"
    },
    "GeneratedAt": "2026-05-20T14:40:00Z"
  },
  "meta": { "Status": "success", "Total": 13 },
  "errors": []
}
```

Interpretación rápida para FE:
- Renderiza documento (`pdf.view=true`).
- Permite zoom (`pdf.zoom=true`).
- Bloquea impresión, descarga, firma y rotación (`false`).

### 4.5 Tercer escenario claro: usuario sin perfil (con consumo individual)
Contexto de datos:
- `codigoImpl`: `workflow`
- `idUsuario`: `333`
- Sin registro vigente en `ra_vis_per_usuario_perfil` para `workflow`
- Overrides activos:
  - `pdf.download = 1`

Regla aplicada:
- Sin perfil, el motor resuelve con:
  1. `usuario_override`
  2. `default_implementacion`
  3. `fallback_deny`

Request:
```http
GET /api/gestor-documental/permisos-visorpdf/implementaciones/workflow/usuarios/333/permisos
Authorization: Bearer eyJ... (token admin)
```

Response OK esperado:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "CodigoImplementacion": "workflow",
    "IdUsuario": 333,
    "Permissions": {
      "pdf.view": true,
      "pdf.print": false,
      "pdf.download": true,
      "pdf.annotate.open_signature_modal": false,
      "pdf.annotate.signature.draw": false,
      "pdf.annotate.signature.upload": false,
      "pdf.annotate.signature.personal": false,
      "pdf.annotate.signature.place": false,
      "pdf.annotate.signature.delete": false,
      "pdf.annotate.signature.lock": false,
      "pdf.annotate.signature.unlock": false,
      "pdf.rotate": false,
      "pdf.zoom": false
    },
    "Sources": {
      "pdf.view": "default_implementacion",
      "pdf.download": "usuario_override",
      "pdf.print": "default_implementacion",
      "pdf.annotate.open_signature_modal": "default_implementacion",
      "pdf.annotate.signature.draw": "default_implementacion",
      "pdf.annotate.signature.upload": "default_implementacion",
      "pdf.annotate.signature.personal": "default_implementacion",
      "pdf.annotate.signature.place": "default_implementacion",
      "pdf.annotate.signature.delete": "default_implementacion",
      "pdf.annotate.signature.lock": "default_implementacion",
      "pdf.annotate.signature.unlock": "default_implementacion",
      "pdf.rotate": "default_implementacion",
      "pdf.zoom": "default_implementacion"
    },
    "GeneratedAt": "2026-05-20T15:00:00Z"
  },
  "meta": { "Status": "success", "Total": 13 },
  "errors": []
}
```

Interpretación rápida para FE:
- Sí puede consultar permisos aunque no tenga perfil.
- `pdf.view=true` por default de implementación.
- `pdf.download=true` por override individual.

## 5. Ejemplos de request admin para ajustar permisos
### 5.1 Upsert override (habilitar print/download)
Request:
```http
PUT /api/gestor-documental/permisos-visorpdf/implementaciones/workflow/usuarios/141/overrides
Authorization: Bearer eyJ... (token admin)
Content-Type: application/json
```

Body:
```json
{
  "Overrides": [
    {
      "CodigoPermiso": "pdf.print",
      "Permitido": 1,
      "Motivo": "Habilitado por excepción para jefe de área"
    },
    {
      "CodigoPermiso": "pdf.download",
      "Permitido": 1,
      "Motivo": "Habilitado para envío a auditoría"
    }
  ]
}
```

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "CodigoImplementacion": "workflow",
    "IdUsuario": 141,
    "Procesados": 2
  },
  "meta": { "Status": "success" },
  "errors": []
}
```

### 5.2 Delete override puntual
Request:
```http
DELETE /api/gestor-documental/permisos-visorpdf/implementaciones/workflow/usuarios/141/overrides/pdf.print
Authorization: Bearer eyJ... (token admin)
```

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "CodigoImplementacion": "workflow",
    "IdUsuario": 141,
    "Procesados": 1
  },
  "meta": { "Status": "success" },
  "errors": []
}
```

## 6. Paso a paso de consumo frontend
1. Definir `codigoImpl` del módulo (`workflow` o `gestion_correspondencia`).
2. Para sesión normal, llamar `mis-permisos`.
3. Para pantalla admin de soporte, llamar `usuarios/{idUsuario}/permisos`.
4. Aplicar UI con `data.Permissions`.
5. Si hay cambios, ejecutar `PUT`/`DELETE` y recargar permisos.

## 7. Errores esperados
- `400 Validation`: claims inválidos, implementación/permiso inválido, payload inválido.
- `401`: endpoint admin sin autorización.
- `500`: error técnico no controlado.

## 8. Checklist de depuración frontend
- JWT vigente con `defaulalias` y `usuarioid`.
- `codigoImpl` existe (`workflow` o `gestion_correspondencia`).
- `idUsuario` corresponde a `remit_dest_interno.id_Remit_Dest_Int`.
- No enviar `codiperfil` desde frontend.
- Si `success=false`, mostrar `message` y detalle de `errors`.

