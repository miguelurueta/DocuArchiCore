## 1. Worktree orchestration

- [x] 1.1 Detectar cuando un repo impactado no puede operarse en su checkout principal
- [x] 1.2 Crear y reutilizar worktrees temporales por issue/repo desde `opsxj:orchestrate:new`
- [x] 1.3 Registrar metadata del worktree temporal para limpieza y reruns

## 2. Impact classification

- [x] 2.1 Extender el modelo de `sync.md` para registrar `implementation_required`, `traceability_only` y `no_code_change`
- [x] 2.2 Usar la clasificacion para decidir si un repo requiere branch, commit y PR
- [x] 2.3 Evitar creacion de PRs vacios cuando un repo no tiene diff funcional

## 3. Archive hardening

- [x] 3.1 Hacer que `opsxj:orchestrate:archive` use `PR MERGED` como validacion primaria en repos satelite
- [x] 3.2 Mantener validacion git como senal secundaria cuando la branch siga disponible
- [x] 3.3 Limpiar automaticamente worktrees/logs/temporales del issue actual antes y despues del archive

## 4. Tests and docs

- [x] 4.1 Agregar pruebas del flujo con repo sucio y creacion automatica de worktree
- [x] 4.2 Agregar pruebas del archive con PR merged y branch remota borrada
- [x] 4.3 Actualizar `Tools/jira-open/README.md` y la guia operativa multi-repo
- [x] 4.4 Registrar evidencia de pruebas en este `tasks.md`

## Test Evidence

Run:
- `npm.cmd --prefix Tools/jira-open run opsxj:test-orchestrate-worktree` -> PASS
- `npm.cmd --prefix Tools/jira-open run opsxj:test-orchestrate-impact` -> PASS
- `npm.cmd --prefix Tools/jira-open run opsxj:test-orchestrate-archive` -> PASS
- `npm.cmd --prefix Tools/jira-open run opsxj:test-pr` -> PASS
- `npm.cmd --prefix Tools/jira-open run opsxj:test-archive` -> PASS
