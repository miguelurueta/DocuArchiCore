# SCRUM-162 Observabilidad

## objetivo de observabilidad
Permitir trazabilidad completa de la consulta de adjuntos por tarea y diagnóstico rápido de fallos funcionales/técnicos.

## logs agregados
- controller: inicio de solicitud con `idTareaWf` y alias
- repository: cantidad de filas y tiempo de ejecución

## niveles de logs
- Information: flujo normal
- Error: excepción capturada en capas de negocio/datos

## estructura de logs
Mensajes estructurados con placeholders (`{IdTareaWf}`, `{Alias}`, `{Rows}`, `{Ms}`).

## campos capturados
- requestId / correlation id: vía contexto HTTP (disponible en pipeline)
- alias capturado: sí
- usuarioId capturado: no en este endpoint
- idSolicitudAprobacion capturado: no aplica
- idSolicitudAprobacion equivalente (`idTareaWf`): sí
- cantidad de registros: sí
- indicador de truncamiento: implícito por regla `Take(100)` (recomendado explicitar en mejora futura)

## puntos de trazabilidad
- controller: validación claim y parámetros
- service: validaciones, deduplicación, límite
- repository: SQL, tiempo y total de filas

## troubleshooting
- sin resultados: revisar existencia de `idTareaWf` y datos en `IdImagen/IdImagenRespuesta`
- usuario no autorizado: validar token y claim `defaulalias`
- duplicados detectados: esperado; se deduplica en service
- error del engine: revisar conexión alias/db y excepción repository
- truncamiento: si hay más de 100, paginar en endpoint futuro

## ejemplos de logs
- `SolicitaDocumentosAdjuntosRespuestaRadicado: idTareaWf=1250 alias=WF`
- `SolicitaDocumentosAdjuntosRespuestaRadicado: idTareaWf=1250 alias=WF rows=48 ms=22`

## recomendaciones operativas
- agregar métrica explícita de truncamiento (`isTruncated`, `originalCount`)
- centralizar `correlation-id` en middleware y propagarlo a todos los logs de GestiónCorrespondencia
