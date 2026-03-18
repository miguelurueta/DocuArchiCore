## Context

- Jira issue key: SCRUM-74
- Jira summary: CREA-REPOSITORY-SOLICITA-ESTRUCTURA-CONFIGURACION-PLANTILLA
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-74

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

PROMPT ARQUITECTÓNICO — Consulta Solicita estructura cofiguración plantilla de radicación Rol esperado: Arquitecto de software senior y desarrollador backend .NET Nombre: SolicitaListaEstructuraConfiguracionPlantillaRadicacionRespository OBJETIVO Implementar la consulta de configuración de plantilla de radicación siguiendo el patrón Repository dentro de un proyecto multirepo con arquitectura por capas. Parámetros: - idPlantilla - defaultDbAlias TABLA: ra_rad_config_plantilla_radicacion CONDICIONES system_plantilla_radicado_id_Plantilla = idPlantilla --------------------------
 UBICACIÓN DE COMPONENTES
 -------------------------- Repositorio Proyecto: MiApp.Repository Ruta: /Repositorio/radicador/PlantillaRadicado Modelo Proyecto: MiApp.Models Ruta: Models/Radicacion/PlantillaRadicado Documentación Repositorio: DocuArchiCore Ruta: /Docs/Radicacion/PlantillaRadicado ---------------------------
 FIRMA ESPERADA DEL MÉTODO
 --------------------------- Task<AppResponses<raradConfigPlantillaradicacion?> SolicitaListaEstructuraConfiguracionPlantillaRadicacionRespository( int idPlantilla, string defaultDbAlias) Si la tabla puede retornar múltiples registros válidos, usar: Task<AppResponses<List>> ----------------------------
 REQUERIMIENTOS FUNCIONALES
 ---------------------------- Implementar patrón: Repository La respuesta debe estar envuelta en: AppResponses La función debe estar envuelta en bloque try/catch. La consulta debe ser parametrizada para evitar SQL Injection. En QueryOptions agregar: DefaultAlias = defaultDbAlias Si no existen registros: success = true data = null message = “Sin resultados” En caso de excepción: success = false message descriptivo errors o ErrorMessage = ex.Message -------------------------
 REQUERIMIENTOS TÉCNICOS
 ------------------------- Crear interfaz del repositorio en el mismo archivo que la clase si es la convención del proyecto. Registrar interfaz y clase en Program.cs. Documentar con comentarios XML: propósito de la clase propósito del método descripción de parámetros valor de retorno comportamiento cuando no hay resultados comportamiento ante excepciones Mantener nombres consistentes con el estándar del proyecto. Usar DapperCrudEngine + QueryOptions si es la base actual del repositorio. No implmentarservicio para esta funcion. No implementar Api para eta funcion. -----------------------
 DOCUMENTACIÓN TÉCNICA
 ----------------------- Ubicación: /Docs/Radicacion/PlantillaRadicado Archivos: SCRUM-[ID]-Diagramas.md Debe incluir: - diagrama de casos de uso - diagrama de clases - diagrama de secuencia - diagrama de estados (si aplica) SCRUM-[ID]-Integracion-Frontend.md (si aplica) Debe incluir: - descripción técnica del DTO de respuesta - parámetros (si aplica) DOCUMENTAR CONSUMO INTERNO Ejemplo de invocación PRUEBAS UNITARIAS Incluir pruebas para: - respuesta exitosa con datos - respuesta exitosa sin resultados - excepción controlada - mock de repositorio o dependencias PRUEBAS DE INTEGRACIÓN Usar: MySQL Testcontainers / Docker Validar: - consulta real a la tabla - filtros correctos - comportamiento sin registros - comportamiento con registros válidos -------------
 ENTREGABLES
 ------------- Modelo de tabla ra_rad_config_plantilla_radicacion Interfaz y clase de repositorio Servicio que consume el repositorio Registro en Program.cs Comentarios XML completos Pruebas unitarias Pruebas de integración ---------------
 RESTRICCIONES
 --------------- No romper la arquitectura actual del proyecto. No usar SQL concatenado manualmente. Mantener consistencia con los patrones existentes. Usar AppResponses como wrapper estándar.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-74-crea-repository-solicita-estructura-conf.