# jira-scrum-156 Specification

## Purpose
TBD - created by archiving change scrum-156-crea-api-solicita-correo-electronico-rem. Update Purpose after archive.
## Requirements
### Requirement: API route and contract
The system MUST expose a GET endpoint to resolve the sender email from dynamic template configuration.

#### Scenario: Valid request returns AppResponses<string>
- **GIVEN** claim `defaulalias` is present
- **WHEN** calling `GET /api/GestionCorrespondencia/PlantillaValidacion/solicita-correo-electronico-remitente?idPlantillaRadicado=10&idDestinatarioExterno=450`
- **THEN** the response is `AppResponses<string>`
- **AND** `success=true` and `meta.status="success"` when `data != ""`
- **AND** `success=true` and `meta.status="empty"` when `data == ""`
- **AND** `success=false` and `meta.status="error"` when an error occurs

#### Scenario: Missing claim returns error
- **WHEN** claim `defaulalias` is missing
- **THEN** the API returns `400 BadRequest` with `AppResponses<string>` and `success=false`

#### Scenario: Invalid query params return validation error
- **WHEN** `idPlantillaRadicado <= 0` OR `idDestinatarioExterno <= 0`
- **THEN** the API returns `400 BadRequest` with `AppResponses<string>` and `success=false`

### Requirement: Dynamic query safety
The system MUST validate any dynamic table/column identifiers before executing SQL.

#### Scenario: Identifiers validated before querying data
- **GIVEN** dynamic identifiers (table, email column, PK column) are resolved from configuration tables
- **WHEN** executing the final data query
- **THEN** identifiers match a safe regex (alphanumeric + underscore)
- **AND** existence is checked against `INFORMATION_SCHEMA` in the current database

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-156.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-156

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

