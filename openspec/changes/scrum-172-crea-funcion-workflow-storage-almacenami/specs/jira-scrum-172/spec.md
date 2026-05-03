## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-172.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-172

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Conditional workflow log insertion
The system MUST insert a workflow log record only when the request includes a valid workflow task identifier greater than zero.

#### Scenario: Workflow absent or invalid
- **WHEN** `context.Command.Workflow` is null or `IdTareaWorkflow <= 0`
- **THEN** the coordinator skips workflow log persistence
- **AND** the transaction continues without failing by this condition

#### Scenario: Workflow valid
- **WHEN** `IdTareaWorkflow > 0`
- **THEN** the coordinator builds a `WorkflowStorageLogModel`
- **AND** executes the repository insert inside the active transaction

### Requirement: Legacy-compatible field mapping for `logdocuarchi`
The workflow log model MUST preserve legacy-equivalent fields required by `logdocuarchi`.

#### Scenario: Building workflow log payload
- **WHEN** builder receives `StorageContext` and `StorageTransactionResult`
- **THEN** it maps at least `IdAlmacen`, `UsuarioOperacion`, `FechaTransaccion`, `RutaDocumento`, `NombreGabinete`, `Campos`, `Radicado`, `IdTareaWorkflow`, `IdRutaWorkflow`, `UsuarioPropietario`, and `TipologiaDocumental`
- **AND** route data remains logical/relative (non-sensitive physical paths)

### Requirement: Repository persistence via DapperCrudEngine
Workflow log repository operations MUST use `DapperCrudEngine + QueryOptions` and typed parameters.

#### Scenario: Inserting into `logdocuarchi`
- **WHEN** repository executes insert
- **THEN** it uses `QueryOptions` with explicit fields and transaction propagation
- **AND** it does not use direct `IDbConnection.ExecuteAsync/QueryAsync/ExecuteScalarAsync`

#### Scenario: Invalid repository input
- **WHEN** model/connection/transaction or required values are invalid
- **THEN** repository throws a controlled storage transaction error

### Requirement: Coordinator transactional integrity
Workflow log persistence MUST belong to the same storage transaction managed by `StorageTransactionCoordinator`.

#### Scenario: Workflow insert success
- **WHEN** workflow insert returns one affected row
- **THEN** coordinator can commit the global storage transaction if all previous phases succeeded

#### Scenario: Workflow insert failure
- **WHEN** workflow insert fails or returns unexpected rows
- **THEN** coordinator rolls back the global storage transaction
- **AND** returns a controlled transaction failure

### Requirement: Observability of workflow phase
The system MUST expose observability events for workflow-log phase without leaking sensitive data.

#### Scenario: Successful workflow phase
- **WHEN** workflow log is built and inserted
- **THEN** logs include `requestId`, `idAlmacen`, `idTareaWorkflow`, `idRutaWorkflow`, `nombreGabinete`, and phase status

#### Scenario: Workflow phase failure
- **WHEN** workflow insertion fails
- **THEN** logs capture failing phase and error category
- **AND** avoid full raw index/fulltext payload

### Requirement: Testability of workflow phase
The system MUST provide test evidence for builder/repository validation rules and coordinator commit/rollback behavior.

#### Scenario: Unit tests for builder and repository
- **WHEN** unit suite runs
- **THEN** it validates mapping rules, conditional behavior, and repository guard clauses

#### Scenario: Coordinator workflow branch tests
- **WHEN** tests simulate valid/invalid workflow task ids and repository failures
- **THEN** tests assert expected skip/insert behavior and rollback semantics
