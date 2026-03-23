# jira-scrum-85 Specification

## Purpose
TBD - created by archiving change scrum-85-actualizacion-validaractividadiniciofluj. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-85.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-85

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow start-activity validation returns the resolved model
`ValidarActividadInicioFlujoAsync` MUST resolve and return `SolicitaDatosActividadInicioFlujo` when a workflow flow id is present in `RegistrarRadicacionEntranteAsync`.

#### Scenario: Workflow start activity exists
- **WHEN** `RegistrarRadicacionEntranteAsync` executes with `tipoModuloRadicacion = 2`
- **AND** `RE_flujo_trabajo.id_tipo_flujo_workflow > 0`
- **AND** the repository returns a valid start activity
- **THEN** `ValidarActividadInicioFlujoAsync` returns `success = true`
- **AND** `data` contains `SolicitaDatosActividadInicioFlujo`
- **AND** the registration flow continues to workflow prevalidation

### Requirement: Workflow start-activity validation stops the flow when no valid model exists
`RegistrarRadicacionEntranteAsync` MUST stop the workflow registration flow when `ValidarActividadInicioFlujoAsync` resolves no valid start activity model.

#### Scenario: Workflow start activity is missing
- **WHEN** `ValidarActividadInicioFlujoAsync` returns `success = true`
- **AND** `data = null`
- **THEN** `RegistrarRadicacionEntranteAsync` returns a controlled validation response
- **AND** the registration repository is not executed

### Requirement: Workflow start-activity validation handles exceptions in the service layer
`ValidarActividadInicioFlujoAsync` MUST wrap unexpected failures in a controlled `AppResponses` error response.

#### Scenario: Service dependency throws during workflow start-activity validation
- **WHEN** `ValidarActividadInicioFlujoAsync` encounters an unexpected exception
- **THEN** the response returns `success = false`
- **AND** the error is reported with `Field = "ValidarActividadInicioFlujoAsync"`
- **AND** `RegistrarRadicacionEntranteAsync` stops before workflow prevalidation and final registration

