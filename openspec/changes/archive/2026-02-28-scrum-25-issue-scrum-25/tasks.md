## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-25 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo (N/A para alcance de tooling, sin cambios en modelos).

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-25/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Aplicar cambios de tooling en `opsxj:new` para autodeteccion + seleccion manual (`-SelectRepos`) y generacion de `sync.md`.
- [x] 3.2 Integrar referencia de reglas backend en artefactos generados (`proposal/design/tasks/spec`).
- [x] 3.3 Integrar cambios de aplicacion y verificar ejecucion local con scripts del repositorio.

## 4. Test

- [x] 4.1 Implementar/usar pruebas automatizadas del flujo (`test-opsxj-pr-flow.ps1`, `test-opsxj-archive-flow.ps1`) y documentar evidencia.
- [x] 4.2 Ejecutar pruebas locales: `npm.cmd --prefix Tools/jira-open run opsxj:test-pr` y `npm.cmd --prefix Tools/jira-open run opsxj:test-archive`.
- [x] 4.3 Validar y archivar con OpenSpec.

