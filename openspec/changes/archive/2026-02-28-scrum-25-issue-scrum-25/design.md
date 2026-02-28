## Context

- Jira issue key: SCRUM-25
- Jira summary: Issue SCRUM-25
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-25

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

El flujo `opsxj:new` en tickets con alcance multi-repo necesitaba una forma clara de decidir repositorios impactados cuando la autodeteccion no era suficiente. Sin esa decision temprana:

- se ejecutaba `opsxj:new` en repos no impactados,
- la matriz `sync.md` no quedaba consistente (`yes/no`, `done/pending/n/a`),
- y el cambio OpenSpec se quedaba con tareas genericas sin trazabilidad operativa.

## Approach

- Agregar modo de seleccion manual de repos en `opsxj:new` usando `-SelectRepos`.
- Mantener autodeteccion por keywords y fallback a seleccion manual cuando no hay confianza.
- Generar `sync.md` con estado inicial coherente por repo (`yes/no`, `done/pending/n/a`).
- Mantener referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md` en artefactos generados.
- Cubrir flujo con pruebas automatizadas del comando (`opsxj:test-pr`, `opsxj:test-archive`).
- Validar cambio con `openspec.cmd validate scrum-25-issue-scrum-25`.
