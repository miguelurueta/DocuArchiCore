## 1. CLI y perfiles de ejecucion

- [ ] 1.1 Agregar el switch `-NonInteractive` en `Tools/jira-open/opsxj.ps1`.
- [ ] 1.2 Propagar el modo de ejecucion a `Invoke-Preflight`, `Invoke-Doctor`, `Invoke-New`, `Invoke-Archive` y helpers relacionados.
- [ ] 1.3 Mantener el flujo legacy como comportamiento por defecto sin cambios funcionales.

## 2. Politica de no interaccion

- [ ] 2.1 En `-NonInteractive`, exigir `GITHUB_TOKEN` y bloquear fallback interactivo de GitHub CLI.
- [ ] 2.2 En `-NonInteractive`, validar configuracion Jira requerida antes de generar artefactos o ejecutar cambios Git.
- [ ] 2.3 Garantizar que el modo nuevo no invoque rutas de input interactivo.

## 3. Auditoria

- [ ] 3.1 Extender `Write-OpsxjLog` para registrar `mode` y metadatos operativos relevantes.
- [ ] 3.2 Confirmar que `opsxj:new`, `opsxj:doctor` y `opsxj:archive` registran consistentemente el modo de ejecucion.

## 4. Pruebas

- [ ] 4.1 Agregar o actualizar pruebas para `opsxj:new -NonInteractive` con `GITHUB_TOKEN`.
- [ ] 4.2 Agregar una prueba de fallo controlado cuando `-NonInteractive` se ejecuta sin `GITHUB_TOKEN`.
- [ ] 4.3 Verificar que el flujo legacy existente sigue pasando sin cambios de comportamiento esperados.

## 5. Documentacion

- [ ] 5.1 Actualizar `Tools/jira-open/README.md` con ejemplos del nuevo switch.
- [ ] 5.2 Documentar el uso enterprise/headless y los prerequisitos minimos de CI/CD.
