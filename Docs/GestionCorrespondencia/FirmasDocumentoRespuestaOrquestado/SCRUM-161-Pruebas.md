# SCRUM-161 Pruebas

## estrategia de pruebas
Unit tests de controller y service orquestado, validando contratos, deduplicación y estados.

## matriz unitaria
- Controller: claim inválido, query inválido, success.
- Service: alias inválido, empty, merge+dedupe.

## matriz integración
Pendiente de validación integrada e2e con servicios dependientes.

## matriz QT
Prueba manual de endpoint con JWT real en QA.

## matriz regresión
Verificar que endpoints SCRUM-159/160 sigan sin regresión.

## pruebas de autorización
Cubierto por propagación de servicio de autorizadas.

## pruebas de deduplicación
Cubierto en combinación de principal + autorizadas.

## pruebas de nulls
Cubierto indirectamente por servicios base.

## pruebas de límite 100
Cobierto por implementación `Take(100)`; falta test explícito >100.

## casos cubiertos
6 casos unitarios.

## casos no cubiertos
Integración real y carga.

## hallazgos
Se reduce roundtrip frontend al consolidar llamadas.

## riesgos residuales
Dependencia de reglas de servicios base reutilizados.

## METADATA
- identificador del ticket: SCRUM-161
- usuario que creó el ticket: (según Jira)
- fecha de creación: (según Jira)
- módulo afectado: Gestión Correspondencia / Firmas
- objetivo resumido: validar API orquestada
- relación con tickets previos: SCRUM-159, SCRUM-160
- autor de implementación: Codex + equipo backend
- autor de documentación: Codex
- estado del ticket: En implementación
