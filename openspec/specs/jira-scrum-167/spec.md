# jira-scrum-167 Specification

## Purpose
TBD - created by archiving change scrum-167-crea-funcion-coordinador-transacciones-a. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-167.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-167

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Unified DB transaction coordinator
The storage orchestration layer MUST execute identity reservation and disk quota update inside one serializable transaction.

#### Scenario: Successful transactional reservation
- **WHEN** validation pipeline succeeds and transaction coordinator executes with valid context
- **THEN** it opens one connection, starts one `Serializable` transaction, reserves identity once, updates `disco_detalle`, commits, and returns `StorageTransactionResult.Success=true`

#### Scenario: Any transactional failure triggers rollback
- **WHEN** identity, disk lock, disk update, or workflow extension fails
- **THEN** coordinator performs rollback and throws `StorageTransactionException`

### Requirement: Disk quota update after identity reservation
The coordinator MUST update `disco_detalle` after identity reservation using repository abstractions and transaction context.

#### Scenario: Disk update rowcount mismatch
- **WHEN** `IStorageDiskQuotaRepository.UpdateDiskUsageAsync` does not affect exactly one row
- **THEN** coordinator throws `StorageTransactionException` and rolls back

#### Scenario: Disk update computes legacy image accumulation
- **WHEN** coordinator calculates the final quota payload
- **THEN** `NuevoNumeroImagenes` equals current `NumeroImagenes` plus document page count, and `NuevoNumPagCarp` equals reservation folder pages

### Requirement: Workflow log extension point
The coordinator MUST expose an extension point for workflow logging without embedding SQL logic in the service.

#### Scenario: Workflow task id not present or non-positive
- **WHEN** workflow data is null, missing task id, or task id is zero/negative
- **THEN** coordinator skips workflow insert and continues transaction

#### Scenario: Workflow task id positive with repository available
- **WHEN** workflow task id is greater than zero and a workflow log repository is injected
- **THEN** coordinator invokes workflow log insert in the same transaction and reports `WorkflowLogInserted=true`

### Requirement: Orchestrator integration with transaction coordinator
`DocumentStorageOrchestrator` MUST delegate DB phase to `IStorageTransactionCoordinator` after successful validation.

#### Scenario: Orchestrator returns reserved identity values
- **WHEN** validation and transaction coordinator succeed
- **THEN** orchestrator response includes reserved `IdAlmacen`, transaction state, and request traceability fields

### Requirement: SCRUM-167 documentation package
The change MUST include architecture, implementation, tests, observability, legacy coverage and metadata documents under StorageEngine docs.

#### Scenario: Reviewer audits SCRUM-167 documentation
- **WHEN** reviewer opens StorageEngine documentation
- **THEN** the SCRUM-167 markdown set explains transaction order, rollback semantics, disk quota update, workflow extension, and test evidence

