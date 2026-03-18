## 1. Discovery

- [x] 1.1 Revisar el flujo actual de `opsxj:new` y ubicar donde se crean artefactos OpenSpec, se valida el change y se asegura el PR.
- [x] 1.2 Confirmar el comportamiento actual de Jira: hoy solo pasa a `En revisión` cuando el PR es creado en esa misma ejecución.

## 2. Implementation

- [x] 2.1 Agregar transición Jira a `En curso` después de generar y validar artefactos OpenSpec.
- [x] 2.2 Ajustar `opsxj:new` para mover Jira a `En revisión` cuando el PR exista o sea creado, sin degradar estados ya en revisión/done.
- [x] 2.3 Registrar el estado actual del issue en la carga de Jira para soportar re-ejecuciones idempotentes.

## 3. Verification

- [x] 3.1 Actualizar la documentación de `Tools/jira-open/README.md` con el nuevo flujo de estados.
- [x] 3.2 Ajustar `Tools/jira-open/test-opsxj-pr-flow.ps1` para cubrir transición a `En curso`, PR nuevo y PR existente.
- [x] 3.3 Ejecutar `Tools/jira-open/test-opsxj-pr-flow.ps1` y verificar resultado `PASS`.

## 4. Release

- [ ] 4.1 Hacer commit, push y actualizar el PR del cambio.
- [ ] 4.2 Marcar/archivar el change en OpenSpec cuando el PR sea mergeado.
