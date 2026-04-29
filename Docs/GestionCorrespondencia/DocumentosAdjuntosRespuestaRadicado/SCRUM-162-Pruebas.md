# SCRUM-162 Pruebas

## estrategia de pruebas
Pruebas unitarias de controller y service para validación, éxito, empty, deduplicación y límite.

## matriz unitaria
- controller: claim inválido -> bad request
- controller: id inválido -> bad request
- controller: success -> ok
- service: alias inválido -> validación
- service: empty -> meta empty
- service: deduplicación + límite 100 -> success

## matriz integración
Pendiente en entorno con DB real (consulta SQL + datos productivos enmascarados).

## matriz QT
- contrato de respuesta `AppResponses`
- consistencia de `meta.status`

## matriz regresión
Verificar que no afecta endpoints previos de `GestionRespuesta` y `Firmas`.

## pruebas de autorización
Cubierto claim `defaulalias` inválido en controller.

## pruebas de deduplicación
Cubierto en service con dataset duplicado.

## pruebas de nulls
Cubierto indirectamente por tratamiento `COALESCE` y rutas empty/validación.

## pruebas de límite 100
Cubierto en service con 110 únicos + duplicados.

## casos cubiertos
Validación de entrada, éxito, vacío, dedup, truncamiento.

## casos no cubiertos
Integración repository con DB real y comportamiento bajo alta concurrencia.

## hallazgos
Se detectó necesidad de registrar DI faltante en `Program.cs`.

## riesgos residuales
Calidad de datos legacy y posible necesidad de índice adicional por volumen.
