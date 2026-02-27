## Why

SCRUM-23 requiere ampliar el contexto de OpenSpec para trabajar con repositorios separados y mantener trazabilidad centralizada desde `DocuArchiCore`.

## What Changes

- Ampliar `openspec/config.yaml` con lineamientos de contexto multi-repo para SCRUM-23.
- Definir que `DocuArchiCore` es repositorio coordinador de contexto/documentacion.
- Exigir referencia a `openspec/context/multi-repo-context.md` en cambios cross-repo.
- Mantener tablero de sincronizacion para estado y enlaces de PR por repositorio.
- Declarar explicitamente el alcance de este cambio como context-only.

## Capabilities

### New Capabilities
- jira-scrum-23: Coordinacion de contexto OpenSpec multi-repo para el issue SCRUM-23.

### Modified Capabilities
- Ninguna.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-23
- OpenSpec change path: openspec/changes/scrum-23-contexto-backend-docuarchi/
- Referencia de contexto multi-repo: openspec/context/multi-repo-context.md
- Alcance de implementacion: No code changes in executor repositories
- Repositorios afectados:
- DocuArchiCore (coordinador OpenSpec)
- DocuArchi.Api
- MiApp.Services
- MiApp.Repository
- MiApp.DTOs
- MiApp.Models
