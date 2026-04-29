# SCRUM-159 - Observabilidad

## Objetivo de observabilidad
Garantizar trazabilidad del flujo de consulta de firmantes permitidos por solicitud, facilitando diagnóstico de vacíos funcionales, fallas de autorización y errores de acceso a datos.

## Logs agregados
- Repository:
  - Warning cuando la consulta no retorna datos o falla en lectura (`query without data ...`).

## Niveles de logs
- `Warning`: ausencia de datos/fallo de query retornado por engine.
- `Error`: actualmente manejado en respuesta de service; recomendado elevar a log explícito en catch futuro.

## Estructura de logs
- Mensaje con contexto:
  - `idSolicitudAprobacion`
  - `alias`
  - `message` del engine

## Campos capturados
- `idSolicitudAprobacion`
- `defaultDbAlias`
- `message` retorno del engine

## requestId / correlation id
- Endpoint puede usar `HttpContext.TraceIdentifier` y `X-Request-Id` en capa controller.
- Recomendado incluirlo explícitamente en logs de service/repository para correlación end-to-end.

## alias capturado
- Sí, en log de repository (`alias={Alias}`).

## usuarioId capturado
- Validado en controller/service.
- Recomendado agregarlo explícitamente a logs de service.

## idSolicitudAprobacion capturado
- Sí, en log de repository.

## cantidad de registros
- No se registra actualmente de forma explícita.
- Recomendado: loggear `rows.Count` luego de map/dedupe.

## indicador de truncamiento
- No se registra actualmente.
- Recomendado: `isTruncated = rows.Count > 100 || mappedOriginalCount > 100`.

## Puntos de trazabilidad
- Controller:
  - Validación de claims y parámetros.
  - Inicio de ejecución por request.
- Service:
  - Reglas de negocio, deduplicación, status success/empty/error.
- Repository:
  - QueryOptions + ejecución de consulta + warning de no datos.

## Troubleshooting
- Sin resultados:
  - Verificar `idSolicitudAprobacion` válido.
  - Validar estado `ESTADO_AUTORIZACION_FIRMA=1`.
- Usuario no autorizado:
  - Revisar claims `defaulalias`/`usuarioid`.
- Duplicados detectados:
  - Confirmar data fuente y dedupe por `Id` en service.
- Error del engine:
  - Revisar alias, conectividad DB, columnas/join/filtros.
- Truncamiento:
  - Verificar cardinalidad de resultados y necesidad de paginación.

## Ejemplos de logs
```text
WARN SolicitaListaFirmasPermitidasPorSolicitudAsync: query without data idSolicitudAprobacion=123 alias=WF message=Sin resultados
```

## Recomendaciones operativas
1. Agregar logging informativo en controller/service con `requestId`, `usuarioId`, `idSolicitudAprobacion`.
2. Incorporar métrica de conteo (`rows_found`, `rows_returned`, `is_truncated`).
3. Unificar plantilla de logs para todos los endpoints de GestionCorrespondencia/Firmas.
