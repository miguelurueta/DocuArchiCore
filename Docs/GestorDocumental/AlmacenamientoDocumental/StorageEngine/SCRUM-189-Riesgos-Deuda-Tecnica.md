# SCRUM-189 — Riesgos y Deuda Técnica StorageEngine

## 1. Riesgos actuales
| Riesgo | Probabilidad | Impacto | Estado | Mitigación |
|---|---|---|---|---|
| Orquestador aún incremental (`pipeline futuro`) | Media | Alta | Abierto | cerrar wiring completo de fases en un único flujo |
| Escenarios E2E con Docker no siempre ejecutados | Alta | Alta | Abierto | pipeline dedicado con Testcontainers obligatorio |
| Dependencia en tablas dinámicas de gabinete | Media | Alta | Abierto | validación metadata y smoke por gabinete |
| Divergencia entre commit DB y fallos FS/XML | Media | Alta | Controlado | compensation + monitoreo de inconsistencias |
| Degradación por locks serializables en alta concurrencia | Media | Media | Abierto | pruebas de carga y ajuste de índices |

## 2. Deuda técnica identificada
1. Consolidar `DocumentStorageOrchestrator` como engine completo (sin placeholders).
2. Reducir configuración opcional de dependencias críticas en runtime.
3. Estandarizar más pruebas de integración hoy marcadas `Skip`.
4. Fortalecer telemetría de errores de compensación con alertas activas.

## 3. Automatizaciones pendientes
1. Job nocturno de paridad (subset crítico) con evidencia versionada.
2. Gate de PR que valide que matriz de paridad no retrocede.
3. Reporte automático de inventario de funciones por módulo (drift detection).

## 4. Riesgos de regresión
- Cambios en reglas `system1` sin pruebas de opciones.
- Cambios en repos de expediente/unidad sin pruebas de lock.
- Cambios en naming/ruta sin validar compatibilidad con visor legacy.

## 5. Prioridad de remediación
1. Alta: cierre de E2E Docker y concurrencia real.
2. Alta: consolidación final del orchestrator.
3. Media: endurecimiento observabilidad y tableros de compensación.
