## Why

El flujo actual de OpenSpec requiere redactar propuestas manualmente, lo que retrasa la preparación de cambios basados en tickets Jira y produce inconsistencias. Automatizar la generación de propuestas a partir de la metadata de Jira reduce fricción y estandariza el inicio de cambios.

## What Changes

- Se agrega un generador de propuestas OpenSpec basado en un issue de Jira, usando un script de consulta y un paso de IA.
- La propuesta se guardara automaticamente en `openspec/changes/<issueKey>/proposal.md` a partir del `summary` y `description` del ticket.
- Se define un input `issueKey` y pasos de ejecucion claros para reutilizar el flujo.
- La IA debe detectar automaticamente el repositorio objetivo del cambio segun la logica del requerimiento.
- Si la IA no puede determinar con confianza el repositorio, el flujo debe solicitar al usuario el repo o una plantilla de cambios antes de continuar.
- El flujo debe completar artefactos OpenSpec hasta `Application` y `Test`.
- Al archivar, se deben detectar repos impactados, validar PRs por repo y esperar merge de todos antes de archivar.
- Luego del merge total: archivar local, hacer push del archivado en `DocuArchiCore` y cambiar estado del ticket en Jira.
- El sistema debe registrar logs trazables por etapa: deteccion, PR, merge, archive, push y Jira.

## Capabilities

### New Capabilities
- `jira-proposal-generator`: Generacion automatica de propuestas OpenSpec usando datos de Jira (summary/description) y un flujo predefinido de pasos.

### Modified Capabilities
- 

## Impact

- Nuevos/ajustados scripts en `scripts/` para consultar Jira.
- Nuevos artefactos en `openspec/changes/<issueKey>/proposal.md`.
- Posible actualizacion de documentacion o tooling interno para ejecutar el generador.

## AI Repository Context

- `DocuArchi.Api`: repositorio para crear APIs y controllers.
- `DocuArchiCore`: repositorio orquestador que guarda toda la informacion de OpenSpec.
- `MiApp.DTOs`: repositorio para crear y guardar contratos DTOs.
- `MiApp.Models`: repositorio para crear o guardar clases Dapper que mapean tablas.
- `MiApp.Repository`: repositorio para crear consultas y actualizaciones de base de datos.
- `MiApp.Services`: repositorio para crear servicios y automapeos.
