# jira-scrum-166 Specification

## Purpose
TBD - created by archiving change scrum-166-crea-funcion-asignador-identidad-almacen. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-166.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-166

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Legacy identity reservation on system1
The system MUST reserve storage identity from `system1` using row-level lock semantics and legacy-compatible rules for `proxid`, `tamdisc`, `numcarp` and `NUMPAG_CARP`.

#### Scenario: Reserve identity with strict legacy proxid rule
- **WHEN** allocator loads a locked `system1` row for gabinete and receives a valid page count
- **THEN** it calculates `IdAlmacen = ProxId + 1` and sets `NewProxId = IdAlmacen`

#### Scenario: Reject unsupported legacy tamdisc values
- **WHEN** `tamdisc` is different from `572523149` and `4310948432`
- **THEN** allocator throws `StorageTransactionException`

#### Scenario: Rotate folder when 230 pages threshold is exceeded
- **WHEN** `current.NumPagCarp + numeroPaginasDocumento > 230`
- **THEN** reservation increments folder and resets folder pages to current document page count

### Requirement: Locked disk quota validation on disco_detalle
The system MUST lock and validate `disco_detalle` before confirming reservation in `system1`.

#### Scenario: Disk marked as over limit
- **WHEN** `EstadoDisco = "SL"` in locked `disco_detalle` row
- **THEN** allocator throws `StorageTransactionException` and aborts reservation update

#### Scenario: Disk row unavailable
- **WHEN** disk status row is not found for gabinete/disco combination
- **THEN** disk quota policy throws `StorageTransactionException`

### Requirement: Repository access through DapperCrudEngine only
Repository operations for `system1` and `disco_detalle` MUST be executed via `DapperCrudEngine` using `QueryOptions`, including `LockMode` and `UpdateValues` support.

#### Scenario: Lock operations use QueryOptions lock mode
- **WHEN** `SystemStorageRepository.LockByGabineteAsync` or `StorageDiskQuotaRepository.LockDiskStatusAsync` executes
- **THEN** it issues the query through `DapperCrudEngine` with `LockMode = ForUpdate`

#### Scenario: Reservation update uses optimistic filter
- **WHEN** `SystemStorageRepository.UpdateReservationAsync` updates `system1`
- **THEN** it applies filters for gabinete and previous `proxid`, and fails if affected rows are not exactly one

### Requirement: Service/repository contracts are registered in DI
The API bootstrap MUST register identity allocator, policies and repositories required for SCRUM-166.

#### Scenario: Runtime resolves all SCRUM-166 contracts
- **WHEN** application starts
- **THEN** DI container resolves `IStorageIdentityAllocator`, `IStorageIdentityPolicy`, `IStorageDiskQuotaPolicy`, `ISystemStorageRepository`, and `IStorageDiskQuotaRepository`

### Requirement: SCRUM-166 documentation package
The change MUST include architecture, implementation, test, observability, legacy-coverage, concurrency and metadata documents under StorageEngine docs path.

#### Scenario: Reviewer audits implementation artifacts
- **WHEN** reviewer opens `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine`
- **THEN** the SCRUM-166 markdown set documents locking strategy, legacy rules, repository usage and test evidence

