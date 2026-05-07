## Context

- Jira issue key: `SCRUM-192`
- Jira summary: `IMPLEMENTACION-AUDITORIA-INTEGRAL-ALMACENAMIENTO`
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-192
- Legacy source of truth: `D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt`

## Problem Statement

El sistema ya pasó por múltiples tickets funcionales de migración.  
Falta consolidar una auditoría final que confirme:

1. paridad funcional real VB vs C# (o desviaciones aprobadas),
2. evidencia de pruebas E2E y regresión,
3. riesgos residuales clasificados,
4. decisión formal de despliegue.

Sin este cierre, existe riesgo de pasar a producción con brechas no trazadas.

## Scope

### In Scope

- Auditoría de componentes Storage Engine:
  - Orchestrator y pipeline de validación
  - Metadata gabinete y preindex
  - Opciones `system1` (inventario/TRD/unidad)
  - Metadata física (ruta/tamaño/páginas/naming)
  - Transacción DB y registros de inventario/expediente/log
  - XML documental y XML índice expediente
  - CompensationManager
  - DI y feature flag `StorageEngineV2`
- Matriz de paridad final con severidad, evidencia y acción.
- Decisión `GO/GO CONDICIONADO/NO GO`.
- Actualización documental `SCRUM-189-*`.

### Out of Scope

- Desarrollo de funcionalidades nuevas.
- Cambios de esquema DB.
- Ajustes de comportamiento para “forzar” paso de pruebas.

## Architectural Decisions

1. **Legacy-first rule**: ante conflicto entre prompts/documentación e implementación, prevalece el comportamiento observado en el legacy.
2. **Evidence-first closure**: no se marca cierre sin evidencia técnica trazable (archivo/línea/test/log).
3. **No hidden deviations**: toda diferencia se clasifica explícitamente:
   - `CUMPLE`
   - `CUMPLE CON DESVIACIÓN APROBADA`
   - `NO APLICA`
   - `NO CUMPLE`
4. **Critical fail policy**: si hay un `NO CUMPLE` crítico, decisión automática `NO GO`.

## Validation Strategy

- Ejecutar matriz de escenarios E2E definidos por el ticket.
- Correlacionar cada regla legacy con componente C# y caso de prueba.
- Verificar observabilidad y ausencia de stubs/placeholders.
- Consolidar conclusión técnica en documento de decisión final.

## Deliverables

- `SCRUM-189-Arquitectura-Cierre-Integral.md`
- `SCRUM-189-Matriz-Paridad-StorageEngine.md`
- `SCRUM-189-Pruebas-E2E-StorageEngine.md`
- `SCRUM-189-Riesgos-Desviaciones-Aprobadas.md`
- `SCRUM-189-Go-NoGo-StorageEngine.md`
- `SCRUM-189-Metadata.md`

## No-Regression Constraints

- No cambiar lógica funcional durante la auditoría.
- No suprimir evidencia negativa.
- No cerrar ticket si faltan escenarios críticos o pruebas base.
