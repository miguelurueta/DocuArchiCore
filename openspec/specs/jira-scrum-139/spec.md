# jira-scrum-139 Specification

## Purpose
TBD - created by archiving change scrum-139-implementacion-api-configuracion-upload. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-139.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-139

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Configuración Upload API by `NAME_PROCESO`
The system MUST expose an API endpoint to query `ra_config_upload_gestion` filtered only by `NAME_PROCESO`, using the existing repository via a service layer.

#### Scenario: Route and querystring contract
- **WHEN** the API is implemented
- **THEN** it exposes `GET /api/gestor-documental/configuracion-upload?nameProceso={NAME_PROCESO}`
- **AND** `nameProceso` is required (querystring)
- **AND** it returns `AppResponses<List<RaConfiguracionUploadModel>>`

#### Scenario: Claim requirement
- **WHEN** the request is handled
- **THEN** the controller validates claim `defaulalias` via `IClaimValidationService`
- **AND** it returns `BadRequest(validation.Response)` when the claim is missing/invalid

#### Scenario: Validation for `nameProceso`
- **WHEN** `nameProceso` is null/empty/whitespace
- **THEN** the controller returns `success = false` in `AppResponses` with `data = []` and a validation error

#### Scenario: Service and repository delegation
- **WHEN** both claim and `nameProceso` are valid
- **THEN** the controller calls `IServiceSolicitaEstructuraConfiguracionUpload`
- **AND** the service calls `ISolicitaEstructuraConfiguracionUploadNameProcesoRepository`
- **AND** the repository query filters only by `NAME_PROCESO`

#### Scenario: Successful response
- **WHEN** the underlying repository returns `success = true`
- **THEN** the controller returns `Ok(result)`

#### Scenario: Error response
- **WHEN** the underlying service/repository returns `success = false`
- **THEN** the controller returns `BadRequest(result)`

