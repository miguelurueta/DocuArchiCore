## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-184.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-184

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Legacy-compatible expediente/unidad execution plan
Storage flow MUST build an execution plan for expediente/unidad based on real command data, resolved options, and physical metadata, matching VB decision semantics.

#### Scenario: AplicaUnidadConservacion disabled
- **GIVEN** `context.ResolvedOptions.AplicaUnidadConservacion = false`
- **WHEN** storage execution reaches expediente/unidad phase
- **THEN** no expediente/unidad persistence update is executed

#### Scenario: Ambiguous expediente and unidad selection
- **GIVEN** both `IdExpediente` and `IdUnidadConservacion` are provided
- **WHEN** the plan is built
- **THEN** execution fails with explicit ambiguity validation error

#### Scenario: Missing class document with expediente or unidad
- **GIVEN** expediente or unidad is selected
- **WHEN** `IdClaseDocumento` is missing or invalid
- **THEN** execution fails before persistence

### Requirement: Legacy-compatible folio updates
The implementation MUST update legacy folio counters using transactional locking and correct target fields.

#### Scenario: Expediente folio update
- **GIVEN** valid expediente with active state
- **WHEN** folios are applied
- **THEN** `NUMERO_ELECTRONICO_CONTENIDO` is incremented under `FOR UPDATE`

#### Scenario: Unidad digitalized folio update
- **GIVEN** unidad with `unidad_conserva_tipo = DIGITALIZADO`
- **WHEN** folios are applied
- **THEN** `NUMERO_DIGITALIZADO_CONTENIDO` is incremented

#### Scenario: Unidad electronic folio update
- **GIVEN** unidad with `unidad_conserva_tipo = ELECTRONICO`
- **WHEN** folios are applied
- **THEN** `NUMERO_ELECTRONICO_CONTENIDO` is incremented

#### Scenario: Forbidden legacy mismatch field
- **WHEN** unidad folios are updated
- **THEN** `NUMERO_FOLIO_UNIDAD_CONSERVACION` is not used as write target

### Requirement: TransactionCoordinator integration contract
TransactionCoordinator MUST execute expediente/unidad processing within the same open transaction and expose outcome for downstream inventory/index decisions.

#### Scenario: Result propagation to context and transaction result
- **WHEN** expediente/unidad execution completes
- **THEN** the resulting `IdTipoUnidadDocumental` and execution flags are available in `StorageContext` and `StorageTransactionResult`

### Requirement: Legacy regression coverage
The change MUST include explicit regression coverage against VB behavior for expediente/unidad decision and folio accumulation.

#### Scenario: Legacy regression matrix exists
- **WHEN** reviewers validate technical artifacts
- **THEN** a documented VB vs C# matrix covers expediente-only, unidad-only, mandatory class, expediente status, unidad tipo, and folio field selection
