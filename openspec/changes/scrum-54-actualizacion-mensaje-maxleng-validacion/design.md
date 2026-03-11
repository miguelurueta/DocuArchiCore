## Context

- Jira issue key: SCRUM-54
- Jira summary: ACTUALIZACION-MENSAJE_MAXLENG-VALIDACION-CAMPOS-RADICACION
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-54

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito actualizar la validación de la API radicacion_entrante. Ubicación: Repositorio Api: DocuArchi.Api Ruta: /Controllers/Radicacion/Tramite Objetivo: Modificar el mensaje de error para el tipo de error maxleng. Dependencias: Servicio ValidaCamposRadicacion Repositorios asociados a radicación (Usuario, Plantilla, Radicados) Requerimientos funcionales: para el mensaje de validación Ejemplo: "Campo Fecha Límite Respuesta: mensaje de vaiidacion" El mensaje de validación  para el tipo de error maxleng: supera la longitud máxima permitida. Retornar la estructura envuelta en AppResponses con Success, Message, Data y Errors. Requerimientos adicionales: Mantener el patrón Controller → Service → Repository. Documentar todas las funciones con comentarios XML indicando propósito, parámetros y retorno. Validar que si no existen registros coincidentes, se retorne Success=true con Data=null y Message="Sin resultados". En caso de excepción, retornar Success=false con:

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-54-actualizacion-mensaje-maxleng-validacion.