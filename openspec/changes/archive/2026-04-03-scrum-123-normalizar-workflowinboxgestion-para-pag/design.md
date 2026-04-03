## Context

- Jira issue key: SCRUM-123
- Jira summary: Normalizar workflowInboxgestion para paginacion consistente y claims reales
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-123

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

El inbox workflow usa una ruta backend que combina contexto, metadata de columnas, consulta SQL dinamica y construccion de `DynamicUiTableDto`. El contrato publicado al frontend tenia tres desviaciones:

1. `data = null` cuando la consulta no retornaba filas.
2. `PageSize` publicado sin respetar la prioridad real de `NumeroTareaLista`.
3. ausencia de `UserClaims` reales en el payload final.

## Approach

- Mantener el patron actual `Service + Repository + AppResponses + try/catch` definido en `openspec/context/OPSXJ_BACKEND_RULES.md`.
- Extender la respuesta del repositorio con un DTO interno que preserve `Rows` y `TotalRecords`.
- Normalizar en `WorkflowInboxService` el `Page` y el `PageSize` con la misma logica efectiva de consulta.
- Construir siempre la tabla final, incluso para listas vacias.
- Propagar `_currentUserService.Permisos` al builder de `DynamicUiTableDto`.
- Validar con pruebas unitarias enfocadas en:
  - sin filas
  - claims propagados
  - page size efectivo por `NumeroTareaLista`
  - total records preservado desde repositorio
