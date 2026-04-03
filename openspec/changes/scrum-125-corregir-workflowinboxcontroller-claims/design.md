## Context

- Jira issue key: SCRUM-125
- Jira summary: CORREGIR-WORKFLOWINBOXCONTROLLER-CLAIMS-REALES-Y-ELIMINAR-HARDCODES
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-125

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

El backend del inbox workflow ya delega correctamente a `WorkflowInboxService`, pero el controller quedó temporalmente con hardcodes:

- `idUsuarioGestion = 144`
- `defaultDbAlias = "DA"`

Eso ignora los claims reales del token y hace que el endpoint no represente la sesión autenticada.

## Approach

- Reutilizar `IClaimValidationService` como en otros controllers protegidos.
- Validar `defaulalias` y `usuarioid` en el controller.
- Parsear `usuarioid` como entero y lanzar error de seguridad si el claim es inválido.
- Delegar al servicio con valores reales sin mover lógica de contexto workflow al controller.
- Cubrir con pruebas unitarias:
  - claim `defaulalias` ausente
  - claim `usuarioid` inválido
  - delegación correcta con claims válidos
