## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-24 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla `rea_001_feriados` (columnas/tipos/nullability/PK) antes de generar modelo. Ver `table-schema-request.md`.
- [x] 1.4 Preparar SQL de introspeccion para DBA/desarrollador (`db-introspection.sql`).

## 2. Specs

- [ ] 2.1 Completar specs/jira-scrum-24/spec.md con requisitos finales.
- [ ] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [ ] 2.3 Verificar escenarios testables por requisito.

## 3. Execution

- [ ] 3.1 Aplicar patron ApiController + Service + AutoMapper + Repository con AppResponses y try/catch.
- [ ] 3.2 Registrar interfaces en Program.cs (Services L / Repositories R).
- [ ] 3.3 Implementar Unit/Integration/Contract tests y documentar evidencia.
- [ ] 3.4 Ejecutar dotnet test (o skipped explicito si Docker no disponible).
- [ ] 3.5 Validar y archivar con OpenSpec.

## Current State

- `in_progress`: modelo creado en `MiApp.Models` (PR abierto), pendiente capa repository/tests.
