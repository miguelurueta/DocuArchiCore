# SCRUM-150 — Pruebas: TemplateDefinitions

## Unitarias — Controller

Archivo: `tests/TramiteDiasVencimiento.Tests/TemplateDefinitionsControllerTests.cs`

- claim inválido → 400
- request inválido → 400

## Unitarias — Service

Archivo: `tests/TramiteDiasVencimiento.Tests/ServiceTemplateDefinitionsTests.cs`

- alias inválido → no invoca repo

## Integración (pendiente)

Recomendado con MySQL Testcontainers/Docker:

- inserción real de definición
- inserción real de versión
- consulta por código

