## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-156 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [ ] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-156/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Aplicar patron ApiController + Service + AutoMapper + Repository con AppResponses y try/catch.
- [x] 3.2 Registrar interfaces en Program.cs (Services L / Repositories R).
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local.

## 4. Test

- [ ] 4.1 Implementar Unit/Integration/Contract tests y documentar evidencia.
- [ ] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible).
- [ ] 4.3 Validar y archivar con OpenSpec.

### Evidencia (avance parcial)

- Unit tests agregados: `tests/TramiteDiasVencimiento.Tests/ServiceSolicitaCorreoElectronicoRemitenteTests.cs` y `tests/TramiteDiasVencimiento.Tests/SolicitaCorreoElectronicoRemitenteControllerTests.cs`.
- Tests focales OK: `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj -c Release --filter FullyQualifiedName~SolicitaCorreoElectronicoRemitente` (7/7).
- `dotnet test` completo del proyecto de pruebas presenta fallas en otros tests no relacionados a SCRUM-156 (preexistentes / mantenimiento pendiente).
- Compilación API OK: `dotnet build ..\DocuArchi.Api\DocuArchi.Api.csproj -c Release`.
