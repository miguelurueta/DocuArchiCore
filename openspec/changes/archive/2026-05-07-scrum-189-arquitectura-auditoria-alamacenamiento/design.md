## Context

- Jira issue key: SCRUM-189
- Jira summary: ARQUITECTURA-AUDITORIA-ALAMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-189

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — DOCUMENTACIÓN Y AUDITORÍA INTEGRAL Storage Engine / Función Almacenamiento DocuArchi.Core ROL ESPERADO Actúa como Arquitecto Master de Software .NET, especialista en sistemas ECM/SGDEA, DocuArchi legacy, migración VB → C#, Clean Architecture, Orchestrator/Engine, Dapper, MySQL, FileSystem, XML, transacciones críticas, pruebas de paridad y auditoría técnica enterprise. OBJETIVO Generar un documento integral de arquitectura, auditoría técnica y validación del nuevo Storage Engine , construido para reemplazar la función legacy Almacenamiento . El documento debe servir como referencia formal para: arquitectura del sistema mapa funcional del código trazabilidad de módulos paridad legacy VB vs C# plan de pruebas validación Go/No-Go mejoras pendientes prevención de regresiones Ruta esperada: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\Arquitectura-Final Si no existe crea el directorio. ALCANCE Documentar el desarrollo completo asociado a la función Almacenamiento , incluyendo como mínimo: Prompt 2 — DTOs y Models Prompt 3 — UseCase + Orchestrator Prompt 4 — ValidationPipeline Prompt 5 — IdentityAllocator + SystemStorageRepository Prompt 6 — StorageTransactionCoordinator Prompt 7 — Gabinete + Inventario Prompt 8 — Expediente + Unidad + Índice Electrónico Prompt 9 — FileSystem + XML + CompensationManager Prompt 10 — Cierre completo + API + E2E Prompt 11 — Metadata Gabinete + Campos Obligatorios Prompt 12 — Preindex Prompt 13 — Naming DIG/FXL Prompt 14 — Ruta física SYSTEM1RUT Prompt 15 — Opciones legacy system1 Prompt 16 — Tamaño real + conteo de páginas Prompt 17 — Inventario legacy-compatible Prompt 18 — Expediente/unidad legacy-compatible Prompt 19 — XML índice expediente Prompt 20 — Logdocuarchi workflow Prompt 21 — Pruebas de paridad VB vs C# CONTEXTO ARQUITECTÓNICO El sistema debe documentarse bajo el patrón: Controller
  → UseCase
    → DocumentStorageOrchestrator
      → ValidationPipeline
      → MetadataAnalyzer
      → NamingService
      → PhysicalPathService
      → StorageTransactionCoordinator
        → IdentityAllocator
        → Repositories
      → FileSystemWriter
      → XmlWriter
      → CompensationManager No documentar como CRUD simple. 1. DOCUMENTO DE REQUISITOS DEL SISTEMA Incluir: requisitos funcionales requisitos no funcionales restricciones técnicas restricciones de negocio reglas legacy heredadas reglas nuevas de seguridad criterios de paridad funcional Debe incluir requisitos específicos: reserva de proxid bloqueo FOR UPDATE IsolationLevel.Serializable naming DIG y FXL rutas reales SYSTEM1RUT metadata de gabinete preindex inventario documental expediente/unidad XML índice expediente logdocuarchi compensación post-commit 2. DIAGRAMAS UML OBLIGATORIOS Generar diagramas Mermaid: clases secuencia componentes casos de uso estados despliegue lógico flujo transaccional flujo de compensación 3. ESPECIFICACIONES FUNCIONALES Y NO FUNCIONALES Por cada módulo documentar: responsabilidad entradas salidas dependencias errores posibles validaciones pruebas asociadas Módulos mínimos: Controller UseCase Orchestrator ValidationPipeline Preindex Metadata Gabinete Options system1 Metadata física Naming Ruta física TransactionCoordinator IdentityAllocator GabineteRepository InventarioRepository Expediente/Unidad Índice electrónico FileSystem XML FXL XML índice expediente CompensationManager Logdocuarchi Parity Tests 4. MODELO ENTIDAD-RELACIÓN Documentar tablas: system1 system1rut disco_detalle tabla dinámica del gabinete gabinete_detalle / DETALLE_GABIENETE DA_EXTENSION registro_producion_documental expediente_archivo unidad_conservacion ra_cert_indice_expediente logdocuarchi Para cada tabla incluir: propósito PK FK si aplica campos usados operaciones realizadas relación con prompts riesgos de integridad 5. ARQUITECTURA LÓGICA Documentar: capas proyectos rutas físicas de código patrón Orchestrator / Engine patrón Repository patrón Pipeline patrón Compensation / Saga separación DB vs FileSystem/XML feature flag StorageEngineV2 6. INTEGRACIONES ENTRE MÓDULOS Incluir: interfaces contratos APIs inyección de dependencias flujo Controller → Engine flujo Engine → DB flujo Engine → FS/XML flujo de compensación flujo de observabilidad 7. CASOS DE PRUEBA Incluir: unitarios integración E2E concurrencia regresión legacy paridad VB vs C# Escenarios mínimos: almacenamiento simple batch preindex TXT batch preindex XMLS inventario activo TRD activa expediente electrónico unidad digitalizada unidad electrónica workflow activo fallo DB fallo FileSystem fallo XML concurrencia 8. PLAN DE VALIDACIÓN Debe incluir: estrategia de pruebas herramientas fixtures Testcontainers / MySQL / Docker validación de archivos validación XML validación DB matriz Go/No-Go criterios de aceptación criterios de bloqueo 9. AUDITORÍA DE CÓDIGO — INVENTARIO DE FUNCIONES Generar tabla: Archivo | Clase | Función | Parámetros | Retorno | Invoca | Invocado por | Prompt asociado | Estado | Observación No omitir: interfaces services builders validators repositories policies writers compensation controllers tests 10. MAPA DE INTERACCIONES Documentar quién invoca a quién: Controller → UseCase
