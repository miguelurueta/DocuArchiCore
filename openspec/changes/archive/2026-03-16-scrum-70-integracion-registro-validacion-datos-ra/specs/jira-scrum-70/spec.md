## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-70.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-70

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow pre-validation before radicacion registration
`RegistrarRadicacionEntranteAsync` MUST execute the workflow pre-validation chain before persisting the radicado when `util_tipo_modulo_envio == 2`.

#### Scenario: util_tipo_modulo_envio different from 2
- **GIVEN** a valid radicacion request
- **AND** `util_tipo_modulo_envio != 2`
- **WHEN** `RegistrarRadicacionEntranteAsync` is executed
- **THEN** the existing registration flow continues without calling the workflow route, assignment or workflow validation services

#### Scenario: Workflow claim is missing
- **GIVEN** a valid radicacion request
- **AND** `util_tipo_modulo_envio == 2`
- **AND** the authenticated user does not provide claim `defaulaliaswf`
- **WHEN** `RegistrarRadicacionEntranteAsync` is executed
- **THEN** the service returns a controlled validation error
- **AND** the radicado is not registered

#### Scenario: Workflow route is resolved and validated before registration
- **GIVEN** a valid radicacion request
- **AND** `util_tipo_modulo_envio == 2`
- **AND** claim `defaulaliaswf` exists
- **AND** there is an active workflow route
- **WHEN** `RegistrarRadicacionEntranteAsync` is executed
- **THEN** the service consumes `SolicitaEstructuraRutaWorkflow(defaulaliaswf)`
- **AND** builds `nombreRuta = "dat_adic_tar" + Nombre_Ruta`
- **AND** resolves the relation between plantilla and ruta
- **AND** executes assignment and workflow validation before registration

#### Scenario: Workflow validation fails
- **GIVEN** a valid radicacion request
- **AND** `util_tipo_modulo_envio == 2`
- **AND** the workflow route or relation data cannot be resolved, assigned or validated
- **WHEN** `RegistrarRadicacionEntranteAsync` is executed
- **THEN** the service returns a controlled functional error
- **AND** the radicado is not registered
