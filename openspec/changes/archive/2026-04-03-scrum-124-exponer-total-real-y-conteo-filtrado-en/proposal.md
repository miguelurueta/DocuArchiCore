## Why

`workflowInboxgestion` publicaba `Pagination.Total` con el tamaño de la página actual en vez del total real de la consulta filtrada. Eso impide paginación servidor consistente y no permite UX tipo `1-25 de 320`.

## What Changes

- Construir una query separada de `COUNT(1)` para `workflowInboxgestion`.
- Reutilizar exactamente la misma lógica de filtros/joins del `WorkflowInboxQueryBuilder` para datos y conteo.
- Propagar el total filtrado real desde repository/query layer hasta `DynamicUiTableDto.Pagination.Total`.
- Mantener intacto el contrato público del endpoint y el comportamiento actual de filtros, búsqueda y ordenamiento.

## Capabilities

### Modified Capabilities
- jira-scrum-124: paginación servidor real para `workflowInboxgestion` con `Pagination.Total` filtrado.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-124
- OpenSpec change path: openspec/changes/scrum-124-exponer-total-real-y-conteo-filtrado-en/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Repos impactados:
  - `MiApp.Repository`
  - `MiApp.Services`
  - `DocuArchiCore`
