## 1. Validacion Git en archive

- [x] 1.1 Agregar validacion de merge en `Tools/jira-open/opsxj.ps1` antes de `openspec archive`
- [x] 1.2 Reutilizar configuracion de rama base (`GIT_BASE_BRANCH`) y deteccion automatica

## 2. Cierre de ticket Jira

- [x] 2.1 Implementar obtencion de transiciones Jira para un issue
- [x] 2.2 Implementar transicion a estado terminado al finalizar `opsxj:archive`
- [x] 2.3 Reportar errores claros si no existe transicion de cierre

## 3. Pruebas y documentacion

- [x] 3.1 Agregar prueba automatizada de flujo `opsxj:archive` con validacion Git + Jira done
- [x] 3.2 Actualizar `Tools/jira-open/README.md` y ejemplo de entorno
- [x] 3.3 Registrar evidencia de pruebas en este `tasks.md`

## Test Evidence

Run: 2026-02-27 local

```text
> npm.cmd --prefix Tools/jira-open run opsxj:test-pr

PASS: opsxj:new creates PR and avoids duplicates.

> npm.cmd --prefix Tools/jira-open run opsxj:test-archive

PASS: opsxj:archive validates git merge and transitions Jira to Done.
```
