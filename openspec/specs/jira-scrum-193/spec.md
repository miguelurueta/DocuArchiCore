# jira-scrum-193 Specification

## Purpose
TBD - created by archiving change scrum-193-implementacion-compensacion-bd-fallo-fis. Update Purpose after archive.
## Requirements
### Requirement: Trigger DB compensation on physical failure after DB commit
The system MUST execute logical DB compensation when physical phase fails after a successful transactional commit.

#### Scenario: File copy fails post-commit
- **GIVEN** transaction phase committed successfully
- **AND** physical phase starts
- **WHEN** file copy fails with `StoragePhysicalException`
- **THEN** physical compensation MUST run
- **AND** DB compensation MUST run
- **AND** original physical error MUST be preserved in the final failure path

#### Scenario: XML generation fails post-commit
- **GIVEN** transaction phase committed successfully
- **WHEN** XML write fails in physical phase
- **THEN** DB compensation MUST execute using transaction result data

### Requirement: Deterministic and idempotent compensation sequence
DB compensation MUST execute in deterministic order and be safe for re-execution.

#### Scenario: Compensation executes with missing records
- **WHEN** a compensation step targets a record that no longer exists
- **THEN** step MUST be treated as no-op
- **AND** process continues to next step

#### Scenario: Compensation executes twice
- **GIVEN** same `requestId` and `idAlmacen`
- **WHEN** compensation is executed multiple times
- **THEN** no duplicated side effects are produced

### Requirement: Protect system1 identity counters
Compensation MUST NOT revert `system1.proxid`, `system1.numcarp`, or `system1.NUMPAG_CARP`.

#### Scenario: Physical failure with successful DB commit
- **WHEN** compensation executes
- **THEN** only allowed tables/fields are reverted
- **AND** `system1` reservation counters remain unchanged

### Requirement: Revert allowed operational effects
Compensation MUST revert only approved post-commit effects.

#### Scenario: Full compensation scope
- **WHEN** compensation plan indicates affected entities
- **THEN** system reverts `disco_detalle` counters
- **AND** deletes gabinete row by `ID=idAlmacen`
- **AND** annuls inventario record according to schema
- **AND** reverts expediente/unidad folios when applicable
- **AND** removes workflow log when applicable

### Requirement: Compensation observability and auditability
Every compensation execution MUST produce structured traceability data.

#### Scenario: Compensation result is PARTIAL
- **WHEN** at least one compensation action fails but others succeed
- **THEN** result MUST be `PARTIAL`
- **AND** failed actions MUST be logged with reason
- **AND** event MUST include `requestId`, `idAlmacen`, phase, duration and outcome

### Requirement: No API contract changes
Implementation MUST keep existing API contracts and response envelope behavior.

#### Scenario: Physical failure path
- **WHEN** compensation executes after physical failure
- **THEN** API contract remains unchanged
- **AND** failure response continues using existing error model

