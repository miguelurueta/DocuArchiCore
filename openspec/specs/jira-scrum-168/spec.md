# jira-scrum-168 Specification

## Purpose
TBD - created by archiving change scrum-168-crea-funcion-insertcion-gabinete-almacen. Update Purpose after archive.
## Requirements
### Requirement: Gabinete dynamic insert
The system MUST insert storage records into the dynamic gabinete table within the active storage transaction.

#### Scenario: Valid gabinete payload
- **WHEN** `StorageTransactionCoordinator` has a valid identity reservation and gabinete insert model
- **THEN** `IGabineteStorageRepository.InsertAsync` inserts exactly one row into the validated gabinete table
- **AND** table/column identifiers are validated before execution

#### Scenario: Invalid dynamic identifier
- **WHEN** the gabinete table name or a dynamic column name fails identifier validation
- **THEN** the repository rejects the operation with a transaction validation error
- **AND** no insert is performed

### Requirement: Inventario documental insert
The system MUST insert the documental inventory record into `registro_producion_documental` inside the same transaction.

#### Scenario: Valid inventario payload
- **WHEN** gabinete insert succeeded and inventario model is valid
- **THEN** `IInventarioDocumentalRepository.InsertAsync` inserts inventory data and returns generated inventory id

#### Scenario: Inventory insert fails
- **WHEN** inventory insert throws or returns invalid generated id
- **THEN** coordinator triggers rollback for the full transaction

### Requirement: Coordinator transactional integration
The transaction coordinator MUST execute identity reservation, gabinete insert, and inventory insert in a single commit/rollback boundary.

#### Scenario: End-to-end success
- **WHEN** reserve + gabinete insert + inventario insert succeed
- **THEN** coordinator commits once and returns `IdRegistroProduccionDocumental`

#### Scenario: Gabinete or inventario failure
- **WHEN** any persistence phase fails after transaction start
- **THEN** coordinator rolls back all persisted changes

### Requirement: DapperCrudEngine repository rule
Repository-level persistence MUST use DapperCrudEngine abstractions and not direct Dapper execute/query calls.

#### Scenario: Repository persistence implementation
- **WHEN** reviewers inspect repository implementation for SCRUM-168
- **THEN** they find DapperCrudEngine/QueryOptions usage for inserts and transactional context propagation
- **AND** no direct `ExecuteAsync`/`QueryAsync`/`ExecuteScalarAsync` calls in target repositories

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-168.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-168

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Orchestrated impact matrix consistency
The orchestrated change MUST keep `sync.md` aligned with impacted repositories and impact type classification.

#### Scenario: Traceability-only repository is listed
- **WHEN** a repository is classified as `traceability_only`
- **THEN** `sync.md` keeps the repository with `Impacta? = yes`, `Tipo impacto = traceability_only`, and no satellite PR URL

#### Scenario: Coordinator repository requires implementation
- **WHEN** the coordinator repo (`DocuArchiCore`) manages OpenSpec artifacts for SCRUM-168
- **THEN** `sync.md` records `implementation_required` and links the coordinator PR

### Requirement: Publish gate enforcement
The workflow MUST block `opsxj:orchestrate:publish` until OpenSpec tasks are complete and review readiness is explicit.

#### Scenario: Pending tasks exist
- **WHEN** `tasks.md` still has at least one unchecked item
- **THEN** `opsxj:orchestrate:publish` fails with an incomplete tasks policy error

#### Scenario: Tasks and review gate are complete
- **WHEN** `tasks.md` has no unchecked items and review confirmation is present
- **THEN** `opsxj:orchestrate:publish` can continue evaluating satellite repository diffs

