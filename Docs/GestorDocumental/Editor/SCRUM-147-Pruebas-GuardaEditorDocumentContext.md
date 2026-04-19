# SCRUM-147 — Pruebas: GuardaEditorDocumentContext (Catálogo)

## Unitarias — Controller

Archivo: `tests/TramiteDiasVencimiento.Tests/GuardaEditorDocumentContextControllerTests.cs`

- Claim `defaulalias` ausente → `400`
- Request inválido (DocumentId <= 0) → `400`
- Service OK → `200`

## Unitarias — Service

Archivo: `tests/TramiteDiasVencimiento.Tests/ServiceGuardaEditorDocumentContextTests.cs`

- `ContextCode` inexistente/inactivo → error controlado
- `requires_unique_entity` violado → `Validation`
- OK → invoca repositorio y retorna `success=true`

## Integración (pendiente)

Recomendado con MySQL Testcontainers/Docker:

- resolución real de `ContextCode` en `ra_editor_context_definitions`
- insert real en `ra_editor_document_context`
- idempotencia (reintento no duplica)
- validación `requires_unique_entity` en datos reales

