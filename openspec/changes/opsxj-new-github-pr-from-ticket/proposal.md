## Why

El flujo actual de `opsxj:new` genera artefactos OpenSpec desde Jira, pero no abre automaticamente el Pull Request en GitHub. Esto deja el proceso incompleto y agrega trabajo manual despues de crear `proposal.md`.

## What Changes

- Se agrega al flujo `opsxj:new` la creacion automatica de PR en GitHub cuando la propuesta se genera correctamente.
- El titulo del PR se construira con el asunto (`summary`) del ticket de Jira solicitado.
- Se agregan validaciones de autenticacion/conexion de GitHub y manejo de errores claros para fallas al crear PR.
- Se define comportamiento idempotente para evitar PR duplicados para el mismo ticket y rama.

## Capabilities

### New Capabilities
- `opsxj-new-github-pr`: Automatizar creacion de Pull Request en GitHub dentro del flujo `opsxj:new`, usando metadata del ticket Jira.

### Modified Capabilities
- Ninguna.

## Impact

- Scripts en `Tools/jira-open/opsxj.ps1` para integrar creacion de PR.
- Posible soporte auxiliar para invocar `gh` y resolver rama/base/titulo/cuerpo.
- Actualizacion de documentacion de `Tools/jira-open/README.md`.
- Evidencia de pruebas para el nuevo flujo en OpenSpec.
- Repositorios afectados:
- `DocuArchiCore`
