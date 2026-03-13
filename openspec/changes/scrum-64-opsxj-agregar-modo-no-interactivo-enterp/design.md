## Context

- Jira issue key: SCRUM-64
- Jira summary: OPSXJ: agregar modo no interactivo enterprise para opsxj:new manteniendo compatibilidad legacy
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-64

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se requiere incorporar un modo no interactivo explicito para `opsxj:new`, orientado a operacion enterprise y compatibilidad con CI/CD, sin alterar el comportamiento actual por defecto.

El nuevo modo debe activarse mediante un parametro adicional, por ejemplo `-NonInteractive`, y debe ejecutar el flujo sin prompts ni dependencias de autenticacion manual. El modo actual debe conservarse intacto para evitar regresiones operativas.

## Goals

- Mantener compatibilidad total con `opsxj:new <ISSUE-KEY>`.
- Agregar `opsxj:new <ISSUE-KEY> -NonInteractive`.
- Extender el mismo criterio a `opsxj:doctor` y `opsxj:archive`.
- Hacer que el modo no interactivo sea apto para CI/CD.
- Registrar auditoria diferenciada por modo de ejecucion.

## Non-Goals

- No redisenar el flujo funcional principal de `opsxj:new`.
- No eliminar el modo legacy.
- No reemplazar la autodeteccion actual de repos salvo donde sea necesario para evitar interaccion.
- No cambiar el contrato funcional de generacion de artefactos, PR o archive fuera del nuevo perfil.

## Proposed Design

### CLI

Se agrega el switch `-NonInteractive` a:
- `opsxj:new`
- `opsxj:doctor`
- `opsxj:archive`

### Execution Profiles

#### Legacy

- Se activa por defecto.
- Mantiene el comportamiento actual.
- Conserva compatibilidad con usuarios y automatizaciones existentes.

#### NonInteractive

- No solicita input del usuario.
- No usa seleccion interactiva de repos.
- Exige configuracion valida de Jira antes de generar artefactos.
- Exige `GITHUB_TOKEN` para operaciones GitHub.
- No depende de `gh auth login`.
- Falla rapido antes de modificar archivos o ejecutar push si falta configuracion critica.

### GitHub Authentication Policy

En `-NonInteractive`:
- La autenticacion GitHub se resuelve exclusivamente con `GITHUB_TOKEN`.
- Si no existe token, el comando falla.
- No se permite fallback operativo a `gh auth status` o `gh auth login`.

En modo legacy:
- Se conserva el comportamiento actual.

### Repo Resolution Policy

En `-NonInteractive`:
- Se mantiene prohibido `-SelectRepos`.
- Se permite `OPSXJ_IMPACT_REPOS`.
- Se permite la autodeteccion deterministica existente.
- La resolucion usada debe quedar registrada en logs.

### Audit Logging

Cada ejecucion debe registrar:
- `mode`
- `issueKey`
- `step`
- `status`
- `message`
- `branch` cuando aplique
- `prUrl` cuando aplique

La auditoria continuara escribiendose en `openspec/logs/*.log.jsonl`.

## Approach

- Implementar el nuevo perfil detras del switch `-NonInteractive`.
- Reusar `Invoke-Preflight`, `Invoke-Doctor`, `Invoke-New`, `Invoke-Archive` y logging condicionando las validaciones por modo.
- Mantener el flujo legacy intacto por defecto para minimizar riesgo.
- Validar con `openspec.cmd validate scrum-64-opsxj-agregar-modo-no-interactivo-enterp` y pruebas `opsxj`.
