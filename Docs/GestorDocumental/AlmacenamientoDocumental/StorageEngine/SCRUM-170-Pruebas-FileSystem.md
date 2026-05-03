# SCRUM-170 Pruebas FileSystem

## Unitarias implementadas
- `StoragePathResolverTests`
- `StoragePhysicalPhaseExecutorTests`
- ajustes en `AlmacenarDocumentoUseCaseTests`
- ajustes en `StorageValidationPipelineTests`

## Cobertura funcional validada
- hardening de rutas (rechazo de traversal).
- ejecución física success (`Completed` + `NombreArchivoFinal`).
- compensación cuando falla copy/xml.

## Ejecución
- `dotnet test` intentado en `TramiteDiasVencimiento.Tests`.
- entorno actual: bloqueo de restore/MSBuild (`_GenerateRestoreProjectPathWalk`) sin errores C# detallados.
