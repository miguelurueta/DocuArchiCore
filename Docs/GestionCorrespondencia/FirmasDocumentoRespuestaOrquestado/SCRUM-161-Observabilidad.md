# SCRUM-161 Observabilidad

## objetivo de observabilidad
Trazar ejecución de API orquestada de firmas para identificar vacíos, autorización y fallas.

## logs agregados
No nuevos dedicados; se aprovecha instrumentación existente en servicios dependientes.

## niveles de logs
Information/Warning/Error vía servicios existentes.

## estructura de logs
Campos de contexto ya presentes en APIs/servicios subyacentes.

## campos capturados
alias, usuarioId, idUsuarioGestion (en logs de cadena existente).

## requestId / correlation id
Se usa `HttpContext.TraceIdentifier` en flujo de firmas relacionado.

## alias capturado
Sí.

## usuarioId capturado
Sí.

## idSolicitudAprobacion capturado
No aplica.

## cantidad de registros
Disponible indirectamente por logs de truncamiento en servicio de autorizadas.

## indicador de truncamiento
Warning en servicio base cuando supera 100.

## puntos de trazabilidad
- controller
- service
- repository (indirecto)

## troubleshooting
- sin resultados
- usuario no autorizado
- duplicados detectados
- error del engine
- truncamiento

## ejemplos de logs
Consultar logs de `SolicitaListaFirmasAutorizadasDocumento` y `SolicitaUsuarioPrincipalRespuesta`.

## recomendaciones operativas
Agregar log explícito del orquestador en siguiente iteración para visibilidad consolidada.

## METADATA
- identificador del ticket: SCRUM-161
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: observabilidad API orquestada
- relación con tickets previos: SCRUM-159, SCRUM-160
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
