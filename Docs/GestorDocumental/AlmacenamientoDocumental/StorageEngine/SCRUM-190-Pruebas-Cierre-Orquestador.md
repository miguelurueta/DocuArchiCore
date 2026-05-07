# SCRUM-190 - Pruebas Cierre Orquestador

## Objetivo
Validar que el orquestador ejecute el flujo completo y no mantenga comportamiento placeholder.

## Evidencia Ejecutada
- Build:
  - `dotnet build MiApp.Services.csproj -c Release`
  - Resultado: exitoso, 0 errores.
- Test focal almacenamiento (con build):
  - `dotnet test DocuArchiCore/tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj -c Release --filter "FullyQualifiedName~AlmacenarDocumentoUseCaseTests|FullyQualifiedName~StorageValidationPipelineTests"`
  - Resultado: **12 superadas, 0 fallidas**.
  - Nota: se corrigio test desactualizado del orquestador para reflejar constructor y flujo actual.

## Casos Minimos Cubiertos
1. Contexto nulo:
- esperado: `ArgumentNullException`.

2. `RequestId` vacio:
- esperado: `ArgumentException`.

3. Validacion invalida en pipeline:
- esperado: `StorageValidationException`.

4. Sin documentos de entrada:
- esperado: `StorageValidationException` con `DOC_REQUIRED`.

5. Metadata fisica invalida:
- esperado: `StoragePhysicalException`.

6. Transaccion sin identidad valida:
- esperado: `StorageTransactionException`.

7. Fase fisica no completada:
- esperado: `StoragePhysicalException`.

8. Excepcion no tipada:
- esperado: wrap en `StorageTransactionException`.

## Riesgo Residual
- Persisten warnings de nulabilidad en el proyecto global (no bloqueantes para SCRUM-190).
- La paridad legacy integral del StorageEngine completo sigue dependiente de regresión E2E multi-fase.
