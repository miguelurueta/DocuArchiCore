## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-171 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo. (N/A: ticket enfocado en API final, claims, feature flag y documentación; no requiere nueva tabla.)

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-171/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Aplicar patron ApiController + Service + AutoMapper + Repository con AppResponses y try/catch.
- [x] 3.2 Registrar interfaces en Program.cs (Services L / Repositories R).
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local. (Evidencia: `dotnet build ..\\DocuArchi.Api\\DocuArchi.Api.csproj` OK).

## 4. Test

- [x] 4.1 Implementar Unit/Integration/Contract tests y documentar evidencia.
- [x] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible). (Ejecucion intentada; bloqueo por error preexistente en test suite: `SolicitaListaTiposRespuestaControllerTests` referencia namespace/controlador inexistente, no relacionado con SCRUM-171).
- [ ] 4.3 Validar y archivar con OpenSpec.