UseCase → Orchestrator
Orchestrator → ValidationPipeline
Orchestrator → TransactionCoordinator
TransactionCoordinator → IdentityAllocator
TransactionCoordinator → Repositories
Orchestrator → FileWriter
Orchestrator → XmlWriter
Orchestrator → CompensationManager 11. MATRIZ DE PARIDAD LEGACY Crear tabla: Comportamiento VB | Implementación C# | Prompt asociado | Estado | Evidencia | Brecha | Acción Estados: CUMPLE
CUMPLE CON MEJORA
PARCIAL
NO CUMPLE
PENDIENTE VALIDACIÓN 12. ATOMICIDAD Y CONSISTENCIA Documentar explícitamente: DB → atomicidad ACID
FileSystem/XML → compensación post-commit
Sistema completo → consistencia eventual controlada Incluir: commit rollback compensation escenarios de inconsistencia plan de recuperación riesgos residuales 13. OBSERVABILIDAD Documentar: logs requestId alias usuarioId idAlmacen idRegistroProduccion fases duración errores eventos críticos 14. AUTOMATIZACIONES PENDIENTES Identificar: pruebas faltantes validaciones faltantes documentación faltante redundancias código duplicado placeholders riesgos de regresión deuda técnica 15. RECOMENDACIONES DE ARQUITECTURA Incluir: mejoras prioritarias riesgos bloqueantes refactorizaciones sugeridas optimización de trazabilidad endurecimiento de seguridad estrategia de activación gradual recomendación Go/No-Go ENTREGABLES Crear documentación en: Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/ Archivos: SCRUM-[ID]-Arquitectura-Integral-StorageEngine.md
SCRUM-[ID]-Auditoria-Codigo-StorageEngine.md
SCRUM-[ID]-Modelo-ER-StorageEngine.md
SCRUM-[ID]-Diagramas-StorageEngine.md
SCRUM-[ID]-Plan-Pruebas-Validacion.md
SCRUM-[ID]-Matriz-Paridad-Legacy.md
SCRUM-[ID]-Riesgos-Deuda-Tecnica.md
SCRUM-[ID]-Go-NoGo-Produccion.md CRITERIOS DE ACEPTACIÓN Documento integral generado. Diagramas completos. ERD completo. Inventario de funciones completo. Matriz de paridad completa. Plan de pruebas completo. Riesgos identificados. Automatizaciones pendientes documentadas. Recomendación Go/No-Go incluida. No se omiten detalles críticos del código. Se mantiene trazabilidad con prompts 2 al 21. INSTRUCCIÓN FINAL Analiza el código implementado y genera la documentación integral solicitada. No lo trates como CRUD. No omitas funciones. No omitas interacciones. No omitas brechas frente al legacy. El resultado debe servir como documento maestro de arquitectura, auditoría técnica y validación del Storage Engine.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-189-arquitectura-auditoria-alamacenamiento.