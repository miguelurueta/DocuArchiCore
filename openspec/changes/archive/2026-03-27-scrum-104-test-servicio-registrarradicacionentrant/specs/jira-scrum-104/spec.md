## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-104.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-104

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Service test coverage for RegistrarRadicacionEntranteAsync
The solution MUST protect `RegistrarRadicacionEntranteAsync` with deterministic unit tests focused on validation, workflow prechecks and controlled error handling.

#### Scenario: Null request is rejected
- **WHEN** `RegistrarRadicacionEntranteAsync` receives `request = null`
- **THEN** it returns `AppResponses` with `success=false`
- **AND** the response identifies the `request` field as validation error
- **AND** the repository registration flow is not executed

#### Scenario: Workflow pre-registration fails
- **WHEN** `ValidaPreRegistroWorkflowAsync(...)` returns `success=false`
- **THEN** the service returns a controlled error response preserving the workflow error
- **AND** `_registrarRepository.RegistrarRadicacionEntranteAsync(...)` is not executed

#### Scenario: Workflow task registration dependency is missing
- **WHEN** the workflow flow requires registering a new task and `IRegistroRadicadoTareaWorkflowRepository` is not configured
- **THEN** the service returns a controlled dependency error
- **AND** the response explains that `RegistrarTareaWorkflowAsync` is required

#### Scenario: Workflow claim is missing during task registration
- **WHEN** the workflow flow requires registering a new task and claim `defaulaliaswf` is empty
- **THEN** the service returns a controlled validation error
- **AND** the workflow task repository is not invoked

### Requirement: Coverage evidence is documented
The change MUST document the covered scenarios and the test execution evidence for SCRUM-104.

#### Scenario: Reviewer inspects technical evidence
- **WHEN** a reviewer opens the SCRUM-104 technical note
- **THEN** the document lists covered unit scenarios
- **AND** it records the test command executed and its result
- **AND** it states any remaining integration-test limitations explicitly
