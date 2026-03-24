## Why

`opsxj:orchestrate:new` ya permite abrir cambios multi-repo desde `DocuArchiCore`, pero en `SCRUM-90` todavia requirio intervenciones manuales para aislar repos sucios, distinguir PRs funcionales de PRs tecnicos y completar el archive cuando GitHub elimino ramas remotas despues del merge.

## What Changes

- Endurecer `opsxj:orchestrate:new` para crear worktrees limpios automaticamente cuando un repo impactado no este listo para operar en su checkout principal.
- Clasificar el impacto de cada repo como `implementation_required`, `traceability_only` o `no_code_change` para evitar PRs innecesarios.
- Endurecer `opsxj:orchestrate:archive` para usar el estado `PR MERGED` como fuente principal de verdad en repos satelite.
- Agregar limpieza automatica de residuos temporales del flujo orquestado antes y despues del archive.
- Actualizar documentacion y pruebas del flujo orquestado multi-repo.

## Capabilities

### New Capabilities
- `opsxj-orchestrated-worktree-management`: aislamiento automatico por worktree para repos impactados.
- `opsxj-orchestrated-impact-classification`: clasificacion de impacto funcional vs tecnico en `sync.md`.

### Modified Capabilities
- `opsxj-orchestrated-archive`: archive idempotente basado en estado real de PRs y limpieza automatica.

## Impact

- Cambios en `Tools/jira-open/opsxj.ps1` para worktrees, clasificacion de impacto, limpieza y archive.
- Ajustes en `Tools/jira-open/README.md` y documentacion operativa del flujo multi-repo.
- Nuevas pruebas del flujo orquestado y de escenarios con ramas borradas o repos sucios.
- Repositorio afectado:
- `DocuArchiCore`
