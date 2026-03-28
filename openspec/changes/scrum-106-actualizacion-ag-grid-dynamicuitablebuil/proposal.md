## Why

La configuracion actual de `DynamicUiTableBuilder` expone aliases pensados para MUI y Ant Design, pero no entrega metadata explicita para AG Grid. Eso obliga al frontend a inferir `field` y el tipo de filtro de cada columna, lo que introduce reglas duplicadas y ambiguedades en pantallas que ya consumen la tabla dinamica.

## What Changes

- Extender `UiColumnDto` para publicar aliases explicitos consumibles por AG Grid (`field`, `agGridFilterType`) sin romper el contrato existente.
- Ajustar `DynamicUiTableBuilder` para normalizar aliases y derivar metadata de filtro compatible con MUI, AG Grid y Ant Design.
- Actualizar pruebas y documentacion tecnica para dejar trazabilidad del contrato multi-repo.

## Capabilities

### New Capabilities
- jira-scrum-106: Inicio de cambio OpenSpec originado en Jira issue SCRUM-106.

### Modified Capabilities
- None.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-106
- OpenSpec change path: openspec/changes/scrum-106-actualizacion-ag-grid-dynamicuitablebuil/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Multi-repo context reference: openspec/context/multi-repo-context.md
- Affected repositories:
  - DocuArchiCore
  - MiApp.DTOs
  - MiApp.Services
