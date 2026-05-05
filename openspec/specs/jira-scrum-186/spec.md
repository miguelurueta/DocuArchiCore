# jira-scrum-186 Specification

## Purpose
TBD - created by archiving change scrum-186-implementacion-log-docuarchi-almacenamie. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-186.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-186

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow log activation rule
The system MUST insert `logdocuarchi` only when workflow task is present and greater than zero.

#### Scenario: Workflow absent or disabled
- **WHEN** command workflow is null, missing task id, or task id <= 0
- **THEN** workflow log is not inserted

#### Scenario: Workflow enabled
- **WHEN** `Workflow.IdTareaWorkflow > 0`
- **THEN** workflow log is built and inserted inside current DB transaction

### Requirement: Legacy-compatible workflow log mapping
Workflow log fields MUST preserve VB-compatible semantics for route, typology, fields, and identity.

#### Scenario: Build legacy log payload
- **WHEN** workflow logging applies
- **THEN** `id_tran` uses storage identity (`IdAlmacen`)
- **AND** `RUT_DOCU` uses physical final route plus DIG filename
- **AND** `TIPOLOGIA_DOCUMENTAL` uses document typology description (not numeric id)
- **AND** `CAMPOS` uses legacy format `|valor1|valor2|...`

### Requirement: HTTP IP propagation to storage context
The API MUST resolve requester IP and propagate it into storage context for workflow logging.

#### Scenario: Resolve IP from HTTP context
- **WHEN** storage endpoint is called
- **THEN** controller resolves IP using standard proxy-aware helper
- **AND** use case receives and stores the value in storage context (`IpTrans`)
- **AND** workflow log uses `IP_TRANS` from this propagated value

