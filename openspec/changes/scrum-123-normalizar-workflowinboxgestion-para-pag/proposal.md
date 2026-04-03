## Why

`workflowInboxgestion` estaba respondiendo de forma inconsistente para la misma consulta:

- cuando no habia filas, el servicio devolvia `data = null` en lugar de una `DynamicUiTableDto` consistente
- la paginacion publicada por el payload no reflejaba el `PageSize` efectivo usado por la consulta
- el payload final no propagaba los claims reales del usuario autenticado

Esto rompe contratos del frontend y dificulta reglas de visibilidad basadas en claims.

## What Changes

- Normalizar la salida de `WorkflowInboxService` para que siempre construya `DynamicUiTableDto`, incluso sin resultados.
- Ajustar la respuesta intermedia del repositorio para transportar `Rows` y `TotalRecords`.
- Propagar `ICurrentUserService.Permisos` al request que construye `DynamicUiTableDto`.
- Calcular `Page` y `PageSize` con la misma logica efectiva del inbox workflow:
  - `Page >= 1`
  - `NumeroTareaLista` tiene prioridad cuando exista
  - si no hay `PageSize` explicito, usar `1000`
- Cubrir el cambio con pruebas unitarias del servicio y del repositorio.

## Capabilities

### Modified Capabilities
- jira-scrum-123: Normalizacion del contrato `workflowInboxgestion` para paginacion consistente y claims reales.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-123
- OpenSpec change path: openspec/changes/scrum-123-normalizar-workflowinboxgestion-para-pag/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Repos impactados:
  - `MiApp.Services`
  - `DocuArchiCore`
