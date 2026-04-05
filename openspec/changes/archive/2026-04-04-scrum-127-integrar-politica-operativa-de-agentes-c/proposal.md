## Why

El flujo `opsxj` ya soporta operaciones avanzadas de Jira, GitHub y OpenSpec, pero no tenía una política operativa explícita sobre cuándo usar subagentes mini y cuándo reservar el agente principal. Eso eleva consumo y deja la operación dependiente de criterio implícito.

## What Changes

- Crear una política operativa formal de agentes Codex para `opsxj`.
- Documentar la estrategia en `Docs/Codex-Agent-Strategy.md`.
- Referenciarla desde `Tools/jira-open/README.md`.
- Extender `Tools/jira-open/opsxj.ps1` con hints visibles por comando mediante `Write-CodexAgentHint`.

## Capabilities

### Modified Capabilities
- jira-scrum-127: política operativa de agentes Codex integrada al flujo `opsxj`.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-127
- OpenSpec change path: openspec/changes/scrum-127-integrar-politica-operativa-de-agentes-c/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Repos impactados:
  - `DocuArchiCore`
