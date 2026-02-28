# OPSXJ Backend Rules

Use these rules for OpenSpec update requests created with `opsxj:new`.

## Repository Requirements

- Use `DocuArchi.Api` structure as the baseline for new backend development.
- Respect existing Controllers organization.
- Ask for target route before creating new Controllers.
- Use `MiApp.DTOs` structure as the baseline for DTOs.
- Ask for target route before creating new DTOs.
- Use `MiApp.Models` structure as the baseline for Models.
- Ask for target route before creating new Models.
- Request table structure in requirements before creating a new model.
- Use `MiApp.Repository` structure as the baseline for repositories.
- Ask for target route before creating new repository functions.
- All functions must have interfaces to support unit testing.
- Use `MiApp.Services` structure as the baseline for services.
- Ask for target route before creating new service functions.
- All functions must have interfaces to support unit testing.

## General Requirements

- Register all interface-based functions in `DocuArchi.Api/Program.cs`.
- Services must be registered below `// Services (L)`.
- Repositories must be registered below `// Repositories (R)`.
- Keep interface and class in the same file following current project pattern.
- New APIs must follow: `ApiController + Service + AutoMapper + Repository`.
- Register mappings in `AutoMapperProfile` following current mapping style.
- APIs must return `AppResponses`.
- Wrap all functions in `try/catch`.
- Apply architecture principles: SoC, low coupling, high cohesion, SOLID.

## Testing Requirements

- Generate automated tests for .NET backend with Dapper + MySQL using real architecture:
  - `Controller -> Service -> Repo`
- Test types required:
  - Unit Tests (mock interfaces, do not mock Dapper)
  - Integration Tests (real MySQL with Testcontainers/Docker, include minimal `schema.sql` and `seed.sql`)
  - Contract Tests (OpenSpec)
- Validate response contract shape: `Success/message/errorMessage/data`.
- If Docker is unavailable, mark integration tests as skipped with explicit message.
- Keep interfaces in the same file as class.

## Minimum Cases

- Service Unit:
  - `Repo Success=false` or `Data=null` -> Service `Success=false` + empty data
  - `Repo Success=true` -> Service `Success=true` + `Message="OK"` + mapped data
  - `Repo throw` -> Service `Success=false` + `ErrorMessage=ex.Message` + empty data
- Repo Integration:
  - No restriction and `ValueAuto` matches -> list <=10 and concatenated `texValue`
  - `fecha_limite_acceso` past -> user omitted
  - `fecha_limite_acceso` future -> user included
  - no results -> `[{idValue:-1, texValue:""}]` with `Success=true`
- Contract:
  - `TramiteController` endpoints must validate `AppResponses<T>` shape
  - Validate behavior when claim `defaulalias` is missing

## Deliverables

- `tests/...UnitTests/*.cs`
- `tests/...IntegrationTests/*.cs`
- `tests/.../Database/schema.sql`
- `tests/.../Database/seed.sql`
- MySQL Testcontainers fixture
- `dotnet test` passing
