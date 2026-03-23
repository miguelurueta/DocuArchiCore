# jira-scrum-87 Specification

## Purpose
TBD - created by archiving change scrum-87-implementacion-solicitaexistenciaradicad. Update Purpose after archive.
## Requirements
### Requirement: Workflow existence lookup after radicado registration
`RegistrarRadicacionEntranteAsync` MUST execute `SolicitaExistenciaRadicadoRutaWorkflowAsync` immediately after the repository registration step when the workflow module applies.

#### Scenario: Workflow module triggers existence lookup
- **GIVEN** `RegistrarRadicacionEntranteAsync` is processing a request with `tipoModuloRadicacion = 2`
- **AND** workflow prevalidation succeeded with a resolved `NombreRuta`
- **AND** repository registration returned a valid radicado consecutivo
- **WHEN** the service continues the workflow flow
- **THEN** it calls `SolicitaExistenciaRadicadoRutaWorkflowAsync`
- **AND** it passes `workflowValidation.NombreRuta` as route name
- **AND** it resolves `defaulaliaswf` from current claims as workflow alias

#### Scenario: Workflow lookup stays internal
- **WHEN** the lookup succeeds
- **THEN** the result is stored only in an internal variable inside the service flow
- **AND** the public response contract of `RegistrarRadicacionEntranteAsync` is not expanded with workflow existence fields

#### Scenario: Workflow lookup failure stops the flow
- **GIVEN** `SolicitaExistenciaRadicadoRutaWorkflowAsync` returns `success = false`
- **WHEN** `RegistrarRadicacionEntranteAsync` processes the workflow path
- **THEN** the service returns `success = false`
- **AND** propagates the controlled message and errors from the workflow lookup

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Missing implementation constraints
- **WHEN** proposal, design and tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, `AppResponses`/`try-catch` and test requirements

