## Why

Se requiere acelerar la creacion de cambios OpenSpec basados en Jira para reducir trabajo manual y mantener consistencia.

## What Changes

- Reubicar físicamente (mover archivos) del flujo `SolicitaEstructuraRespuestaIdTarea` a nuevas carpetas.
- Actualizar `namespace` / `using` / referencias (incluye DI en `Program.cs` si aplica) para que compile sin cambios funcionales.
- Mantener ruta HTTP, contratos públicos y nombres de clases/interfaces.

## Capabilities

### New Capabilities
- jira-scrum-157: Inicio de cambio OpenSpec originado en Jira issue SCRUM-157.

### Modified Capabilities
- None.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-157
- OpenSpec change path: openspec/changes/scrum-157-reubicar-solicitaestructurarespuestaidta/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Multi-repo context reference: openspec/context/multi-repo-context.md
