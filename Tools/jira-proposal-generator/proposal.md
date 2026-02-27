## Why

El flujo actual de OpenSpec requiere redactar propuestas manualmente, lo que retrasa la preparación de cambios basados en tickets Jira y produce inconsistencias. Automatizar la generación de propuestas a partir de la metadata de Jira reduce fricción y estandariza el inicio de cambios.

## What Changes

- Se agrega un generador de propuestas OpenSpec basado en un issue de Jira, usando un script de consulta y un paso de IA.
- La propuesta se guardara automaticamente en `openspec/changes/<issueKey>/proposal.md` a partir del `summary` y `description` del ticket.
- Se define un input `issueKey` y pasos de ejecucion claros para reutilizar el flujo.

## Capabilities

### New Capabilities
- `jira-proposal-generator`: Generacion automatica de propuestas OpenSpec usando datos de Jira (summary/description) y un flujo predefinido de pasos.

### Modified Capabilities
- 

## Impact

- Nuevos/ajustados scripts en `scripts/` para consultar Jira.
- Nuevos artefactos en `openspec/changes/<issueKey>/proposal.md`.
- Posible actualizacion de documentacion o tooling interno para ejecutar el generador.
