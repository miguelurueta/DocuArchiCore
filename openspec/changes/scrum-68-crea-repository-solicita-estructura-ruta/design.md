## Context

- Jira issue key: SCRUM-68
- Jira summary: CREA-REPOSITORY-SOLICITA-ESTRUCTURA-RUTA-WORKFLOW
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-68

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Consulta Solicita estructura ruta workflow Rol esperado: Arquitecto de software senior y desarrollador backend .NET Nombre: SolicitaEstructuraRutaWorkflow OBJETIVO Implementar la consulta de de la estructura de la ruta workflow activa de radicación siguiendo el patrón Repository dentro de un proyecto multirepo con arquitectura por capas. Se debe consultar la configuración de la ruta usando:  defaultDbAlias Parámetros: - defaultDbAlias TABLA: rutas_workflow CONDICIONES Estado_Ruta = 1 --------------------------
 UBICACIÓN DE COMPONENTES
 -------------------------- Repositorio Proyecto: MiApp.Repository Ruta: /Repositorio/Workflow/RutaTrabajo Modelo Proyecto: MiApp.Models Ruta: Models/workflow/RutaTrabajo Documentación Repositorio: DocuArchiCore Ruta: /Docs/workflow/RutaTrabajo si no esta lo crea ---------------------------
 FIRMA ESPERADA DEL MÉTODO
 --------------------------- Task<AppResponses<rutasWorkflow>> SolicitaEstructuraRutaWorkflow(string defaultDbAlias) Si la tabla puede retornar múltiples registros válidos, usar: Task<AppResponses<List>> ----------------------------
 REQUERIMIENTOS FUNCIONALES
 ---------------------------- Implementar patrón: Repository La respuesta debe estar envuelta en: AppResponses La función debe estar envuelta en bloque try/catch. La consulta debe ser parametrizada para evitar SQL Injection. En QueryOptions agregar: DefaultAlias = defaultDbAlias Crear el modelo/mapeo de la tabla: rutas_workflow Si no existen registros: success = true data = null message = “Sin resultados” En caso de excepción: success = false message descriptivo errors o ErrorMessage = ex.Message -------------------------
 REQUERIMIENTOS TÉCNICOS
 ------------------------- Crear interfaz del repositorio en el mismo archivo que la clase si es la convención del proyecto. Registrar interfaz y clase en Program.cs. Documentar con comentarios XML: propósito de la clase propósito del método descripción de parámetros valor de retorno comportamiento cuando no hay resultados comportamiento ante excepciones Mantener nombres consistentes con el estándar del proyecto. Usar DapperCrudEngine + QueryOptions si es la base actual del repositorio. -----------------------
 DOCUMENTACIÓN TÉCNICA
 ----------------------- Ubicación: /Docs/workflow/RutaTrabajo Archivos: SCRUM-[ID]-Diagramas.md Debe incluir: - diagrama de casos de uso - diagrama de clases - diagrama de secuencia - diagrama de estados (si aplica) SCRUM-[ID]-Integracion-Frontend.md Debe incluir: - descripción técnica del DTO de respuesta - parámetros requeridos para invocar desde el servicio ----------------------------
 DOCUMENTAR CONSUMO INTERNO
 ---------------------------- Ejemplo de invocación: var result = await ISolicitaEstructuraRutaWorkflow .SolicitaEstructuraRutaWorkflow(  defaultDbAlias); ---------
 PRUEBAS
 --------- PRUEBAS UNITARIAS Incluir pruebas para: - respuesta exitosa con datos - respuesta exitosa sin resultados - excepción controlada - mock de repositorio o dependencias PRUEBAS DE INTEGRACIÓN Usar: MySQL Testcontainers / Docker Validar: - consulta real a la tabla - filtros correctos - comportamiento sin registros - comportamiento con registros válidos -------------
 ENTREGABLES
 ------------- Modelo de tabla rutas_workflow Interfaz y clase de repositorio Servicio que consume el repositorio Registro en Program.cs Comentarios XML completos Pruebas unitarias Pruebas de integración ---------------
 RESTRICCIONES
 --------------- No romper la arquitectura actual del proyecto. No usar SQL concatenado manualmente. Mantener consistencia con los patrones existentes. Usar AppResponses como wrapper estándar.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-68-crea-repository-solicita-estructura-ruta.