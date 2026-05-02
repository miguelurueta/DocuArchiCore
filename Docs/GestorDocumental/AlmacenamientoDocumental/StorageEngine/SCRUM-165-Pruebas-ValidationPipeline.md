# SCRUM-165 Pruebas ValidationPipeline

## Matriz unitaria
- PreindexValidator:
  - Tipo != BatchPreindex: no valida preindex.
  - Batch sin archivo: `PREINDEX_NOT_FOUND`.
  - Batch con resultado valido: carga `PreindexValues`.
  - Ruta invalida: `PREINDEX_PATH_INVALID`.
- GabineteRequiredFieldsValidator:
  - Metadata vacia: `GAB_FIELDS_NOT_FOUND`.
  - Mismatch y requerido vacio: `GAB_FIELDS_MISMATCH` + `GAB_REQUIRED_EMPTY`.
- StorageOptionsValidator:
  - Inventario requerido y nulo: `INV_REQUIRED`.
- TrdRulesValidator:
  - IDs negativos: errores TRD.
- ExpedienteUnidadRulesValidator:
  - Clase faltante con expediente/unidad: `EXP_CLASE_REQUIRED`, `UNI_CLASE_REQUIRED`.
- Orchestrator:
  - Pipeline invalido lanza `StorageValidationException`.

## Ejecucion
- `dotnet build` exitoso en `MiApp.Models`, `MiApp.Services`, `DocuArchi.Api`.
- `dotnet test` global bloqueado por error preexistente no relacionado:
  - `SolicitaListaTiposRespuestaControllerTests.cs` referencia namespace inexistente.

## Riesgo residual
- Resolver test preexistente para poder ejecutar suite completa y confirmar regressions globales.
