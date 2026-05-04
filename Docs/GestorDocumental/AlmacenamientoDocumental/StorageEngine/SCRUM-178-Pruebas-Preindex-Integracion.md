# SCRUM-178 — Pruebas Preindex Integración

## Unitarias actualizadas
Archivo: `tests/TramiteDiasVencimiento.Tests/StorageValidationPipelineTests.cs`

Casos cubiertos:
- `PreindexValidator_ShouldSkip_WhenTipoIsNotBatchPreindex`
- `PreindexValidator_ShouldReturnNotFound_WhenBatchAndNoFile`
- `PreindexValidator_ShouldReadValues_WhenBatchAndTxtValid`
- `PreindexValidator_ShouldReturnPathInvalid_WhenPathIsInvalid`
- `PreindexValidator_ShouldReturnMismatch_WhenValuesCountDoesNotMatchCampos`
- `PreindexValidator_ShouldNotOverwriteManualValues_WhenPreindexHasData`

## Cobertura funcional
- Aplicación condicional a `BatchPreindex`.
- Error controlado cuando no existe preindex.
- Integración de valores leídos al contexto.
- Control de mismatch cantidad valores vs cantidad campos.
- Protección de valores manuales no vacíos.

## Estado de ejecución
- En este entorno no fue posible completar ejecución estándar de `dotnet test` por fallo de resolución de referencias de proyecto en MSBuild (`_GetProjectReferenceTargetFrameworkProperties`) sin errores de compilación explícitos.
- Las pruebas quedaron actualizadas en código para ejecución en pipeline CI/CD del repositorio.
