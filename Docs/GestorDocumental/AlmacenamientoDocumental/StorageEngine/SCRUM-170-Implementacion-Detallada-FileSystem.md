# SCRUM-170 Implementacion Detallada FileSystem

## Repos impactados
- `MiApp.Services`
- `MiApp.Models`
- `DocuArchi.Api`
- `DocuArchiCore` (OpenSpec, docs, pruebas)

## Archivos clave
- `MiApp.Services/.../Builders/StoragePlanBuilder.cs`
- `MiApp.Services/.../Builders/StorageXmlBuilder.cs`
- `MiApp.Services/.../Physical/StoragePathResolver.cs`
- `MiApp.Services/.../Physical/StorageFileWriter.cs`
- `MiApp.Services/.../Physical/StorageXmlWriter.cs`
- `MiApp.Services/.../Physical/StorageCompensationManager.cs`
- `MiApp.Services/.../Physical/StoragePhysicalPhaseExecutor.cs`
- `MiApp.Services/.../DocumentStorageOrchestrator.cs` (integración)
- `MiApp.Models/.../StoragePhysicalStatusModel.cs` (agrega `NombreArchivoFinal`)
- `DocuArchi.Api/Program.cs` (DI)

## Flujo
1. Validation pipeline.
2. Coordinator DB transaccional.
3. Build plan físico.
4. Copia de archivos (`.tmp` -> final).
5. Construcción y escritura XML (`.tmp` -> final).
6. Si falla cualquier paso, compensación.
