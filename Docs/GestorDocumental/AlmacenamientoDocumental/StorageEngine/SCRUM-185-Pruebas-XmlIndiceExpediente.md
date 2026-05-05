# SCRUM-185 — Pruebas XML Índice de Expediente

## Unitarias agregadas
- `ExpedienteIndiceXmlBuilderTests`
  - construye modelo con datos válidos.
  - falla cuando falta `PhysicalMetadata`.
- `ExpedienteIndiceXmlWriterTests`
  - agrega nodo `DocumentoIndizado` en XML existente.
  - falla cuando no existe archivo ruta.
- `ExpedienteIndiceXmlServiceTests`
  - retorna `NO_EXPEDIENTE` cuando no aplica.
  - ejecuta writer cuando se cumplen condiciones legacy.
- `StoragePhysicalPhaseExecutorTests`
  - falla post-commit de XML índice no revierte estado `Completed`.

## Ejecución validada
Comando:
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~ExpedienteIndiceXml|FullyQualifiedName~StoragePhysicalPhaseExecutorTests"
```

Resultado:
- `Total: 9`
- `Superado: 9`
- `Fallido: 0`

