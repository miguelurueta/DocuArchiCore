# PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE

# PROMPT 23F — Observabilidad + Reconciliación Operativa

# (FASE OPERATIVA — POST-COMPENSACIÓN)



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ROL ESPERADO

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Actúa como Arquitecto Master Backend .NET experto en:



- observabilidad enterprise

- auditoría técnica de sistemas ECM

- reconciliación de inconsistencias DB/FS/XML

- compensaciones idempotentes

- Storage Engine DocuArchi

- Dapper / SQL operativo

- diseño de runbooks

- gobernanza operativa



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBJETIVO

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Formalizar el control operativo de:



```txt

- fallos físicos post-commit

- ejecuciones de compensación (DB + FS)

- detección de inconsistencias (huérfanos)

- reconciliación segura e idempotente

- trazabilidad completa para auditoría

```



Este prompt complementa:



```txt

PROMPT 23D — Compensación lógica DB post-fallo físico

```



No lo reemplaza.



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CONTEXTO

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Después de implementar compensación (23D), pueden existir escenarios:



```txt

- compensación parcial (PARTIAL)

- fallo en compensación

- interrupción del proceso

- inconsistencias residuales DB vs FS/XML

```



Este prompt define cómo:



```txt

- detectar

- auditar

- reconciliar

- monitorear

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ALCANCE

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



### 1. LOGS ESTRUCTURADOS (OBLIGATORIOS)



Todos los eventos deben incluir:



```txt

requestId

idAlmacen

idRegistroProduccionDocumental

gabinete

usuario

fase

duracionMs

resultado (OK | FAILED | PARTIAL)

tipoEvento (STORAGE | COMPENSATION | RECONCILIATION)

```



Fases mínimas:



```txt

Validation

Metadata

Transaction

Physical

XML

Compensation

Reconciliation

Completed

Failed

```



---



### 2. MÉTRICAS OPERATIVAS



Registrar métricas:



```txt

total_operaciones_storage

total_operaciones_ok

total_fallos_fisicos

total_compensaciones_db

total_compensaciones_fs

total_compensaciones_failed

total_compensaciones_partial

total_reconciliaciones

```



Si existe sistema de métricas:



```txt

Prometheus / AppInsights / Logs estructurados

```



---



### 3. TABLA DE AUDITORÍA (INTEGRACIÓN)



Debe integrarse con:



```txt

ra_log_sotorage_compensacion

```



Estados válidos:



```txt

OK

PARTIAL

FAILED

```



Cada evento debe registrar:



```txt

requestId

idAlmacen

fase_error

acciones

resultado

duración

```



---



### 4. DETECCIÓN DE INCONSISTENCIAS (HUÉRFANOS)



Definir consultas operativas (ejemplos):



#### Huérfanos DB (sin archivo físico)



```sql

SELECT g.ID

FROM {gabinete} g

LEFT JOIN <ruta_fisica_resuelta> f ON ...

WHERE archivo_no_existe = 1;

```



#### Inventario sin documento



```sql

SELECT ID_REGISTRO_PRODUCCION_DOCUMENTAL

FROM registro_producion_documental

WHERE ID_DOCUMENTO_DOCUARCHI_ALMACEN NOT IN (...)

```



#### Índice expediente inconsistente



```sql

SELECT *

FROM ra_cert_indice_expediente

WHERE ruta_documento no corresponde a archivo real;

```



⚠️ Ajustar consultas a modelo real.



---



### 5. RECONCILIACIÓN (REINTENTO SEGURO)



Implementar servicio:



```txt

IStorageReconciliationService

StorageReconciliationService

```



Responsabilidades:



```txt

- identificar inconsistencias

- reconstruir operación si es posible

- ejecutar compensación si no es recuperable

- ser idempotente

```



Reglas:



```txt

- nunca duplicar efectos

- nunca reinsertar sin validar

- nunca sobrescribir archivos existentes

```



---



### 6. RUNBOOK OPERATIVO (OBLIGATORIO)



Crear procedimiento documentado para:



```txt

1. Identificar inconsistencias

2. Consultar logs por requestId

3. Determinar estado (OK/PARTIAL/FAILED)

4. Ejecutar reconciliación automática

5. Ejecutar compensación manual (si aplica)

6. Registrar cierre manual

```



Debe incluir:



```txt

- ejemplos reales

- queries

- pasos paso a paso

- criterios de decisión

```



---



### 7. ALERTAS OPERATIVAS



Definir alertas cuando:



```txt

- compensación FAILED > 0

- compensación PARTIAL > umbral

- fallos físicos consecutivos

- inconsistencias detectadas

```



---



### 8. INTEGRACIÓN CON ORCHESTRATOR



El Orchestrator debe:



```txt

- registrar evento de fallo físico

- invocar compensación

- registrar resultado

- permitir rastreo por requestId

```



---



### 9. DIAGRAMAS OBLIGATORIOS



Crear en documentación:



```txt

- secuencia: fallo físico → compensación → reconciliación

- flujo: OK / PARTIAL / FAILED

- arquitectura de observabilidad

- flujo de detección de huérfanos

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## REGLAS CRÍTICAS

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



```txt

- toda compensación debe ser auditable

- toda inconsistencia debe ser detectable

- toda reconciliación debe ser idempotente

- no ocultar errores

- no eliminar evidencia de fallos

- no ejecutar acciones destructivas sin trazabilidad

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PRUEBAS OBLIGATORIAS

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



### Unitarias



```txt

- logging correcto

- estados OK/PARTIAL/FAILED

- métricas incrementan correctamente

- reconciliación idempotente

```



### Integración



```txt

- fallo FS → log + compensación + registro

- fallo XML → log + compensación

- compensación parcial → estado PARTIAL

- consulta de huérfanos detecta inconsistencias

```



### Operativas



```txt

- ejecución de runbook

- reconciliación manual simulada

- verificación de auditoría

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBSERVABILIDAD

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Debe garantizar:



```txt

- correlación por requestId

- trazabilidad completa

- visibilidad de fallos

- métricas operativas

```



No loguear:



```txt

contenido documental

fulltext completo

datos sensibles

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DOCUMENTACIÓN OBLIGATORIA

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Actualizar en:



```txt

D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\Arquitectura-Final\

```



Crear:



```txt

SCRUM-189-Observabilidad-Compensacion.md

SCRUM-189-Runbook-Reconciliacion.md

SCRUM-189-Matriz-Riesgos-Residual.md

SCRUM-189-Diagramas-Reconciliacion.md

SCRUM-189-Metadata.md

```



Debe incluir:



```txt

- flujo completo de compensación

- flujo de reconciliación

- queries de diagnóstico

- ejemplos de incidentes

- decisiones operativas

- riesgos residuales

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CRITERIOS DE ACEPTACIÓN

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



```txt

✔ logs estructurados implementados

✔ métricas disponibles

✔ tabla de compensación integrada

✔ detección de inconsistencias operativa

✔ reconciliación implementada

✔ runbook documentado

✔ alertas definidas

✔ documentación SCRUM-189 actualizada

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RESTRICCIONES

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



```txt

- no modificar lógica funcional

- no alterar contratos API

- no eliminar compensación existente

- no ocultar errores

```



━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## INSTRUCCIÓN FINAL

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━



Implementar la capa de observabilidad y reconciliación del Storage Engine, asegurando trazabilidad completa, detección de inconsistencias y capacidad operativa de recuperación, dejando el sistema listo para operación productiva auditada.
