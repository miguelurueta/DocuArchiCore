# jira-scrum-117 Specification

## Purpose
TBD - created by archiving change scrum-117-crear-repositorio-configuracionlistausua. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-117.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-117

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow user list configuration repository
The system MUST expose a repository that returns the workflow user list configuration from `configuracion_usuario` by `Usuario_Workflow_idU_suario`.

#### Scenario: Existing configuration is found
- **WHEN** `SolicitaConfiguracionListaUsuarioWorkflowRepository` is invoked with a valid `idUsuarioWorkflow` and `defaultDbAlias`
- **THEN** it returns `success = true`
- **AND** `message = "YES"`
- **AND** `data` contains the typed `configuracionUsuarioDTO` record

#### Scenario: No configuration exists
- **WHEN** the repository is invoked and no rows match `Usuario_Workflow_idU_suario`
- **THEN** it returns `success = true`
- **AND** `message = "Sin resultados"`
- **AND** `data = null`

#### Scenario: Invalid input is provided
- **WHEN** `idUsuarioWorkflow` is lower than or equal to zero or `defaultDbAlias` is blank
- **THEN** it returns `success = false`
- **AND** the response includes validation errors

#### Scenario: Repository query fails
- **WHEN** the query engine returns an error or throws an exception
- **THEN** it returns `success = false`
- **AND** the response includes an `AppError` with the controlled message

