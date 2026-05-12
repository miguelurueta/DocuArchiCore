## Context

- Jira issue key: SCRUM-198
- Jira summary: IMPLEMENTACION-CORRECION-DBNULL-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-198

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — STORAGE ENGINE Corrección de manejo de null vs DBNull en flujo transaccional de inventario (FASE CRÍTICA — CORRECCIÓN QUIRÚRGICA SIN CAMBIO FUNCIONAL) ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ROL ESPERADO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actúa como Arquitecto Backend .NET senior experto en: Dapper DapperCrudEngine Storage Engine DocuArchi transacciones críticas Clean Architecture separación dominio / infraestructura migración VB → C# correcciones quirúrgicas sin regresión ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CONTEXTO DEL PROBLEMA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ El flujo transaccional de almacenamiento documental falla con: InsertAsync error:
The member SERIE_DOCUMENTO of type System.DBNull cannot be used as a parameter value La causa es: uso incorrecto de DBNull.Value dentro de capas de servicio/repositorio especialmente en: Dictionary<string, object>
DynamicParameters
estructuras de parámetros intermedias En Dapper: null es válido
DBNull.Value NO debe usarse manualmente ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBJETIVO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Corregir el manejo de valores nulos siguiendo principios arquitectónicos: ✔ usar null de C# para ausencia de datos ✔ eliminar DBNull.Value de capas de dominio/servicio/repositorio ✔ dejar DBNull solo en frontera si fuese estrictamente necesario ✔ mantener separación de responsabilidades ✔ no alterar reglas funcionales del Storage Engine ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRINCIPIO ARQUITECTÓNICO OBLIGATORIO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DBNull.Value pertenece a infraestructura ADO.NET. En el Storage Engine: - dominio
- servicios
- repositories
- diccionarios
- parámetros Dapper

DEBEN usar null. Regla: Dapper debe recibir null directamente. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ALCANCE TÉCNICO ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Revisar y corregir: MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Inventario/IInventarioDocumentalRepository.cs y cualquier bloque relacionado donde se armen: - Dictionary<string, object>
- ReglasValidacionCampo
- CampoParameterRegla
- DynamicParameters Reemplazar patrones incorrectos ["SERIE_DOCUMENTO"] =
    condicion ? DBNull.Value : valor por: ["SERIE_DOCUMENTO"] =
    condicion ? null : valor Aplicar a campos opcionales relacionados Ejemplos mínimos: SERIE_DOCUMENTO
ID_SERIE_DOCUMENTO
SUBSERIE_DOCUMENTO
ID_SUBSERIE_DOCUMENTO
DESCRIPCION_TIPO_DOCUMENTO
ID_TIPO_DOCUMENTO
NOMBRE_AREA_DEPARTAMENTO
EXPEDIENTE
UNIDADCONSERVA Sin alterar reglas funcionales. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REVISIÓN OBLIGATORIA DEL MOTOR DAPPER ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Revisar: MiApp.Repository/Repositorio/DataAccess/DapperCrudEngine.cs Especialmente: InsertBeginTrandAsync Objetivos: - validar soporte correcto de null
- NO introducir lógica de negocio
- NO transformar null → DBNull.Value manualmente
- mejorar mensajes de error confusos Ejemplo: "InsertAsync error" Debe incluir: - tabla
- operación
- requestId si aplica
- parámetro problemático ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ BÚSQUEDA GLOBAL OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Realizar búsqueda global en: MiApp.Services/
MiApp.Repository/
MiApp.Models/ Patrones: DBNull.Value
Convert.DBNull
System.DBNull Objetivo: identificar usos indebidos dentro de Storage Engine No corregir fuera del alcance sin documentarlo. ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS ARQUITECTÓNICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - ValidationPipeline mantiene validaciones de negocio
- Repository solo traduce modelo → persistencia
- No duplicar validaciones funcionales
- No mover reglas de negocio al repositorio
- No introducir lógica condicional innecesaria ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ REGLAS CRÍTICAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO usar DBNull.Value en:
    - diccionarios
    - DynamicParameters
    - modelos
    - builders
    - servicios
    - repositories

- SIEMPRE usar null en C#

- NO romper:
    - TransactionCoordinator
    - InventarioDocumentalRepository
    - DapperCrudEngine
    - flujo transaccional actual ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ PRUEBAS OBLIGATORIAS ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Unitarias — Repository Validar: - SERIE_DOCUMENTO = null → insert OK
- SUBSERIE_DOCUMENTO = null → insert OK
- ID_SERIE_DOCUMENTO = null → insert OK
- combinaciones múltiples null → insert OK Unitarias — DapperCrudEngine Validar: - null en parámetros → OK
- DBNull.Value no requerido
- mensajes de error mejorados Integración — Storage Engine Validar: - flujo transaccional completo
- inventario con campos opcionales null
- rollback correcto
- commit correcto Regresión Validar: - no cambia comportamiento funcional
- no rompe inserts válidos existentes
- no rompe prompts previos del Storage Engine ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ OBSERVABILIDAD ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Logs mínimos: requestId
tabla
operación
campo opcional null
resultado
duración No loguear: datos sensibles
fulltext
contenido documental ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ DOCUMENTACIÓN TÉCNICA OBLIGATORIA ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Actualizar: D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\AlmacenamientoDocumental\Arquitectura-Final\ Archivos: SCRUM-189-Correccion-Null-DbNull-StorageEngine.md
SCRUM-189-Pruebas-Null-DbNull-StorageEngine.md
SCRUM-189-Metadata.md Debe incluir: - política oficial null vs DBNull
- capas permitidas para DBNull
- impacto de la corrección
- evidencia de pruebas
- riesgos residuales ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ CRITERIOS DE ACEPTACIÓN ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ✔ no vuelve a aparecer error DBNull
✔ inserts funcionan con campos opcionales null
✔ Dapper recibe null correctamente
✔ no existen DBNull.Value indebidos
✔ pruebas en verde
✔ sin regresiones funcionales
✔ documentación actualizada ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ RESTRICCIONES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - NO cambiar contratos HTTP
- NO cambiar reglas funcionales
- NO reescribir DapperCrudEngine innecesariamente
- NO introducir lógica de negocio nueva
- cambios mínimos, quirúrgicos y trazables ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ENTREGABLES ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ - lista de archivos modificados
- resumen de cambios por archivo
- resultado de pruebas ejecutadas
- nota de riesgos residuales ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ INSTRUCCIÓN FINAL ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ Corregir el manejo de null vs DBNull en el flujo transaccional del Storage Engine garantizando compatibilidad con Dapper, estabilidad transaccional y ausencia de regresiones funcionales.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-198-implementacion-correcion-dbnull-almacena.