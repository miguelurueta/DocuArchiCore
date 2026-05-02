# SCRUM-164 Pruebas UseCase/Orchestrator Storage Engine

## Casos cubiertos
- Validación cuando request es nulo.
- Mapeo de respuesta exitosa desde orchestrator al response DTO.
- Respuesta base `Pending` del orchestrator.

## Archivo de pruebas
- `tests/TramiteDiasVencimiento.Tests/AlmacenarDocumentoUseCaseTests.cs`

## Ejecución
- `dotnet build MiApp.Services.csproj` OK
- `dotnet build DocuArchi.Api.csproj` OK
- `dotnet test ... --filter FullyQualifiedName~AlmacenarDocumentoUseCaseTests`

## Resultado
- La ejecución focal de test queda bloqueada por error preexistente no relacionado:
  - `SolicitaListaTiposRespuestaControllerTests` (namespace/controller faltante en suite global de tests).
