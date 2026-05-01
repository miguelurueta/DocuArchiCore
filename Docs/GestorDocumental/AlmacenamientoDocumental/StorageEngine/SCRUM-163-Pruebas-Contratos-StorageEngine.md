# SCRUM-163 Pruebas de Contratos Storage Engine

## Matriz
- DTOs instanciables y `required` cubiertos.
- `DocumentoEntradaDto` usa `ArchivoTemporalId`.
- `AlmacenarDocumentoResponse` incluye `RequestId`.
- `AlmacenarDocumentoResult` incluye `Estado`.
- `StorageDocumentState` incluye `PhysicalFailed`.
- `StorageValidationException` conserva `Errors`.
- Existen `StoragePhysicalException` y `StorageTransactionException`.
- Existen modelos de idempotencia y builders base.

## Evidencia
- Test agregado: `tests/TramiteDiasVencimiento.Tests/StorageEngineContractsTests.cs`
- Comando ejecutado:
  - `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter FullyQualifiedName~StorageEngineContractsTests`

## Resultado
- Restore/build de contratos ejecutado correctamente.
- La suite completa falla por errores preexistentes no relacionados en tests de controllers faltantes.
