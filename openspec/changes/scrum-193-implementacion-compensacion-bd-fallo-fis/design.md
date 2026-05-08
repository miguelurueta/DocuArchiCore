## Context

- Jira issue key: SCRUM-193
- Jira summary: IMPLEMENTACION-COMPENSACION-BD-FALLO-FISICO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-193

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 23D — Compensación lógica DB post-fallo físico (FASE CRÍTICA — CONSISTENCIA OPERATIVA POST-COMMIT) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: transacciones críticas y consistencia eventual compensaciones idempotentes Storage Engine DocuArchi Dapper / QueryOptions DapperCrudEngine auditoría técnica y trazabilidad recuperación ante fallos post-commit Clean Architecture ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Eliminar inconsistencias entre: Base de Datos (commit realizado)
vs
FileSystem / XML (fallo posterior) sin introducir estados intermedios tipo PENDIENTE_FISICO y sin rediseñar el flujo completo. Debe garantizar: ✔ Reversión lógica de efectos en DB ✔ No dejar registros huérfanos ✔ Mantener integridad operativa ✔ Ejecutar compensación idempotente ✔ Registrar auditoría completa de la compensación ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta: D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt :contentReference[oaicite:0]{index=0} REGLA La compensación es una mejora moderna.

No existe en legacy VB, pero NO debe romper:
- integridad funcional
- secuencia de negocio
- trazabilidad documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO DEL PROBLEMA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Estado actual: 1. DB hace commit (system1, gabinete, inventario, expediente, log)
2. Fase física falla (copy/XML)
3. Solo se limpian archivos (compensación física)
4. DB queda con registros inconsistentes ❌ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALCANCE ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar compensación lógica en DB tras fallo físico: 1. Revertir disco_detalle (NUMERO_IMAGENES, NUMPAG_CARP)
2. Eliminar registro en gabinete dinámico (por ID = idAlmacen)
3. Anular inventario (soft-delete según esquema existente)
4. Revertir folios expediente/unidad (si se actualizaron)
5. Eliminar registro logdocuarchi si se insertó
6. Mantener compensación física existente ⚠️ IMPORTANTE: NO revertir:
- system1.proxid
- system1.numcarp
- system1.NUMPAG_CARP ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Services MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Compensation/ Archivos: IStorageDbCompensationService.cs
StorageDbCompensationService.cs Models MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/ StorageCompensationDbPlan.cs
StorageCompensationDbResult.cs Repository Reusar: Disk/
Gabinete/
Inventario/
Expediente/
Workflow/
IndiceElectronico/ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ACCESO A DATOS (REGLA OBLIGATORIA) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - DapperCrudEngine es el mecanismo estándar de acceso a datos.
- EXCEPCIÓN EN ESTE PROMPT:

  Se permite el uso de Dapper directo debido a:
    - transacciones explícitas
    - operaciones con orden crítico
    - uso de SELECT ... FOR UPDATE
    - control de concurrencia
    - necesidad de idempotencia

- No mezclar DapperCrudEngine y Dapper directo en la misma operación.
- Cada operación debe documentar la estrategia usada. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ MODELO DE PLAN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Debe contener: IdAlmacen
IdRegistroProduccionDocumental
Gabinete
Disco
Carpeta
NumeroPaginas
TieneInventario
TieneExpediente
TieneUnidad
TieneWorkflow ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ TABLA DE AUDITORÍA (REFERENCIA) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Nombre: ra_log_sotorage_compensacion Campos sugeridos: id_log_compensacion
request_id
id_almacen
id_registro_produccion
nombre_gabinete
usuario
fecha_evento
fase_error
error_original
acciones_ejecutadas
resultado_compensacion
detalle_error_compensacion
duracion_ms ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ORDEN DE COMPENSACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 1. logdocuarchi
2. índice expediente
3. expediente/unidad
4. inventario
5. gabinete dinámico
6. disco_detalle ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS DE IMPLEMENTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - idempotente (ejecución múltiple segura)
- validar existencia antes de operar
- no fallar si registro no existe
- capturar errores parciales
- no ocultar error original ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS ESPECÍFICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Inventario - no hard delete si existe estado
- usar soft-delete si es posible
- documentar si no existe campo estado Gabinete DELETE WHERE ID = idAlmacen Disco_detalle revertir NUMERO_IMAGENES
revertir NUMPAG_CARP ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INTEGRACIÓN EN ORCHESTRATOR ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ try
{
    var txResult = await _transactionCoordinator.ExecuteAsync(...);
    var physical = await _physicalExecutor.ExecuteAsync(...);
}
catch (StoragePhysicalException ex)
{
    await _compensationManager.ExecuteAsync(planFisico);
    await _dbCompensationService.ExecuteAsync(planDb);
    throw;
} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO revertir proxid
- NO romper concurrencia
- SIEMPRE registrar compensación
- SIEMPRE mantener error original
- SI falla compensación → estado PARTIAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias - fallo FS → compensación DB
- fallo XML → compensación DB
- doble ejecución → OK
- registros inexistentes → OK
- error parcial → estado PARTIAL Integración - commit DB + fallo físico
- reversión disco_detalle
- eliminación gabinete
- inventario anulado
- reversión expediente/unidad
- eliminación logdocuarchi Regresión - no rompe flujo legacy
- no reutiliza proxid
- sistema consistente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ requestId
idAlmacen
fase_error
acciones
resultado
duración ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\Arquitectura-Final\ Crear: SCRUM-189-Arquitectura-Compensacion-DB.md
SCRUM-189-Implementacion-Compensacion-DB.md
SCRUM-189-Pruebas-Compensacion-DB.md
SCRUM-189-Observabilidad-Compensacion-DB.md
SCRUM-189-Metadata.md Diagramas obligatorios: - secuencia compensación
- flujo error → compensación
- clases del servicio
- estados (OK/PARTIAL/FAILED) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ no hay registros huérfanos
✔ compensación DB ejecutada
✔ idempotente
✔ no afecta system1
✔ logs registrados
✔ pruebas en verde
✔ documentación actualizada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - no crear tabla en este ticket
- no cambiar lógica legacy
- no introducir estado PENDIENTE_FISICO
- no cambiar API ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar compensación lógica DB post-fallo físico, garantizando consistencia operativa, trazabilidad completa y compatibilidad con el comportamiento legacy.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-193-implementacion-compensacion-bd-fallo-fis.