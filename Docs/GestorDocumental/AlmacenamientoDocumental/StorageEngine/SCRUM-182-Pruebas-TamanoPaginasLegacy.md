# SCRUM-182 - Pruebas Tamano y Conteo Real de Paginas

## Unitarias Ejecutadas
Proyecto:
- `tests/TramiteDiasVencimiento.Tests`

Comando:
- `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "StoragePhysicalMetadataAnalyzerTests|StorageIdentityAllocatorTests|StorageTransactionCoordinatorTests|AlmacenarDocumentoUseCaseTests"`

Resultado:
- `Total: 16`
- `Passed: 16`
- `Failed: 0`

## Cobertura Funcional
- `StorageSizeFormatter`:
  - conversion `Kb/Mb`, precision 2 decimales, validacion de negativos.
- `StoragePageCountReader`:
  - imagen comun = 1,
  - PDF > 0,
  - no soportado = `null`,
  - archivo inexistente = error.
- `StorageDocumentMetadataAnalyzer`:
  - usa paginas fisicas cuando disponibles,
  - fallback a paginas declaradas,
  - error cuando no hay manera de determinar paginas.
- `StorageIdentityAllocator` y `StorageTransactionCoordinator`:
  - consumen `context.PhysicalMetadata.NumeroPaginas`.
