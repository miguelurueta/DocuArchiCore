## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-103.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-103

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Update estado modulo with workflow task id
The system MUST update `ra_rad_estados_modulo_radicacion` with the new `id_tarea_workflow` value when `RegistrarRadicacionEntranteAsync` registers a new workflow task.

#### Scenario: Workflow task registration updates estado and task id
- **GIVEN** `RegistrarTareaWorkflowInternaAsync` returns `idTareaWorkflow > 0`
- **AND** `ReturnRegistraRadicacion.IdEstadoRadicado > 0`
- **WHEN** `RegistrarRadicacionEntranteAsync` calls `ActualizaEstadoModuloRadicacio`
- **THEN** the repository receives `defaultDbAlias`, `idEstadoRadicado`, `estado = 1` and `idTareaWorkflow`

#### Scenario: Missing workflow task id blocks update
- **GIVEN** `RegistrarTareaWorkflowInternaAsync` does not provide a valid `idTareaWorkflow`
- **WHEN** the service tries to update `ra_rad_estados_modulo_radicacion`
- **THEN** the service returns a controlled validation error

#### Scenario: Repository persists estado and task id together
- **WHEN** `ActualizaEstadoModuloRadicacio` executes successfully
- **THEN** it updates both `estado` and `id_tarea_workflow`
- **AND** returns `success = true`
