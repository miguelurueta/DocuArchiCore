# SCRUM-160 Integracion Frontend

## descripción de API
API para obtener firmantes autorizados de un usuario.

## método HTTP
GET

## ruta oficial
`/api/gestion-correspondencia/firmas/autorizadas-documento`

## query params
`idUsuarioAutorizado: long` (obligatorio, > 0)

## origen esperado de idUsuarioAutorizado
Id del usuario autenticado en contexto de sesión/claim de frontend.

## claims requeridos
`defaulalias`, `usuarioid`

## response AppResponses<List<ResponseDropdownDto>>
Contrato estándar backend.

## significado de
- success: operación técnica/funcional ejecutada.
- message: texto de resultado.
- data: lista para dropdown.
- meta.status: `success|empty|error`.
- errors: detalle de validación o excepción.

## ejemplo request válido
`GET /api/gestion-correspondencia/firmas/autorizadas-documento?idUsuarioAutorizado=12`

## ejemplo response success
`{ "success": true, "message": "YES", "data": [{"Id":2,"Descripcion":"Ana - Analista"}], "meta": {"status":"success"}, "errors": [] }`

## ejemplo response empty
`{ "success": true, "message": "Sin resultados", "data": [], "meta": {"status":"empty"}, "errors": [] }`

## ejemplo response unauthorized/error
`{ "success": false, "message": "No autorizado", "data": [], "meta": {"status":"error"}, "errors": [{"type":"Validation","field":"usuarioId","message":"No autorizado"}] }`

## reglas de consumo frontend
Validar `success` y `meta.status`; no asumir datos siempre presentes.

## cómo poblar dropdown/listado de firmas
Mapear `data[].Id` como valor y `data[].Descripcion` como etiqueta.

## UX para lista vacía
Mostrar estado vacío con mensaje claro y opción de refrescar.

## UX para error
Mostrar toast/error inline y habilitar reintento.

## UX para no autorizado
Bloquear componente y mostrar mensaje de permisos.

## consideraciones de seguridad
No reutilizar `idUsuarioAutorizado` de otra sesión; usar claim vigente.

## no asumir que siempre hay firmantes
Caso empty es flujo esperado, no excepción.

## METADATA
- identificador del ticket: SCRUM-160
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: integración frontend de firmas autorizadas
- relación con tickets previos: SCRUM-159
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
