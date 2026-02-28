## Context

- Jira issue key: SCRUM-28
- Jira summary: ACTUALIZA-DAPER-DIASFERIADO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-28

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Agregar el parámetro string defaultDbAlias a la firma de la función SolicitaListaDiasFeriados. En los parámetros de QueryOptions de la función, agregar la propiedad DefaultAlias = defaultDbAlias. Actualizar todas las referencias de la función en el proyecto para que incluyan el nuevo parámetro defaultDbAlias. Mantener la función envuelta en try/catch para manejo controlado de errores. Retornar siempre envuelto en la clase AppResponses, respetando el contrato de respuesta (success, message, errorMessage, Data). Documentar la función con comentarios XML indicando propósito, parámetros y retorno. Validar que si defaultDbAlias es nulo , se retorne un error controlado (success=false, ErrorMessage="DefaultDbAlias requerido"). Incluir pruebas unitarias: Caso con defaultDbAlias válido y datos correctos. Caso con excepción simulada → success=false + errors. Incluir pruebas de integración con MySQL usando Testcontainers/Docker para validar que la consulta funciona con el alias de base de datos correcto. Mantener separación de responsabilidades (SoC), bajo acoplamiento y alta cohesión. Cumplir principios SOLID en la implementación.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-28-actualiza-daper-diasferiado.