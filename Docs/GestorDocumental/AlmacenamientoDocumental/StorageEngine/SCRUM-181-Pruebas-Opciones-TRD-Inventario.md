# SCRUM-181 - Pruebas Opciones TRD/Inventario

## Evidencia de compilacion
- `dotnet build tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj`
- Resultado: 0 errores.

## Evidencia de pruebas focales
- `dotnet test ... --filter "StorageValidationPipelineTests|StorageTransactionCoordinatorTests|StorageTransactionCoordinatorIntegrationTests|StorageEngineContractsTests|AlmacenarDocumentoUseCaseTests|WorkflowStorageLogBuilderTests"`
- Resultado: 26/26 pruebas exitosas.

## Cobertura funcional validada
- Preindex batch/manual.
- Metadata y obligatoriedad de campos.
- Reglas inventario (`INV_REQUIRED`, `INV_USER_REQUIRED`, `INV_EMPRESA_REQUIRED`).
- Reglas TRD (`TRD_INVALID_*`).
- Reglas expediente/unidad (`EXP_CLASE_REQUIRED`, `UNI_CLASE_REQUIRED`).

## Riesgos residuales
- Permanecen warnings de nulabilidad no bloqueantes fuera del alcance del ticket.
