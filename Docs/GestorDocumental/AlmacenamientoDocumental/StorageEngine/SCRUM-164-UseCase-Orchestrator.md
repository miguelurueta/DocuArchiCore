# SCRUM-164 UseCase y Orchestrator Base

## Objetivo
Implementar la base de Application Layer para Storage Engine:
- `IAlmacenarDocumentoUseCase` / `AlmacenarDocumentoUseCase`
- `IDocumentStorageOrchestrator` / `DocumentStorageOrchestrator`

## Implementación
- Repositorio: `MiApp.Services`
- Ruta: `Service/GestorDocumental/AlmacenamientoDocumental/`
- DI registrado en `DocuArchi.Api/Program.cs`

## Alcance técnico
- Validación defensiva de request/alias/usuario/usuarioId.
- Construcción de `StorageContext` desde contratos SCRUM-163.
- Orchestrator base sin acceso DB/FS/XML (solo stub de pipeline).
- Manejo de errores tipados (`StorageValidationException`, `StorageTransactionException`, `StoragePhysicalException`) y respuesta `AppResponses`.

## Evidencia de compilación/pruebas
- `dotnet build MiApp.Services.csproj` OK
- `dotnet build DocuArchi.Api.csproj` OK
- `dotnet test ... --filter FullyQualifiedName~AlmacenarDocumentoUseCaseTests` bloqueado por error preexistente en tests no relacionados:
  `SolicitaListaTiposRespuestaControllerTests` (namespace/controller faltante)
