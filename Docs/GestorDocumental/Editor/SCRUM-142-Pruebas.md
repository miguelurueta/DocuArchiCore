# SCRUM-142 — Pruebas

## Unitarias

- Controller:
  - claim inválido → 400
  - DocumentId inválido → 400
  - success → 200
- Service:
  - validaciones básicas
  - propaga error de repo
  - OK cuando repo OK

Archivos:
- `tests/TramiteDiasVencimiento.Tests/SincronizaEditorDocumentImagesControllerTests.cs`
- `tests/TramiteDiasVencimiento.Tests/ServiceSincronizaEditorDocumentImagesTests.cs`

## Integración (pendiente / skipped)

- `tests/TramiteDiasVencimiento.Tests/SincronizaEditorDocumentImagesRepositoryIntegrationTests.cs`

