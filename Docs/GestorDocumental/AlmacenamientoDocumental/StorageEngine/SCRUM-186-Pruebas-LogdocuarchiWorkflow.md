# SCRUM-186 — Pruebas Logdocuarchi Workflow

## Suite ejecutada
Comando:
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~WorkflowStorageLog|FullyQualifiedName~StorageTransactionCoordinatorTests|FullyQualifiedName~AlmacenamientoDocumentalControllerTests|FullyQualifiedName~AlmacenarDocumentoUseCaseTests"
```

Resultado:
- `Total: 23`
- `Superado: 23`
- `Fallido: 0`

## Cobertura funcional validada
- Builder:
  - activa/desactiva por `IdTareaWorkflow`
  - mapeo de `RutDocu`, tipología descripción, `Campos` legacy, `IpTrans`
- Service:
  - no inserta cuando `NO_WORKFLOW`
  - inserta cuando `READY`
- Repository:
  - valida modelo
  - inserta en `logdocuarchi` con 15 columnas esperadas
- Coordinator:
  - ejecuta log workflow dentro de transacción
- Controller:
  - integra IP desde `IIpHelper`

