## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-169.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-169

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Expediente and unidad transactional locking
The system MUST lock both expediente and unidad de conservacion rows inside the active storage transaction before computing and persisting archival index data.

#### Scenario: Valid expediente and unidad identifiers
- **WHEN** the request includes expediente and unidad identifiers with archival options enabled
- **THEN** the coordinator requests `SELECT ... FOR UPDATE` semantics through repository methods
- **AND** execution continues only when both records exist and are active

#### Scenario: Missing or inactive archival records
- **WHEN** expediente or unidad is missing/inactive
- **THEN** coordinator throws `StorageTransactionException`
- **AND** the storage transaction is rolled back

### Requirement: Archival index calculation
The system MUST calculate `nuevoOrden`, `paginaInicial`, and `paginaFinal` with deterministic arithmetic and validation rules.

#### Scenario: Valid folios and previous index values
- **WHEN** expediente reports `ORDEN_INDICE` and `ULTIMA_PAGINA_INDICE`
- **THEN** calculator returns `nuevoOrden = orden + 1`, `paginaInicial = ultimaPagina + 1`, `paginaFinal = paginaInicial + numeroFolios - 1`

#### Scenario: Invalid index arithmetic
- **WHEN** folios or computed page range is invalid
- **THEN** calculator rejects the operation with transaction validation error

### Requirement: SHA256 index fingerprint
The system MUST build the electronic index insert model using a deterministic SHA256 hash.

#### Scenario: Valid index input model
- **WHEN** builder receives document metadata and calculation result
- **THEN** it generates a SHA256 hash and maps the insert model for `ra_cert_indice_expediente`
- **AND** it does not use MD5 or volatile data

### Requirement: Repository persistence via DapperCrudEngine
Expediente/unidad/index repositories MUST execute read/update/insert operations through `DapperCrudEngine + QueryOptions`.

#### Scenario: Repository lock and update operations
- **WHEN** repository methods execute lock or update behavior
- **THEN** they use `QueryOptions` with explicit columns, filters and lock/update semantics
- **AND** they do not execute direct `IDbConnection.ExecuteAsync/QueryAsync/ExecuteScalarAsync`

#### Scenario: Repository index insert operation
- **WHEN** the coordinator inserts a row in `ra_cert_indice_expediente`
- **THEN** repository uses DapperCrudEngine insert path with typed parameters and transaction context propagation

### Requirement: Coordinator archival phase integration
The transaction coordinator MUST orchestrate archival persistence in a fixed, single-transaction order.

#### Scenario: Full archival phase success
- **WHEN** gabinete + inventario succeed and expediente phase applies
- **THEN** coordinator executes lock expediente -> lock unidad -> calculate index -> update expediente -> update unidad -> insert index -> commit

#### Scenario: Failure in any archival step
- **WHEN** a lock/update/insert step fails
- **THEN** coordinator performs rollback and returns a controlled storage transaction error

### Requirement: Observability for archival phase
The system MUST emit logs and metrics for archival lock/update/index stages without exposing sensitive content.

#### Scenario: Successful archival persistence
- **WHEN** archival phase completes
- **THEN** logs include `requestId`, `idExpediente`, `idUnidad`, `idRegistroProduccion`, `nuevoOrden`, `paginaInicial`, `paginaFinal`, and duration

#### Scenario: Archival failure
- **WHEN** any archival stage fails
- **THEN** logs capture failing phase and error category without leaking full-text document payload

### Requirement: Testability of archival phase
The system MUST provide unit/integration-level evidence for calculator/builder rules and transaction rollback behavior.

#### Scenario: Unit tests for deterministic rules
- **WHEN** calculator and builder are tested
- **THEN** tests validate arithmetic boundaries, hash determinism and invalid input handling

#### Scenario: Transaction coordinator archival tests
- **WHEN** coordinator tests simulate success and failure paths
- **THEN** tests assert single commit on success and rollback on archival failure
