## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-91.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-91

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarRadicacionEntranteService must preserve the internal workflow user
`RegistrarRadicacionEntranteService` MUST keep the `UsuarioWorkflow` resolved by `ValidarUsuarioWorkflowInternoAsync` available in the main flow before the final registration call.

#### Scenario: Internal workflow user is resolved successfully
- **WHEN** `tipoModuloRadicacion != 2`
- **AND** `ValidarUsuarioWorkflowInternoAsync` resolves a valid `UsuarioWorkflow`
- **THEN** `RegistrarRadicacionEntranteAsync` stores that object in the main flow
- **AND** only continues to `_registrarRepository.RegistrarRadicacionEntranteAsync` when the workflow user is not null

#### Scenario: Internal workflow user is missing
- **WHEN** `tipoModuloRadicacion != 2`
- **AND** `ValidarUsuarioWorkflowInternoAsync` cannot resolve a valid `UsuarioWorkflow`
- **THEN** `RegistrarRadicacionEntranteAsync` returns a controlled validation error
- **AND** does not call `_registrarRepository.RegistrarRadicacionEntranteAsync`

### Requirement: ValidarUsuarioWorkflowInternoAsync must return controlled failures
`ValidarUsuarioWorkflowInternoAsync` MUST wrap repository exceptions and functional failures inside controlled `AppResponses`.

#### Scenario: Workflow lookup throws an exception
- **WHEN** the workflow repository throws during `SolicitaEstructuraIdUsuarioWorkflowId`
- **THEN** `ValidarUsuarioWorkflowInternoAsync` returns `success = false`
- **AND** the error field is `ValidarUsuarioWorkflowInternoAsync`
- **AND** the registration flow stops without persisting the radicado
