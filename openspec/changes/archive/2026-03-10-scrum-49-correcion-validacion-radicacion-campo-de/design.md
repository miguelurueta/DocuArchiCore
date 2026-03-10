## Context

- Jira issue key: SCRUM-49
- Jira summary: CORRECION-VALIDACION-RADICACION-CAMPO-Descripcion_Documento
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-49

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito actualizar la validación de la API radicacion_entrante. Ubicación: Repositorio Api: DocuArchi.Api Ruta: /Controllers/Radicacion/Tramite Objetivo: Corregir el error de mapeo entre el campo Descripcion_Documento y la estructura enviada por el backend: { "Descripcion": "DERECHOS DE PETECION", "tipo_doc_entrante": 1 } Actualmente el backend valida contra Descripcion_Documento, pero la estructura enviada es "Descripcion". Se debe alinear la validación para que coincida con la estructura real enviada por el backend. Dependencias: Servicio RegistrarRadicacionEntranteAsync en MiApp.Service /Service/Radicacion/Tramite Servicio ValidaCamposRadicacion Repositorios asociados a radicación (Usuario, Plantilla, Radicados) Requerimientos funcionales: Actualizar el DTO de entrada para que el campo sea "Descripcion" en lugar de "Descripcion_Documento". Ajustar el mapeo en AutoMapperProfile (ubicación: MiApp.Service /Service/Mapping/Radicacion/Tramite) para que: "Descripcion" → se mapee correctamente al campo de validación. "tipo_doc_entrante" → se mantenga como identificador del tipo de trámite. Actualizar la validación en ValidaCamposRadicacion para que use "Descripcion" como campo de referencia. Retornar AppResponses con Success, Message, Data y Errors según corresponda. Documentar la función con comentarios XML indicando propósito, parámetros y retorno. Requerimientos adicionales: Mnatener el patrón Controller → Service → Repository. y Data. Incluir pruebas unitarias: Caso Success con datos válidos (Descripcion="DERECHOS DE PETECION"). Caso sin resultados. Caso excepción simulada. Incluir pruebas de integración con MySQL usando Testcontainers/Docker. Mantener separación de responsabilidades (SoC), bajo acoplamiento y alta cohesión. Mantener principios SOLID en la implementación. Actualizar diagramas de casos de uso, clases, secuencia y estado si es necesario. Documentar pruebas funcionales. Actualizar documentación técnica de implementación para el frontend si es necesario: Descripción del DTO actualizado. Parámetros de envío a la API. Ejemplo de respuesta con radicado registrado.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-49-correcion-validacion-radicacion-campo-de.