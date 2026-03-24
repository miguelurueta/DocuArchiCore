## ADDED Requirements

### Requirement: RegistrarRadicacionEntranteAsync validates internal workflow user
`RegistrarRadicacionEntranteAsync` MUST validate the internal workflow user before persisting the radicado when `tipoModuloRadicacion != 2`.

#### Scenario: Internal workflow user exists
- **WHEN** the service receives a valid non-workflow request
- **AND** the destinatario has `Relacion_Workflow > 0`
- **AND** claim `defaulaliaswf` is present
- **AND** `SolicitaEstructuraIdUsuarioWorkflowId(Relacion_Workflow, defaulaliaswf)` returns a valid `UsuarioWorkflow`
- **THEN** the service continues with `_registrarRepository.RegistrarRadicacionEntranteAsync`
- **AND** the workflow user result is used only as internal flow data

#### Scenario: Missing workflow claim
- **WHEN** the service receives a valid non-workflow request
- **AND** claim `defaulaliaswf` is empty or missing
- **THEN** the service returns a controlled validation error
- **AND** `_registrarRepository.RegistrarRadicacionEntranteAsync` is not executed

#### Scenario: Destinatario without workflow relation
- **WHEN** the service receives a valid non-workflow request
- **AND** the destinatario has `Relacion_Workflow <= 0`
- **THEN** the service returns a controlled validation error
- **AND** `_registrarRepository.RegistrarRadicacionEntranteAsync` is not executed

#### Scenario: Workflow user lookup fails
- **WHEN** `SolicitaEstructuraIdUsuarioWorkflowId` returns `Success = false`
- **OR** returns no valid `UsuarioWorkflow`
- **THEN** the service returns a controlled workflow error
- **AND** `_registrarRepository.RegistrarRadicacionEntranteAsync` is not executed

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Constraints remain explicit
- **WHEN** proposal, design and tests are reviewed
- **THEN** they reference `openspec/context/OPSXJ_BACKEND_RULES.md`
- **AND** preserve AppResponses, try/catch and repository-driven access rules
