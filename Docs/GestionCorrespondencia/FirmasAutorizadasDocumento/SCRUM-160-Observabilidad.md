# SCRUM-160 Observabilidad

## objetivo de observabilidad
Facilitar diagnóstico de consulta de firmantes autorizados y detectar vacíos, no autorizados, errores y truncamiento.

## logs agregados
- Warning por query sin datos (repository)
- Warning por truncamiento a 100 (service)
- Error por excepción (service)

## niveles de logs
`Warning`, `Error`

## estructura de logs
Mensajes estructurados con placeholders y contexto de negocio.

## campos capturados
`idUsuarioAutorizado`, `usuarioId`, `alias`, `cantidadInicial`, `message`.

## requestId / correlation id
Se apoya en trazabilidad ASP.NET/infra existente.

## alias capturado
Sí, `defaultDbAlias`.

## usuarioId capturado
Sí, claim `usuarioid`.

## idSolicitudAprobacion capturado
No aplica a SCRUM-160 (parámetro es `idUsuarioAutorizado`).

## cantidad de registros
Capturado al truncar (`cantidadInicial`).

## indicador de truncamiento
Log warning cuando total deduplicado supera 100.

## puntos de trazabilidad
- controller: validación de claims/query
- service: autorización, dedupe, empty/error
- repository: query y fallback vacío

## troubleshooting
- sin resultados: revisar filtros y estado usuario
- usuario no autorizado: validar claim `usuarioid` vs query
- duplicados detectados: revisar configuración de firmas en DB
- error del engine: validar alias y conectividad
- truncamiento: ajustar negocio si requiere paginación futura

## ejemplos de logs
- `Warning ... truncado a 100 idUsuarioAutorizado=12 usuarioId=12 alias=WF cantidadInicial=135`
- `Error ... error idUsuarioAutorizado=12 usuarioId=12`

## recomendaciones operativas
Agregar dashboard con ratio `success/empty/error` y alertar cuando crezcan errores o truncamientos.

## METADATA
- identificador del ticket: SCRUM-160
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: trazabilidad operativa de consulta de firmantes
- relación con tickets previos: SCRUM-159
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
