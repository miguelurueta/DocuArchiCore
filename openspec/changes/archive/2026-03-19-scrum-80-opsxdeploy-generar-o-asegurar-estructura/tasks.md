## 1. Discovery and Spec Alignment

- [x] 1.1 Revisar el issue Jira SCRUM-80 y confirmar el alcance real de `opsxdeploy`.
- [x] 1.2 Confirmar repos y rutas impactadas: `Tools/iis-deploy/opsxdeploy.ps1`, pruebas del tool, `Tools/iis-deploy/README.md` y `Docs/Publicacion/IIS-DocuArchiApi.md`.
- [x] 1.3 Completar `specs/jira-scrum-80/spec.md` con requisitos finales y escenarios testables.
- [x] 1.4 Incluir referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.

## 2. Tooling

- [x] 2.1 Ajustar `opsxdeploy doctor` para no exigir `web.config` y reportar validacion minima si existe.
- [x] 2.2 Implementar generacion de `web.config` base en `opsxdeploy publish-package` cuando el archivo no exista.
- [x] 2.3 Preservar `web.config` existente y validar `aspNetCore/processPath`, `aspNetCore/arguments` y bloque `environmentVariables`.

## 3. Verification and Docs

- [x] 3.1 Actualizar pruebas automatizadas para cubrir generacion, conservacion y validacion de `web.config`.
- [x] 3.2 Actualizar `Tools/iis-deploy/README.md` con el nuevo comportamiento.
- [x] 3.3 Actualizar `Docs/Publicacion/IIS-DocuArchiApi.md` con el manejo operativo de `web.config`.
- [x] 3.4 Ejecutar validaciones locales relevantes y documentar evidencia.
- [x] 3.5 Validar el cambio con OpenSpec y dejarlo listo para archivo.
