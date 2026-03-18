## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-77 y confirmar que el problema real es el `ProjectReference` web-a-web `DocuArchi.Api -> DocuArchiCore`.
- [x] 1.2 Confirmar repos impactados y rutas destino: `DocuArchi.Api` para implementación; `DocuArchiCore` como coordinador y fuente actual de `Infrastructure/Security/SesionActual*`.
- [x] 1.3 Confirmar que no se requiere estructura de tabla ni nuevos modelos para resolver este ticket.

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-77/spec.md` con requisitos finales de desacople, DI y publish.
- [x] 2.2 Incluir referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.
- [x] 2.3 Verificar escenarios testables por requisito en `DocuArchi.Api.csproj`, `Program.cs` y `dotnet publish`.

## 3. Application

- [x] 3.1 Migrar `SesionActual*` fuera de `DocuArchiCore` web hacia `DocuArchi.Api`.
- [x] 3.2 Actualizar `DocuArchi.Api/Program.cs` para registrar la nueva ubicacion e interfaces de sesion sin `using DocuArchiCore.Infrastructure.Security`.
- [x] 3.3 Eliminar `..\DocuArchiCore\DocuArchiCore.csproj` de `DocuArchi.Api.csproj` y verificar `dotnet build` / `dotnet publish`.

## 4. Test

- [x] 4.1 Ejecutar validacion minima con `dotnet build` y `dotnet publish` de `DocuArchi.Api`; publish completado sin colisiones de assets duplicados.
- [x] 4.2 Ejecutar `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj`; 167 pruebas superadas y 4 integration tests quedaron en `SKIP` por Docker/Testcontainers pendiente.
- [ ] 4.3 Validar el change con OpenSpec y archivar solo cuando los PRs de repos impactados esten mergeados.
