# jira-scrum-187 Specification

## Purpose
TBD - created by archiving change scrum-187-implementacion-validacion-final-almacena. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-187.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-187

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Storage Engine parity suite
The system MUST provide a parity-oriented automated suite comparing expected legacy behavior and current C# behavior for storage-critical outputs.

#### Scenario: Core storage parity
- **WHEN** the parity test suite executes
- **THEN** it validates identity, disk/folder values, pages, format, size legacy string, and DIG/FXL naming equivalence

#### Scenario: Workflow log parity
- **WHEN** workflow storage logging is active
- **THEN** parity tests validate critical `logdocuarchi` fields (`id_tran`, `RUT_DOCU`, `GABINETE`, `ID_TAREA_WF`, `ID_RUTA_WF`, `TIPOLOGIA_DOCUMENTAL`, `IP_TRANS`)

#### Scenario: XML and system increments parity
- **WHEN** XML/DB parity assertions run
- **THEN** tests validate required FXL attributes and expected `system1` increment structure (`proxid`, `numpag_carp`)

### Requirement: Enterprise parity documentation
The change MUST include technical parity documentation for evidence, matrix, residual gaps, and go/no-go decision support.

#### Scenario: Documentation package completeness
- **WHEN** reviewing SCRUM-187 artifacts
- **THEN** the StorageEngine docs folder includes test plan, parity matrix, evidence summary, residual gaps, runbook, go/no-go and metadata files

