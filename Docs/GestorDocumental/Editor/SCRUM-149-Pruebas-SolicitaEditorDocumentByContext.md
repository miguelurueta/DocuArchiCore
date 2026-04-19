# SCRUM-149 — Pruebas: SolicitaEditorDocumentByContext (Catálogo)

## Unitarias — Controller

Archivo: `tests/TramiteDiasVencimiento.Tests/SolicitaEditorDocumentByContextControllerTests.cs`

- Claim `defaulalias` ausente → `400`
- Parámetros inválidos (`contextCode` vacío / `entityId <= 0`) → `400`
- Service OK → `200`

## Unitarias — Service

Archivo: `tests/TramiteDiasVencimiento.Tests/ServiceSolicitaEditorDocumentByContextTests.cs`

- Parámetros inválidos → `Validation`
- Catálogo inválido/inactivo → error controlado
- Repo OK → enriquece `Context` (ContextCode/EntityName/RelationType)

## Integración (pendiente)

Recomendado con MySQL Testcontainers/Docker:

- catálogo activo (`ra_editor_context_definitions`)
- relación activa (`ra_editor_document_context`)
- documento (`ra_editor_documents`)
- imágenes activas (links + images sin `deleted_at`)

