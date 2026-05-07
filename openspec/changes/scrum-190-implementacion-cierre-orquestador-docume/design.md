## Context

- Jira issue key: SCRUM-190
- Jira summary: IMPLEMENTACION-CIERRE-ORQUESTADOR-DocumentStorageOrchestrator
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-190

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE PROMPT 23A — Cierre Core Runtime (Orchestrator + Pipeline + Transacción) (FASE CRÍTICA — BLOQUEANTE DE EJECUCIÓN REAL) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy migración VB → C# con paridad funcional orquestación de casos de uso críticos pipeline de validación transacciones MySQL de alta concurrencia Clean Architecture Dapper / QueryOptions observabilidad enterprise ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Cerrar la brecha principal: el sistema tiene componentes implementados pero el flujo no se ejecuta realmente en runtime. Debes dejar completamente operativo el flujo: UseCase 
→ Orchestrator 
→ ValidationPipeline 
→ MetadataAnalyzer 
→ TransactionCoordinator 
→ PhysicalPhaseExecutor 
→ Resultado final Y garantizar: - retorno real de IdAlmacen > 0 en éxito
- ejecución end-to-end sin stubs
- propagación correcta de errores funcionales y técnicos ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY (VALIDACIÓN OBLIGATORIA) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ El código legacy es la fuente de verdad funcional para validar el comportamiento del flujo ejecutado. Ruta: D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt Archivo cargado para esta validación: funcion-almacena-consolidad(2).txt :contentReference[oaicite:0]{index=0} REGLA DE PARIDAD Si existe discrepancia entre:
- implementación C#
- prompts previos
- documentación

Debe prevalecer el comportamiento del código legacy. USO DE ESTA REFERENCIA Este prompt NO debe reimplementar lógica legacy, pero SÍ debe: - validar el orden de ejecución del flujo
- validar condiciones de corte (errores)
- validar dependencias entre fases
- validar que no se omiten pasos funcionales
- validar que el flujo ejecutado coincide con VB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO DE BRECHA ACTUAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Brechas críticas confirmadas: - DocumentStorageOrchestrator contiene stub (Task.CompletedTask, IdAlmacen=0, Estado=PENDING)
- Componentes implementados pero NO conectados en runtime
- El sistema compila pero NO ejecuta almacenamiento real
- Existen rutas muertas y lógica no invocada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UBICACIÓN ESPERADA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Services MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/ Archivos clave: DocumentStorageOrchestrator.cs
Validation/StorageValidationPipeline.cs
Metadata/IStorageDocumentMetadataAnalyzer.cs
Transaction/StorageTransactionCoordinator.cs
Physical/StoragePhysicalPhaseExecutor.cs Models / Context MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/ StorageContext.cs
AlmacenarDocumentoResult.cs
StorageTransactionResult.cs
StoragePhysicalStatusModel.cs API / DI DocuArchi.Api/Program.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN REQUERIDA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 1. Eliminar stub del Orchestrator Eliminar completamente:
- Task.CompletedTask
- IdAlmacen = 0
- Estado = PENDING 2. Inyección obligatoria IStorageValidationPipeline
IStorageDocumentMetadataAnalyzer
IStorageTransactionCoordinator
IStoragePhysicalPhaseExecutor
IStorageCompensationManager (si aplica)
ILogger<DocumentStorageOrchestrator> 3. Flujo obligatorio 1. ValidationPipeline
2. MetadataAnalyzer
3. TransactionCoordinator
4. PhysicalPhaseExecutor
5. Resultado final 4. Validación - SIEMPRE primero
- Si falla → lanzar StorageValidationException
- NO continuar flujo 5. Metadata - Ejecutar antes de la transacción
- Poblar context.PhysicalMetadata
- No permitir valores vacíos 6. Transacción - Ejecutar solo si validación OK
- Obtener IdAlmacen real
- No continuar si falla 7. Fase física - Ejecutar solo después del commit DB
- Integrar CompensationManager si falla 8. Resultado final IdAlmacen > 0
IdRegistroProduccionDocumental
NombreArchivoFinal
Estado (COMPLETED / FAILED)
RequestId 9. Logging - requestId obligatorio
- logs por fase ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO dejar stubs activos
- NO retornar IdAlmacen = 0
- NO saltar validación
- NO ocultar errores
- NO mezclar capas
- NO introducir lógica nueva
- SOLO conectar y ejecutar correctamente lo existente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ AJUSTES ARQUITECTÓNICOS OBLIGATORIOS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Validar: - DI completo en Program.cs
- servicios registrados como Scoped
- no existen servicios sin inyección
- no existen builders/validators muertos Integrar: IStorageCompensationManager Agregar: Feature Flag: StorageEngineV2 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias - context null → error
- validation falla → no ejecuta transaction
- validation OK → ejecuta flujo completo
- metadata antes de transaction
- transaction falla → no ejecuta físico
- físico falla → lanza excepción
- éxito → IdAlmacen > 0
- no retorna PENDING Integración - flujo real desde UseCase
- respuesta exitosa real
- error de validación propagado
- error técnico propagado Validación contra legacy Comparar flujo ejecutado contra VB:
- orden de pasos
- validaciones
- puntos de error ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs obligatorios: requestId
nombreGabinete
usuarioId
idAlmacen
fase
duración total
errores Fases: Validation
Metadata
Transaction
Physical
Completed
Failed ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar en: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\ Ticket: SCRUM-189 Archivos: SCRUM-189-Arquitectura-Cierre-Core-Runtime.md
SCRUM-189-Implementacion-Detallada-Core-Runtime.md
SCRUM-189-Pruebas-Core-Runtime.md
SCRUM-189-Observabilidad-Core-Runtime.md
SCRUM-189-Metadata.md Debe incluir: - eliminación de stubs
- flujo real del Orchestrator
- diagrama de secuencia actualizado
- DI final
- evidencia de ejecución
- comparación con flujo VB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ Orchestrator sin stubs  
✔ Flujo real end-to-end  
✔ IdAlmacen > 0  
✔ Validación correcta  
✔ Metadata correcta  
✔ Transacción correcta  
✔ Fase física correcta  
✔ Manejo de errores correcto  
✔ Logs completos  
✔ DI validado  
✔ Pruebas en verde  
✔ Documentación SCRUM-189 actualizada  
✔ Flujo alineado con legacy VB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO cambiar lógica de negocio
- NO agregar nuevas reglas
- NO hardcodear valores
- NO omitir logs
- NO cerrar con stubs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar el cierre del flujo runtime del Storage Engine asegurando ejecución real end-to-end, validando contra el comportamiento legacy y dejando el sistema listo para la fase de paridad física/XML.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-190-implementacion-cierre-orquestador-docume.

## Correcciones de Trazabilidad

- Aunque el prompt referenciaba documentos bajo `SCRUM-189`, para este cambio la evidencia técnica y arquitectura deben publicarse bajo prefijo `SCRUM-190` para mantener consistencia Jira/OpenSpec/repositorio.
- La comparación contra la función legacy fuente es obligatoria y debe quedar evidenciada en tareas y documentación del ticket actual.
