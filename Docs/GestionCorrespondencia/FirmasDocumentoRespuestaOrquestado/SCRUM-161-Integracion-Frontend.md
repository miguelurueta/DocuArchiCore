# SCRUM-161 Integracion Frontend

## descripción de API
API orquestada para firmantes de documento respuesta.

## método HTTP
GET

## ruta oficial
`/api/gestion-correspondencia/firmas/documento-respuesta-orquestado`

## query params
`idUsuarioGestion` (int, obligatorio > 0)

## origen esperado de idSolicitudAprobacion
No aplica en SCRUM-161.

## claims requeridos
`defaulalias`, `usuarioid`

## response AppResponses<List<ResponseDropdownDto>>
Devuelve lista consolidada para dropdown.

## significado de
- success
- message
- data
- meta.status
- errors

## ejemplo request válido
`GET /api/gestion-correspondencia/firmas/documento-respuesta-orquestado?idUsuarioGestion=12`

## ejemplo response success
`{ "success": true, "message":"YES", "data":[{"Id":7,"Descripcion":"Ana - Lider"}], "meta":{"status":"success"}, "errors":[] }`

## ejemplo response empty
`{ "success": true, "message":"Sin resultados", "data":[], "meta":{"status":"empty"}, "errors":[] }`

## ejemplo response unauthorized/error
`{ "success": false, "message":"No autorizado", "data":[], "meta":{"status":"error"}, "errors":[...] }`

## reglas de consumo frontend
Consumir `data` sin asumir orden fijo excepto principal al inicio cuando exista.

## cómo poblar dropdown/listado de firmas
`Id` como value, `Descripcion` como label.

## UX para lista vacía
Estado vacío con guía de acción.

## UX para error
Mensaje técnico controlado + reintento.

## UX para no autorizado
Bloquear acción y mostrar permiso insuficiente.

## consideraciones de seguridad
No forzar `idUsuarioGestion` distinto al claim permitido.

## no asumir que siempre hay firmantes
Caso empty es válido.

## METADATA
- identificador del ticket: SCRUM-161
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: consumo frontend API orquestada
- relación con tickets previos: SCRUM-159, SCRUM-160
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
