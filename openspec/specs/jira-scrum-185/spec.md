# jira-scrum-185 Specification

## Purpose
TBD - created by archiving change scrum-185-implementacion-indice-expediente-electro. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-185.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-185

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Legacy XML index path resolution
The system MUST resolve expediente XML index route using legacy route source and zero-fill conventions.

#### Scenario: Resolve route from legacy table
- **GIVEN** a valid `IdExpediente` and active storage process
- **WHEN** XML index route is requested
- **THEN** the service reads legacy route data (`ra_ruta_expediente`)
- **AND** builds `<RUTA>\<DISCO_9_DIGITS>\<ID_EXPEDIENTE_9_DIGITS>.xml`

### Requirement: Conditional execution for electronic expediente
The system MUST update expediente XML index only when legacy conditions are satisfied.

#### Scenario: XML update is applicable
- **GIVEN** `ExpedienteUnidadResult.TieneExpediente = true`
- **AND** `ExpedienteUnidadResult.EstadoExpedienteElectronico = 2`
- **AND** `IdRegistroProduccionDocumental > 0`
- **WHEN** physical phase completes
- **THEN** XML index update executes

#### Scenario: XML update is not applicable
- **WHEN** expediente conditions are not satisfied
- **THEN** the service returns a non-updating state (`NO_EXPEDIENTE`/equivalent)
- **AND** does not fail the physical phase

### Requirement: Post-commit inconsistency handling
If XML index append fails after physical document persistence, the system MUST keep the storage result completed and expose inconsistency metadata.

#### Scenario: XML writer fails post-commit
- **GIVEN** physical file and main XML were persisted
- **WHEN** expediente XML index update throws an exception
- **THEN** the physical status remains `Completed`
- **AND** `InconsistenciaPostCommit` is populated
- **AND** XML index result state is `POST_COMMIT_INCONSISTENCY`

