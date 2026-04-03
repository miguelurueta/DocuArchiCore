## Context

- Jira issue key: SCRUM-124
- Jira summary: Exponer total real y conteo filtrado en workflowInboxgestion
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-124

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

El inbox workflow ya devuelve estructura consistente y claims reales, pero `Pagination.Total` sigue usando el conteo devuelto por la página actual. En la implementación base, `DapperCrudEngine.GetAllAsync` llena `TotalRecords` con `result.Data.Count()`, por lo que el endpoint no conoce el total real filtrado.

## Approach

- Mantener el contrato público: `Pagination.Total` seguirá siendo el único campo expuesto.
- Extender `QueryOptions` con una salida mínima para expresiones de selección seguras (`RawSelect`) usada solo por conteo backend controlado.
- Hacer que `WorkflowInboxQueryBuilder` exponga dos variantes:
  - query de datos con columnas, orden y paginación
  - query de conteo con `COUNT(1)`, sin `ORDER BY`, sin `LIMIT/OFFSET`
- Reutilizar exactamente los mismos filtros, joins, búsqueda y `StructuredFilters` en ambas variantes.
- Ejecutar ambas queries en `WorkflowInboxRepository` y propagar el total real al servicio.
- Validar con pruebas unitarias del builder, repository y service para búsqueda, filtros y página fuera de rango.
