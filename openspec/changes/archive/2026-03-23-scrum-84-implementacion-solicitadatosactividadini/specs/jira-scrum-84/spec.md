## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-84.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-84

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow initial activity gate in RegistrarRadicacionEntranteAsync
When the radicacion flow runs in workflow mode and a workflow flow id is present, the system MUST resolve the initial workflow activity before continuing the main registration flow.

#### Scenario: Workflow request with configured flow id
- **WHEN** `RegistrarRadicacionEntranteAsync` receives `tipoModuloRadicacion = 2`
- **AND** `RE_flujo_trabajo.id_tipo_flujo_workflow > 0`
- **THEN** the service consults `SolicitaDatosActividadInicioFlujoAsync` before calling the registration repository

#### Scenario: Initial activity not found
- **WHEN** `SolicitaDatosActividadInicioFlujoAsync` returns success without activity ids
- **THEN** the service returns a controlled validation response
- **AND** the registration repository is not executed

#### Scenario: Initial activity exists
- **WHEN** `SolicitaDatosActividadInicioFlujoAsync` returns a valid activity
- **THEN** the workflow prevalidation continues
- **AND** the registration flow can continue normally

### Requirement: DI and automated coverage for workflow initial activity
The repository integration for the workflow initial activity MUST be wired in the API composition root and covered with automated tests.

#### Scenario: API startup resolves the repository dependency
- **WHEN** `DocuArchi.Api` starts
- **THEN** `ISolicitaDatosActividadInicioFlujoRepository` is registered in `DocuArchi.Api/Program.cs`

#### Scenario: Service tests cover the new workflow branch
- **WHEN** the unit test suite for `RegistrarRadicacionEntranteService` runs
- **THEN** it covers non-workflow requests, workflow requests without initial activity, and workflow requests with a valid initial activity
