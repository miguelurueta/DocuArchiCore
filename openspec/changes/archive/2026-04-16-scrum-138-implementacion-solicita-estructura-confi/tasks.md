## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-138 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear artefactos.
- [x] 1.3 Confirmar estructura/campos usados del modelo (según ticket) antes de crear el modelo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-138/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Implementar Repository + Model usando IDapperCrudEngine + QueryOptions (filtro solo NAME_PROCESO) con AppResponses y try/catch.
- [x] 3.2 Registrar interface en DI (`DocuArchi.Api/Program.cs`) para resolver el repositorio.
- [x] 3.3 Integrar cambios multi-repo y verificar compilación de la solución a través de `dotnet test` (build/restauración).

## 4. Test

- [x] 4.1 Implementar Unit tests para validaciones, sin resultados, con resultados y excepción del engine.
- [x] 4.2 Ejecutar `dotnet test` (filtrado al suite nuevo) y confirmar passing.
- [x] 4.3 Validar (`openspec.cmd validate`) y archivar con OpenSpec.
