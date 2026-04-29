# SCRUM-160 Pruebas

## estrategia de pruebas
Cobertura unitaria de controller y service con Moq; validación de contrato `AppResponses` y reglas funcionales críticas.

## matriz unitaria
- Controller: claim inválido, query inválido, success.
- Service: no autorizado, empty, deduplicación + nulls.

## matriz integración
Pendiente de ambiente DB para tabla objetivo y datos controlados.

## matriz QT
Pruebas manuales API en ambiente QA con JWT real.

## matriz regresión
Verificar que SCRUM-159 (`firmas permitidas`) no se afecte.

## pruebas de autorización
Cubierto: `usuarioId != idUsuarioAutorizado` retorna `No autorizado`.

## pruebas de deduplicación
Cubierto: IDs repetidos colapsan a un único elemento.

## pruebas de nulls
Cubierto: nombre nulo -> `Sin nombre`, cargo vacío no concatena.

## pruebas de límite 100
Cubierto en implementación (limit/take); pendiente test explícito con >100 registros mockeados.

## casos cubiertos
6 casos unitarios (3 controller + 3 service).

## casos no cubiertos
Integración real contra DB, pruebas de carga y truncamiento observando logs.

## hallazgos
Se confirmó consistencia del patrón con SCRUM-159 y manejo de estados `success|empty|error`.

## riesgos residuales
Nombre de tabla y calidad de datos en producción.

## METADATA
- identificador del ticket: SCRUM-160
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: validar endpoint de firmas autorizadas
- relación con tickets previos: SCRUM-159
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
