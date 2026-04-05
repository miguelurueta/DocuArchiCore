## Context

- Jira issue key: SCRUM-127
- Jira summary: Integrar politica operativa de agentes Codex en flujo opsxj backend
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-127

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

`opsxj` ya centraliza operaciones críticas de gobierno del flujo, pero no explicita una política de uso de agentes Codex. Falta una guía visible y consistente que diga:

- qué tareas conviene ejecutar con subagente mini
- qué tareas deben permanecer en el agente principal
- que el script no controla el modelo y solo orienta la operación

## Approach

- Crear un documento técnico único de política operativa.
- Agregar hints visibles por comando desde `opsxj.ps1` usando una función reusable.
- Referenciar la política desde el README del tooling.
- No alterar el comportamiento funcional de Jira, Git, GitHub ni OpenSpec.
- Validar el cambio con:
  - `openspec.cmd validate scrum-127-integrar-politica-operativa-de-agentes-c`
  - ejecución simple de `opsxj.ps1 doctor` para comprobar la salida de hints
