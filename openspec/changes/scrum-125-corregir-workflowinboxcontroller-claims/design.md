## Context

- Jira issue key: SCRUM-125
- Jira summary: CORREGIR-WORKFLOWINBOXCONTROLLER-CLAIMS-REALES-Y-ELIMINAR-HARDCODES
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-125

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Cerrar el hueco real detectado en workflowInboxgestion: el controller sigue usando idUsuarioGestion y defaultDbAlias hardcodeados en lugar de claims reales, por lo que el backend no esta listo para la integracion final del frontend server mode. Alcance: restaurar validacion real de defaulalias y usuarioid en WorkflowInboxController eliminar hardcodes 144 y DA del endpoint POST /api/workflowInboxgestion/inboxgestion alinear WorkflowInboxControllerTests con la implementacion final y validar claim invalido, claim ausente y delegacion correcta al servicio dejar el controller limitado a validacion de claims y delegacion, sin mover logica de contexto workflow al controller Criterio de aceptacion: el endpoint deja de depender de hardcodes usa claims reales y propaga usuarioid parseado al servicio la suite WorkflowInboxControllerTests queda consistente con la implementacion bloqueo backend para SCRUMCORE-42 y SCRUMCORE-43 queda resuelto

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-125-corregir-workflowinboxcontroller-claims.