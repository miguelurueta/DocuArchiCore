# jira-scrum-190 Specification

## Purpose
TBD - created by archiving change scrum-190-implementacion-cierre-orquestador-docume. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-190.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-190

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Legacy runtime parity validation
The SCRUM-190 implementation MUST validate the runtime orchestration order against the legacy source function.

#### Scenario: Orchestrator executes runtime phases in legacy-compatible order
- **GIVEN** the legacy source `D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt`
- **WHEN** `DocumentStorageOrchestrator` executes a successful flow
- **THEN** it runs validation first, then metadata analysis, then transaction, then physical phase, and returns final result with `IdAlmacen > 0`

#### Scenario: Runtime stops on validation failure
- **WHEN** validation phase returns errors
- **THEN** the flow stops before transaction and physical phases
- **AND** an error is propagated as `StorageValidationException`

#### Scenario: Runtime does not allow stub output
- **WHEN** the flow ends successfully
- **THEN** it MUST NOT return `IdAlmacen = 0`
- **AND** it MUST NOT return `Pending` as final state

