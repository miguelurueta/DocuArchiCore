## 1. Integracion GitHub en opsxj:new

- [x] 1.1 Extender `Tools/jira-open/opsxj.ps1` para ejecutar paso de PR despues de generar `proposal.md`
- [x] 1.2 Validar prerequisitos de GitHub CLI (`gh`) y contexto de repositorio antes de crear PR

## 2. Creacion de PR con asunto del ticket

- [x] 2.1 Resolver datos Jira necesarios para PR (issueKey y `summary`) dentro del flujo actual
- [x] 2.2 Implementar creacion de rama/push y `gh pr create` con titulo basado en el `summary` del ticket
- [x] 2.3 Implementar deteccion de PR existente para evitar duplicados en re-ejecuciones

## 3. Errores y validacion

- [x] 3.1 Implementar mensajes de error claros para fallas de auth, push o creacion de PR
- [x] 3.2 Agregar/actualizar pruebas del flujo `opsxj:new` para validar creacion de PR y manejo de errores

## 4. Documentacion y evidencia

- [x] 4.1 Actualizar `Tools/jira-open/README.md` con el nuevo comportamiento de `opsxj:new`
- [x] 4.2 Registrar evidencia de pruebas en este `tasks.md`

## Test Evidence

Run: 2026-02-27 local

```text
> npm.cmd --prefix Tools/jira-open run opsxj:test-pr

> jira-open-tools@0.1.0 opsxj:test-pr
> powershell -NoProfile -ExecutionPolicy Bypass -File ./test-opsxj-pr-flow.ps1

PASS: opsxj:new creates PR and avoids duplicates.
```
