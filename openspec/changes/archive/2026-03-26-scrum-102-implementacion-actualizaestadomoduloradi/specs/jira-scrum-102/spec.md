## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-102.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-102

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow task registration updates module state
The system MUST update `ra_rad_estados_modulo_radicacion.estado` after registering a workflow task in `RegistrarRadicacionEntranteAsync`.

#### Scenario: Workflow task registered successfully
- **GIVEN** `RegistrarTareaWorkflowInternaAsync` returns a valid task
- **WHEN** `ReturnRegistraRadicacion.IdEstadoRadicado` is available
- **THEN** the service calls `ActualizaEstadoModuloRadicacio(idEstadoRadicado, 1, defaulalias)`

#### Scenario: State update fails
- **GIVEN** `ActualizaEstadoModuloRadicacio` returns an unsuccessful response
- **WHEN** the service processes the workflow branch
- **THEN** it returns a controlled error and stops the flow
