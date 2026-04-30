## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-175 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-175/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Aplicar patron ApiController + Service + AutoMapper + Repository con AppResponses y try/catch.
- [x] 3.2 Registrar interfaces en Program.cs (Services L / Repositories R).
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local.

## 4. Test

- [x] 4.1 Implementar Unit/Integration/Contract tests y documentar evidencia.
- [x] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible).
- [x] 4.3 Validar y archivar con OpenSpec.

## Notes

- Este cambio en `DocuArchiCore` se gestiona como `implementation_required` de orquestación OpenSpec/Jira con PR coordinador.
- Los repos satélite (`DocuArchi.Api`, `MiApp.DTOs`, `MiApp.Services`, `MiApp.Repository`, `MiApp.Models`) quedaron en `traceability_only` en `sync.md`; no se abren PR vacíos desde este repo.
