# SCRUM-148 — Pruebas: Full Save Editor Document (Catálogo)

## Unitarias — Controller

Archivo: `tests/TramiteDiasVencimiento.Tests/FullSaveEditorDocumentControllerTests.cs`

- Claim `defaulalias` ausente → `400`
- Request inválido → `400`
- Service OK → `200`

## Unitarias — Service (orquestación + rollback)

Archivo: `tests/TramiteDiasVencimiento.Tests/ServiceFullSaveEditorDocumentTests.cs`

- Alias inválido → no abre conexión
- Catálogo inválido → rollback y no invoca contexto/imágenes

## Integración (pendiente)

Recomendado con MySQL Testcontainers/Docker:

- persistencia real en `ra_editor_documents`
- lectura real en `ra_editor_context_definitions`
- persistencia real en `ra_editor_document_context`
- sincronización real en `ra_editor_document_image_links`
- rollback real ante fallo simulado en un paso

