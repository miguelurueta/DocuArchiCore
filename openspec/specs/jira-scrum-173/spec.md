# jira-scrum-173 Specification

## Purpose
TBD - created by archiving change scrum-173-crea-api-solicita-tipos-respuesta. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-173.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-173

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Catalog endpoint for tipos de respuesta
The backend MUST expose `GET /api/gestion-correspondencia/tipos-respuesta` returning `AppResponses<List<ResponseDrowp>>` for active catalog values.

#### Scenario: Active rows are available
- **GIVEN** records exist in `ra_tipo_respuesta` with `estado = 1`
- **WHEN** a client calls `GET /api/gestion-correspondencia/tipos-respuesta`
- **THEN** response is `200 OK` with `Success = true`
- **AND** `Data` contains only `ID` and `Descripcion`
- **AND** rows are ordered by `orden ASC`

#### Scenario: No active rows are available
- **GIVEN** no records exist with `estado = 1`
- **WHEN** a client calls `GET /api/gestion-correspondencia/tipos-respuesta`
- **THEN** response is `200 OK` with `Success = true`
- **AND** `Data` is an empty list
- **AND** `Meta.Status` is `empty`

### Requirement: Claim-based alias validation
The endpoint MUST use `defaulalias` claim validated by `IClaimValidationService` and MUST NOT accept alias via query/body.

#### Scenario: Missing or invalid defaulalias claim
- **WHEN** request does not provide a valid `defaulalias` claim
- **THEN** controller returns `400 BadRequest`
- **AND** service/repository execution is not performed

### Requirement: Repository data-access policy
Repository implementation MUST use `DapperCrudEngine` with `QueryOptions` and MUST NOT use manual SQL or direct `IDbConnection` query execution.

#### Scenario: Query policy is enforced
- **WHEN** repository reads `ra_tipo_respuesta`
- **THEN** it builds `QueryOptions` with table `ra_tipo_respuesta`
- **AND** filters by `estado = 1`
- **AND** orders by `orden ASC`
- **AND** applies defensive limit `500`

### Requirement: Service response ownership and resilience
Service layer MUST own `AppResponses` construction, cache-first behavior, and controlled error semantics.

#### Scenario: Cache miss then DB fallback
- **GIVEN** catalog cache has no value for alias key
- **WHEN** service executes request
- **THEN** repository is called
- **AND** successful DB result is returned as `Success = true`
- **AND** cache is populated with configured TTL

#### Scenario: Cache provider failure
- **GIVEN** cache access throws an exception
- **WHEN** service executes request
- **THEN** flow continues with repository query
- **AND** endpoint does not fail only because cache is unavailable

#### Scenario: Repository failure
- **WHEN** repository throws an exception
- **THEN** service returns `Success = false`
- **AND** `Meta.Status` is `error`

### Requirement: Dependency registration and architecture boundaries
Implementation MUST keep Controller -> Service -> Repository boundaries and register interfaces in DI (`Program.cs`).

#### Scenario: Layering compliance
- **WHEN** endpoint implementation is reviewed
- **THEN** controller delegates business logic to service
- **AND** service delegates data access to repository
- **AND** repository returns domain DTO list without `AppResponses`

### Requirement: Test evidence for controller/service/repository
The change MUST include automated tests covering claim validation, response semantics, repository query options, and error handling.

#### Scenario: Test suite validates expected behavior
- **WHEN** unit/integration tests run for SCRUM-173 artifacts
- **THEN** controller tests validate claim handling and HTTP status mapping
- **AND** service tests validate cache hit/miss/failure paths
- **AND** repository tests validate `QueryOptions` configuration and non-SQL-manual policy

