## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-188.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-188

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Scope guard for duplicate legacy-option work
SCRUM-188 MUST avoid duplicating already-merged runtime logic for legacy `system1` options and MUST limit itself to traceability and technical evidence in this repository.

#### Scenario: Reviewer validates no duplicated runtime delta
- **WHEN** the changed files are inspected in DocuArchiCore
- **THEN** only OpenSpec/documentation artifacts are modified for SCRUM-188
- **AND** there are no additional runtime changes that re-implement SCRUM-181 behavior

### Requirement: Enterprise technical documentation for SCRUM-188
The change MUST provide the StorageEngine technical package under the official docs route with architecture, implementation, testing, observability, legacy regression and metadata artifacts.

#### Scenario: Docs package exists in expected route
- **WHEN** a reviewer checks `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine`
- **THEN** SCRUM-188 documents exist with the six required artifacts for audit and publication
