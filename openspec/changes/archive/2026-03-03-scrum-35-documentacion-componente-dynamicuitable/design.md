## Context

- Jira issue key: SCRUM-35
- Jira summary: DOCUMENTACION-COMPONENTE-DynamicUiTable-SCRUM-34
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-35

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

En el proyecto multirepo necesito crear documentación técnica del componente DynamicUiTableBuilder desarrollado en la tarea SCRUM-34. Ubicación: Repositorio DocuArchiCore /Docs/UI/MuiTable Archivos: SCRUM-34-Diagramas.md y SCRUM-34-Integracion-Frontend.md deben moverse a este directorio. Objetivo: Generar un documento que permita entender la funcionalidad del componente DynamicUiTableBuilder y todas sus funciones, incluyendo: Comentarios XML en las funciones principales. Documentación técnica del consumo del servicio desde un servicio interno. Ejemplo completo de ejecución con parámetros y retorno real basado en la tabla ra_rad_estados_modulo_radicacion. Requerimientos: Documentar todas las funciones públicas del componente con comentarios XML: Descripción del propósito. Parámetros y su origen. Retorno esperado. Documentar el consumo del servicio desde otro servicio interno: Especificar parámetros (ej. TableId, DefaultDbAlias, claims). Indicar de dónde provienen (backend, controlador, token). Especificar el retorno (AppResponses<DynamicUiTableDto> o AppResponses<DynamicUiRowsOnlyDto>). Incluir un ejemplo completo de cómo ejecutar el servicio: Request con parámetros enviados. Response con estructura real basada en la tabla ra_rad_estados_modulo_radicacion. Guardar la documentación en el repositorio DocuArchiCore /Docs/UI/MuiTable. Mover los archivos SCRUM-34-Diagramas.md y SCRUM-34-Integracion-Frontend.md a este directorio. Incluir diagramas de casos de uso, clases, secuencia y estado en SCRUM-34-Diagramas.md. Incluir documentación técnica de integración frontend en SCRUM-34-Integracion-Frontend.md: Descripción del DTO. Parámetros de envío a la API. Dirección de la API. Ejemplo de consumo desde React/MUI.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-35-documentacion-componente-dynamicuitable.