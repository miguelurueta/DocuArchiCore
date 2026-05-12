## Context

- Jira issue key: SCRUM-200
- Jira summary: INTEGRACION-GABINETE-DINAMICO-INDICE-ELECTRONICO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-200

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — ORCHESTRATOR / ENGINE Integración completa de gabinete dinámico + índice electrónico lógico (FASE CRÍTICA — PARIDAD FUNCIONAL STORAGE ENGINE) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Master Backend .NET experto en: DocuArchi legacy Storage Engine gabinete dinámico documental expediente electrónico índice electrónico transacciones críticas compensación DB/FS/XML Clean Architecture Dapper / QueryOptions migración VB → C# con paridad funcional ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO GENERAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Proyecto: DocuArchiCore (.NET) Módulo: AlmacenamientoDocumental Ruta base: D:\imagenesda\GestorDocumental\DocuArchiCore ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REFERENCIA LEGACY OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ruta del código legacy: D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt :contentReference[oaicite:0]{index=0} REGLA DE PARIDAD Si existe discrepancia entre: - implementación C#
- prompts
- documentación Debe prevalecer: el comportamiento observado en legacy VB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DIAGNÓSTICO CONFIRMADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Estado actual: ✔ system1.PROXID se reserva
✔ disco_detalle se actualiza
✔ archivos DIG/FXL se crean
✔ expediente/unidad actualiza folios
✔ XML expediente intenta ejecutarse Pero: ❌ NO se inserta gabinete dinámico (ej. contabil)
❌ NO se inserta tabla lógica ra_cert_indice_expediente
❌ XML expediente queda sin prerequisitos lógicos
❌ NOMBRE_AREA_DEPARTAMENTO queda NULL
❌ SERIE_DOCUMENTO queda NULL
❌ SUBSERIE_DOCUMENTO queda NULL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Cerrar completamente la paridad funcional del Storage Engine: A) Insertar SIEMPRE el registro del gabinete dinámico: contabil
u otro gabinete dinámico dentro de la transacción principal. B) Integrar inserción lógica del expediente: ra_cert_indice_expediente cuando aplique expediente electrónico. C) Corregir el llenado descriptivo de TRD en: registro_producion_documental D) Mantener: - atomicidad
- compensación
- consistencia DB/FS/XML ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRINCIPIOS FUNCIONALES OBLIGATORIOS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Gabinete dinámico Es: OBLIGATORIO SIEMPRE porque representa: la entidad documental principal Inventario documental Es: OPCIONAL según reglas existentes. Expediente / índice electrónico Es: CONDICIONAL según: - expediente válido
- reglas de negocio
- estado expediente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA CRÍTICA DE IDENTIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IdAlmacen
=
ID del gabinete dinámico Debe mantenerse consistente en: gabinete
inventario
workflow
expediente
XML
compensación ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ARQUITECTÓNICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO hardcodear nombre de gabinete
- NO hardcodear columnas dinámicas
- reutilizar repos/servicios existentes
- mantener única transacción DB
- validaciones siguen en pipeline
- logging estructurado por requestId ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALCANCE TÉCNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Transaction Coordinator Archivo principal: MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Transaction/StorageTransactionCoordinator.cs Expediente MiApp.Services/.../Expediente/IndiceElectronicoBuilder.cs
MiApp.Services/.../Expediente/IndiceElectronicoCalculator.cs
MiApp.Repository/.../IndiceElectronico/IIndiceElectronicoRepository.cs Compensation MiApp.Services/.../Compensation/IStorageDbCompensationService.cs
MiApp.Repository/.../Compensation/IStorageDbCompensationRepository.cs ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — GABINETE DINÁMICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Integración obligatoria Inyectar: IGabineteStorageRepository y builders necesarios. Método obligatorio Crear: InsertarGabineteDinamicoAsync NO usar: Resolve
Helper
Manager Reglas Debe ejecutarse: ANTES del Commit() Si falla: rollback completo Campos obligatorios ID
DISC
PAG
DBT
IDEX
USER
DATE1
TIME1 Más: campos dinámicos del gabinete Regla DBT DBT
=
DA_EXTENSION.ESTADO_NORMAL NO: TRD.IdTipoDocumento TIME1 Debe poblarse SIEMPRE. NOT NULL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — INVENTARIO DOCUMENTAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Corregir el llenado descriptivo de TRD en: registro_producion_documental Campos obligatorios a corregir NOMBRE_AREA_DEPARTAMENTO
SERIE_DOCUMENTO
SUBSERIE_DOCUMENTO Reglas de mapeo NOMBRE_AREA_DEPARTAMENTO ← command.Trd.NombreArea
SERIE_DOCUMENTO          ← command.Trd.NombreSerie
SUBSERIE_DOCUMENTO       ← command.Trd.NombreSubSerie Regla obligatoria NO deben quedar NULL si existen: ID_AREA_DEPARTAMENTO
ID_SERIE_DOCUMENTO
ID_SUBSERIE_DOCUMENTO Fallback obligatorio Si el request no trae nombres descriptivos: resolver desde repositorio TRD / metadata equivalente ANTES del insert. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ IMPLEMENTACIÓN — ÍNDICE ELECTRÓNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Integración obligatoria Inyectar: IIndiceElectronicoRepository
IIndiceElectronicoBuilder
IIndiceElectronicoCalculator Reglas Insertar: ra_cert_indice_expediente SOLO si: - expediente válido
- IdRegistroProduccionDocumental > 0
- expediente activo
- reglas expediente cumplen PROHIBIDO insertar índice con expediente cerrado/inactivo Secuencia obligatoria 1. lock expediente
2. validar estado
3. calcular orden/folios
4. construir model
5. insertar índice ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ SECUENCIA TRANSACCIONAL FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Orden obligatorio: 1. ReserveAsync (system1)
2. Lock/Update disco_detalle
3. Insert gabinete dinámico
4. Insert inventario documental (si aplica)
5. Flujo expediente/unidad
6. Insert índice electrónico lógico
7. Insert workflow log
8. Commit ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLA DE ATOMICIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ gabinete
inventario
expediente
índice electrónico
workflow deben pertenecer a: LA MISMA TRANSACCIÓN DB ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ COMPENSACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Ante fallo físico post-commit: - borrar gabinete dinámico
- anular inventario
- eliminar workflow log
- revertir expediente/unidad
- revertir índice electrónico Debe seguir patrón existente. Índice electrónico Evaluar: delete
o
marca de anulación según política existente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ VALIDACIONES OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DA_EXTENSION Debe: - aceptar PDF y .PDF
- case-insensitive
- no hardcodear Metadata dinámica Debe validar: - longitud
- tipo
- nullability usando metadata existente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs mínimos: requestId
idAlmacen
gabinete
idRegistroProduccion
idIndiceElectronico
fase
duración Logs obligatorios: inicio insert gabinete
fin OK gabinete
inicio índice expediente
fin OK índice
error rollback NO loguear: fulltext
contenido documental
datos sensibles ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ UNITARIAS — GABINETE Validar: - contabil insertado
- ID = IdAlmacen
- DBT correcto
- TIME1 poblado UNITARIAS — INVENTARIO / TRD Validar: - NOMBRE_AREA_DEPARTAMENTO no NULL
- SERIE_DOCUMENTO no NULL
- SUBSERIE_DOCUMENTO no NULL UNITARIAS — FALLBACK TRD Validar: - request sin nombres descriptivos
- resolución desde repositorio TRD UNITARIAS — ÍNDICE ELECTRÓNICO Validar: - expediente válido → insert OK
- expediente inactivo → bloquea
- IdRegistroProduccion null → NO inserta QT / CALIDAD — ATOMICIDAD Validar: - fallo insert gabinete → rollback total
- fallo índice electrónico → rollback total
- fallo físico post-commit → compensación QT / CALIDAD — COMPLETITUD DOCUMENTAL Validar: - existe fila gabinete dinámico
- existe DIG
- existe FXL
- existe índice electrónico si aplica
- existe inventario si aplica QT / CALIDAD — NO REGRESIÓN Validar: - flujo sin expediente funciona
- flujo sin inventario funciona
- naming DIG/FXL intacto
- DA_EXTENSION sigue gobernando DBT QT / CALIDAD — TRAZABILIDAD Validar: - requestId presente
- idAlmacen presente
- logs estructurados QT / CALIDAD — DATOS OBLIGATORIOS Validar: TIME1 poblado
DBT correcto
DISC correcto
PAG correcto
IDEX correcto INTEGRACIÓN — HAPPY PATH Validar: - PROXID incrementa
- contabil insertado
- disco_detalle consistente
- DIG/FXL creados INTEGRACIÓN — EXPEDIENTE Validar: - expediente actualizado
- ra_cert_indice_expediente insertado
- XML expediente actualizado INTEGRACIÓN — FALLOS Validar: - fallo insert gabinete → rollback total
- fallo físico post-commit → compensación
- no registros huérfanos REGRESIÓN Validar: - naming DIG/FXL intacto
- workflow intacto
- inventario intacto
- DA_EXTENSION intacto ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar documentación en: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\Arquitectura-Final\ ⚠️ IMPORTANTE: Tomar como base los documentos existentes de: SCRUM-189 y ACTUALIZARLOS. NO crear documentación aislada inconsistente. Actualizar mínimo SCRUM-189-Arquitectura-Cierre-Integral.md
SCRUM-189-Implementacion-Detallada-Core-Runtime.md
SCRUM-189-Pruebas-E2E-StorageEngine.md
SCRUM-189-Observabilidad-Core-Runtime.md
SCRUM-189-Matriz-Paridad-StorageEngine.md
SCRUM-189-Metadata.md Crear documentación propia del cambio SCRUM-[ID]-Arquitectura-Gabinete-Dinamico-Indice-Electronico.md
SCRUM-[ID]-Implementacion-Gabinete-Dinamico-Indice-Electronico.md
SCRUM-[ID]-Pruebas-Gabinete-Dinamico-Indice-Electronico.md
SCRUM-[ID]-Observabilidad-Gabinete-Dinamico-Indice-Electronico.md
SCRUM-[ID]-Regresion-Legacy-Gabinete-Dinamico-Indice-Electronico.md
SCRUM-[ID]-Metadata.md Debe incluir - secuencia transaccional actualizada
- tablas afectadas por fase
- gabinete dinámico
- índice electrónico lógico
- estrategia compensación
- guía depuración requestId
- diagramas actualizados
- reglas de TRD descriptivo
- origen de DBT ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ contabil insertado correctamente
✔ ID = IdAlmacen
✔ system1 consistente
✔ disco_detalle consistente
✔ DIG y FXL existentes
✔ expediente actualizado
✔ índice electrónico insertado
✔ XML expediente coherente
✔ NOMBRE_AREA_DEPARTAMENTO poblado
✔ SERIE_DOCUMENTO poblado
✔ SUBSERIE_DOCUMENTO poblado
✔ pruebas QT ejecutadas
✔ sin regresiones
✔ documentación SCRUM-189 actualizada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO hardcodear gabinetes
- NO romper flujo sin expediente
- NO romper naming
- NO romper DA_EXTENSION
- NO crear transacciones separadas
- NO cambiar contratos HTTP ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ESPERADOS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 1. lista de archivos modificados
2. decisiones técnicas
3. resultado de pruebas
4. riesgos residuales
5. guía operativa:
   mantenimiento DA_EXTENSION ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Implementar la integración completa del gabinete dinámico, inventario descriptivo TRD e índice electrónico lógico dentro de la transacción principal del Storage Engine, garantizando paridad funcional con legacy VB, atomicidad y consistencia entre DB, FileSystem y XML.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-200-integracion-gabinete-dinamico-indice-ele.