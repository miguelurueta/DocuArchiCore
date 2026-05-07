## Context

- Jira issue key: SCRUM-192
- Jira summary: IMPLEMENTACION-AUDITORIA-INTEGRAL-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-192

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 23C — Auditoría Integral + Matriz de Paridad + Go/No-Go (FASE FINAL — CIERRE DE CALIDAD Y DESPLIEGUE) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: auditoría técnica de migraciones legacy VB → C# DocuArchi / SGDEA pruebas E2E enterprise análisis de riesgo funcional trazabilidad técnica gobernanza de despliegue validación de consistencia DB + FileSystem + XML observabilidad avanzada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Emitir el cierre formal del Storage Engine con evidencia verificable y auditable: ✔ Paridad funcional VB vs C# confirmada o desviaciones aprobadas ✔ Pruebas E2E completas y repetibles ✔ Matriz de cumplimiento final trazable ✔ Clasificación de riesgos ✔ Decisión formal: GO / GO CONDICIONADO / NO GO Este prompt NO implementa código nuevo. Solo valida, audita y cierra. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ El código legacy consolidado es la fuente de verdad funcional. Ruta: D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt Archivo utilizado en la revisión: funcion-almacena-consolidad(2).txt :contentReference[oaicite:0]{index=0} REGLA DE PARIDAD Si existe discrepancia entre:
- prompts
- documentación
- interpretación
- implementación C#

Debe prevalecer el comportamiento observado en el código legacy. Toda desviación debe documentarse como: CUMPLE CON DESVIACIÓN APROBADA con justificación técnica, impacto y evidencia. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALCANCE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Auditar integralmente todos los componentes: - Runtime core (Orchestrator)
- ValidationPipeline
- Metadata gabinete
- Preindex
- Opciones system1
- Metadata física (tamaño/páginas)
- Naming DIG/FXL
- Ruta física SYSTEM1RUT
- Transacción DB (system1, disco_detalle, gabinete)
- Inventario documental
- Expediente/unidad
- Índice electrónico (DB)
- XML documental FXL
- XML índice expediente
- Logdocuarchi
- CompensationManager
- DI (Program.cs)
- Feature flag StorageEngineV2 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MATRIZ OBLIGATORIA DE PARIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Debe generarse una matriz con columnas: Comportamiento Legacy
Función Legacy Origen
Componente C# Equivalente
Estado
Severidad
Evidencia (archivo + línea)
Riesgo Operativo
Acción de Cierre
Caso de Prueba Asociado
Resultado de Prueba Estados permitidos CUMPLE
CUMPLE CON DESVIACIÓN APROBADA
NO APLICA
NO CUMPLE Regla crítica Si existe al menos un NO CUMPLE crítico → NO GO automático ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ E2E (End-to-End) Ejecutar como mínimo: 1. almacenamiento simple
2. batch preindex TXT
3. batch preindex XMLS
4. inventario activo
5. TRD activa
6. expediente activo
7. unidad conservación digitalizada
8. unidad conservación electrónica
9. expediente electrónico con índice
10. workflow con logdocuarchi
11. falla DB (rollback)
12. falla FileSystem (compensación)
13. falla XML (compensación)
14. concurrencia (reserva de identidad) Regresión Legacy - comparar resultados contra comportamiento VB
- validar mensajes de error equivalentes
- validar naming, rutas y XML
- validar registros DB Validaciones específicas - IdAlmacen único y consistente
- proxid correcto en system1
- NUMPAG_CARP correcto
- gabinete dinámico correcto
- inventario completo
- expediente/unidad consistente
- XML FXL correcto
- XML índice expediente correcto
- logdocuarchi correcto
- rutas físicas correctas ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD A VALIDAR ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Debe verificarse: requestId (correlación completa)
idAlmacen
idRegistroProduccionDocumental
nombreGabinete
usuarioId
fase de ejecución
duración total
duración por fase
errores con código funcional Reglas: - NO exponer datos sensibles
- NO loguear fulltext completo
- NO loguear contenido documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIONES ADICIONALES CRÍTICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Debe validarse explícitamente: - Feature flag StorageEngineV2 funcional
- Posibilidad de rollback a legacy (fallback)
- No existen stubs activos (Task.CompletedTask, valores dummy)
- DI completo en Program.cs
- Todos los servicios registrados correctamente (Scoped)
- No existen rutas muertas
- No existen placeholders ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar documentación en: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\ Ticket: SCRUM-189 Crear/actualizar: SCRUM-189-Arquitectura-Cierre-Integral.md
SCRUM-189-Matriz-Paridad-StorageEngine.md
SCRUM-189-Pruebas-E2E-StorageEngine.md
SCRUM-189-Riesgos-Desviaciones-Aprobadas.md
SCRUM-189-Go-NoGo-StorageEngine.md
SCRUM-189-Metadata.md Debe incluir: - matriz completa de paridad
- evidencia de pruebas
- decisiones de desviación
- riesgos residuales
- estrategia de rollback
- evidencia de observabilidad
- verificación de eliminación de stubs
- verificación de DI completo ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE DECISIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ GO:
- 0 NO CUMPLE críticos
- sistema estable
- pruebas en verde
- evidencia completa

GO CONDICIONADO:
- existen desviaciones medias/altas
- mitigación definida
- fecha de corrección definida
- riesgo controlado

NO GO:
- existe al menos un NO CUMPLE crítico
- inconsistencia funcional
- riesgo operativo alto
- falta de evidencia ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ Matriz de paridad completa y trazable ✔ 0 brechas críticas abiertas ✔ Pruebas E2E exitosas ✔ Evidencia documentada ✔ Riesgos clasificados ✔ Decisión formal emitida ✔ Documentación SCRUM-189 actualizada ✔ No existen stubs ✔ DI validado ✔ Sistema auditable end-to-end ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ NO desarrollar funcionalidades nuevas NO modificar lógica para “pasar pruebas” NO ocultar desviaciones NO usar datos productivos reales NO omitir escenarios críticos NO cerrar ticket sin evidencia ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ejecutar auditoría integral del Storage Engine, validar paridad contra el código legacy, consolidar evidencia técnica completa y emitir decisión formal de despliegue (GO / GO CONDICIONADO / NO GO). Actualizar la documentación en SCRUM-189 y dejar trazabilidad completa para auditoría y operación productiva.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-192-implementacion-auditoria-integral-almace.