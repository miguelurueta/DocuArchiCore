# SCRUM-189 - Arquitectura Cierre Integral

## Objetivo
Emitir el cierre técnico integral del Storage Engine con enfoque de auditoría (paridad VB -> C#, evidencia y decisión de despliegue), sin introducir funcionalidad nueva.

## Fuente de Verdad
- Legacy funcional: `D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt`
- OpenSpec de evolución: `SCRUM-177` a `SCRUM-191`

## Alcance Auditado
- Runtime: `AlmacenarDocumentoUseCase` y `DocumentStorageOrchestrator`.
- Validación: pipeline + validadores de metadata/preindex/opciones/TRD/expediente.
- Identidad/transacción: `StorageIdentityAllocator` + `StorageTransactionCoordinator`.
- Persistencia: system1/disco_detalle/gabinete/inventario/expediente/indice/log.
- Fase física: ruta legacy, naming, copia de archivos, XML FXL, XML índice expediente.
- Resiliencia: `StorageCompensationManager`.
- Gobernanza: DI y feature flag `StorageEngineV2`.

## Flujo Arquitectónico Validado
1. Controller API valida claims y feature flag.
2. UseCase construye `StorageContext` con `requestId`.
3. Orchestrator ejecuta:
- ValidationPipeline
- MetadataAnalyzer
- TransactionCoordinator
- PhysicalPhaseExecutor
4. Si falla fase física post-commit, se ejecuta compensación.

## Hallazgos Arquitectónicos
- Paridad funcional principal implementada en componentes críticos de storage.
- Separación correcta DB vs FileSystem/XML.
- Observabilidad base presente por `requestId`, `idAlmacen` y fases.
- Existe brecha de rollback legacy por feature flag: si `StorageEngineV2=false`, hoy retorna error controlado y no fallback operativo a adapter legacy.

## Resultado Arquitectónico
- Estado global de cierre: **GO CONDICIONADO**.
- Razón: no hay brechas críticas abiertas en el flujo principal V2, pero sí riesgos altos/medios de gobernanza operativa (fallback legacy y cobertura E2E de Docker pendiente).

## Evolución SCRUM-193 (post-cierre)
- Se agregó compensación DB post-fallo físico en `DocumentStorageOrchestrator` como manejo específico de `StoragePhysicalException` luego de `commit`.
- Se formalizó `StorageCompensationDbPlan/Result` para trazabilidad y ejecución determinística.
- Se incorporó repositorio de compensación DB para revertir cuota disco, anular inventario, limpiar workflow log y eliminar registro dinámico del gabinete.
- Se conservó la excepción física original como error raíz para no ocultar el incidente operativo.
- Se actualizó el diagrama integral con la secuencia específica de compensación en:
  - `SCRUM-189-Diagramas-StorageEngine.md`
  - `PlantUML/SCRUM-189/Sequence-SCRUM-193-DbCompensation-PostPhysicalFailure.puml`
