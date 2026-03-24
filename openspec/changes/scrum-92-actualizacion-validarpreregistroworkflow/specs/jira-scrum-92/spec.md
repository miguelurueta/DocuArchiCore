## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-92.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-92

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Pre-registro workflow returns resolved route
`ValidarPreRegistroWorkflowAsync` MUST return the resolved `SolicitaEstructuraRutaWorkflowDto` when the workflow pre-validation succeeds.

#### Scenario: Workflow pre-registro succeeds with active route
- **WHEN** the service resolves an active workflow route and validates mapped workflow fields
- **THEN** the response is `success = true`
- **AND** `data.RutaWorkflow` contains the resolved route with `id_Ruta > 0`
- **AND** `data.NombreRuta` remains available for downstream workflow checks

### Requirement: Registro workflow blocks when pre-registro route is missing
`RegistrarRadicacionEntranteAsync` MUST stop the workflow registration flow when `tipoModuloRadicacion = 2` and the workflow pre-validation does not return a valid route.

#### Scenario: Workflow pre-registro returns no route
- **WHEN** `tipoModuloRadicacion = 2`
- **AND** `ValidarPreRegistroWorkflowAsync` returns `success = true`
- **AND** `data.RutaWorkflow` is `null` or has `id_Ruta <= 0`
- **THEN** `RegistrarRadicacionEntranteAsync` returns a controlled validation error
- **AND** the repository registration call is not executed

### Requirement: Registro no workflow validates internal route before user workflow
`RegistrarRadicacionEntranteAsync` MUST resolve the internal workflow route before validating the internal workflow user when `tipoModuloRadicacion != 2`.

#### Scenario: Internal route does not exist
- **WHEN** `tipoModuloRadicacion != 2`
- **AND** the internal route lookup does not return an active route
- **THEN** `RegistrarRadicacionEntranteAsync` returns a controlled validation error
- **AND** the internal workflow user lookup is not executed
