## Why

`workflowInboxgestion` seguía dependiendo de hardcodes en el controller (`144` y `DA`) aunque el flujo backend ya soporta claims reales. Eso bloquea la integración final del frontend y deja el endpoint desacoplado del usuario autenticado.

## What Changes

- Restaurar validación real de `defaulalias` y `usuarioid` en `WorkflowInboxController`.
- Eliminar hardcodes del endpoint `POST /api/workflowInboxgestion/inboxgestion`.
- Mantener el controller limitado a validación de claims y delegación al servicio.
- Alinear `WorkflowInboxControllerTests` con el comportamiento final esperado.

## Capabilities

### Modified Capabilities
- jira-scrum-125: corrección de claims reales en `workflowInboxgestion`.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-125
- OpenSpec change path: openspec/changes/scrum-125-corregir-workflowinboxcontroller-claims/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Repos impactados:
  - `DocuArchi.Api`
  - `DocuArchiCore`
