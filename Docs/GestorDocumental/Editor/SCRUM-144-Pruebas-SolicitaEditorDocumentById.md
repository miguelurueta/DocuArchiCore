# SCRUM-144 — Pruebas: `SolicitaEditorDocumentById`

## Unit tests

- `tests/TramiteDiasVencimiento.Tests/ServiceSolicitaEditorDocumentByIdTests.cs`
  - valida `documentId` inválido
  - valida `defaultDbAlias` vacío
  - valida propagación de respuesta OK desde repositorio

## Notas

- `dotnet test` en la solución de tests reporta fallos existentes en `TramiteControllerContractTests` (retornos `ObjectResult` vs `OkObjectResult/BadRequestObjectResult`) ajenos a este ticket.
- Pruebas de integración/contrato específicas del repositorio pueden agregarse cuando haya acceso a DB de test / entorno Docker según estándar del repo.
