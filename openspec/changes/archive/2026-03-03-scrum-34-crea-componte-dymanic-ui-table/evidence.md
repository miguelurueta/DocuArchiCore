# SCRUM-34 - Evidence Template

## PRs

- MiApp.DTOs: pending
- MiApp.Models: pending
- MiApp.Repository: pending
- MiApp.Services: pending
- DocuArchi.Api: pending
- DocuArchiCore: pending

## Build/Test

- dotnet build:
  - MiApp.DTOs: OK (Debug/net10.0)
  - MiApp.Models: OK (Debug/net10.0)
  - MiApp.Repository: OK (Debug/net10.0)
  - MiApp.Services: OK (Debug/net10.0)
  - DocuArchi.Api: OK (Debug/net10.0) after fixing Program.cs and stopping running process that locked DLLs
- dotnet test:
  - `TramiteDiasVencimiento.Tests` (filtro `DynamicUiTableServiceTests`): 4 passed, 0 failed
  - `TramiteDiasVencimiento.Tests` (suite existente previa): 21 passed, 0 failed
- unit tests agregados:
  - `tests/TramiteDiasVencimiento.Tests/DynamicUiTableServiceTests.cs`
- integration tests: pending

## OpenSpec

- validate: OK (`openspec validate scrum-34-crea-componte-dymanic-ui-table`)
- archive: pending

## Documentation

- `Docs/Radicacion/tramite/SCRUM-34-Diagramas.md`: creado
- `Docs/Radicacion/tramite/SCRUM-34-Integracion-Frontend.md`: creado

## Jira

- ticket: https://contasoftcompany.atlassian.net/browse/SCRUM-34
- status: pending
