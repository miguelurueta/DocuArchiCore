## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-171.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-171

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Storage Engine final API endpoint
The API MUST expose `POST /api/gestor-documental/almacenamiento` through `AlmacenamientoDocumentalController` and delegate business execution to `IAlmacenarDocumentoUseCase`.

#### Scenario: Successful storage request
- **GIVEN** `defaulalias` and `usuarioid` claims are valid and `StorageEngineV2` is enabled
- **WHEN** the controller receives a valid `AlmacenarDocumentoRequest`
- **THEN** it calls `IAlmacenarDocumentoUseCase.ExecuteAsync`
- **AND** returns `200 OK` with `AppResponses<AlmacenarDocumentoResponse>`

### Requirement: Claims and feature-flag gate
The endpoint MUST enforce claims (`defaulalias`, `usuarioid`) and a feature gate (`StorageEngineV2`) before invoking the use case.

#### Scenario: Missing or invalid claims
- **WHEN** `defaulalias` is missing OR `usuarioid` is missing/invalid
- **THEN** the endpoint returns `400 BadRequest` with controlled `AppResponses`
- **AND** the use case is not executed

#### Scenario: StorageEngineV2 disabled
- **WHEN** `StorageEngineV2` is disabled
- **THEN** the endpoint returns `400 BadRequest`
- **AND** response includes `meta.status = "feature_disabled"`
- **AND** no repository or use case action is executed

### Requirement: Controlled error translation
The controller MUST not expose internal details in unhandled failures.

#### Scenario: Unexpected exception at API boundary
- **WHEN** an unhandled exception reaches the controller
- **THEN** it returns `500 InternalServerError` with controlled `AppResponses`
- **AND** it does not expose stack trace, SQL text or physical storage paths

### Requirement: Technical closure documentation
The change MUST include final technical documentation for architecture, implementation, integration, tests, observability, runbook, security, debt and metadata under `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine`.

#### Scenario: Reviewer checks closure artifacts
- **WHEN** reviewer inspects storage-engine docs for SCRUM-171
- **THEN** all required technical files for final closure exist with SCRUM-171 scope
