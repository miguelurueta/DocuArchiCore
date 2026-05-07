## ADDED Requirements

### Requirement: Legacy-based final audit scope
The final Storage Engine audit MUST use the consolidated legacy function as functional source of truth.

#### Scenario: Functional baseline is established
- **GIVEN** the audit process for SCRUM-192 starts
- **WHEN** baseline behavior is defined
- **THEN** baseline is extracted from `funcion-almacena-consolidad.txt`
- **AND** prompt/document interpretation cannot override observed legacy behavior

### Requirement: Mandatory parity matrix
The audit MUST produce a complete parity matrix for legacy vs C# behavior.

#### Scenario: Matrix completeness and traceability
- **WHEN** matrix is reviewed
- **THEN** each row contains behavior, legacy source function, C# equivalent, state, severity, evidence, risk and closure action
- **AND** each row references a concrete test case and test result

### Requirement: Allowed parity states and critical gate
Parity classification MUST use controlled states and critical gating.

#### Scenario: Critical non-compliance
- **WHEN** at least one behavior is classified as `NO CUMPLE` with critical severity
- **THEN** final decision is automatically `NO GO`
- **AND** closure cannot be marked as complete

### Requirement: E2E evidence for critical flows
The audit MUST include E2E evidence for critical storage flows and controlled failure flows.

#### Scenario: End-to-end evidence set
- **WHEN** E2E validation is executed
- **THEN** evidence exists for simple storage, preindex TXT/XMLS, inventory/TRD, expediente/unidad, workflow/log
- **AND** evidence exists for DB failure, FS failure, XML failure and concurrency behavior

### Requirement: Observability and safety verification
The audit MUST verify observability, fallback controls and implementation hygiene.

#### Scenario: Operational controls review
- **WHEN** technical controls are reviewed
- **THEN** logs include request correlation and key IDs (`requestId`, `idAlmacen`, `idRegistroProduccionDocumental`)
- **AND** `StorageEngineV2` flag and rollback path are validated
- **AND** no active stubs/placeholders remain in critical flow

### Requirement: Enterprise documentation closure
The audit MUST update SCRUM-189 closure documents as official evidence set.

#### Scenario: Documentation package availability
- **WHEN** release board reviews SCRUM-192
- **THEN** updated documents exist for architecture, matrix, E2E tests, risks/deviations, go-no-go and metadata
- **AND** documents are internally consistent with OpenSpec conclusions

### Requirement: Final deployment decision
The audit MUST emit one explicit final decision with rationale.

#### Scenario: Decision issuance
- **WHEN** parity matrix and evidence are consolidated
- **THEN** decision is one of `GO`, `GO CONDICIONADO` or `NO GO`
- **AND** decision includes residual risks, mitigation and follow-up actions
