## Context

- Jira key: `SCRUM-193`
- Summary: `IMPLEMENTACION-COMPENSACION-BD-FALLO-FISICO`
- Current architecture commits DB in `StorageTransactionCoordinator` and runs physical phase in `StoragePhysicalPhaseExecutor`.

## Problem

On post-commit physical failure:
1. DB effects are already committed.
2. Existing compensation removes physical artifacts only.
3. No DB logical compensation exists.
4. DB/FS/XML inconsistency appears.

## Design Goals

- Operational consistency after physical failure.
- Idempotent compensation.
- Auditable traceability.
- Minimal deviation from current implementation.
- No API behavior change.

## Non-Goals

- No `PENDIENTE_FISICO` adoption.
- No full orchestration redesign.
- No `system1` reservation rollback.

## Architecture

### New Service Contract

- `IStorageDbCompensationService`
  - `Task<StorageCompensationDbResult> ExecuteAsync(StorageCompensationDbPlan plan, StorageContext context, Exception originalError)`

### New Models

- `StorageCompensationDbPlan`
  - `IdAlmacen`
  - `IdRegistroProduccionDocumental`
  - `NombreGabinete`
  - `Disco`
  - `Carpeta`
  - `NumeroPaginas`
  - `TieneInventario`
  - `TieneExpediente`
  - `TieneUnidad`
  - `TieneWorkflow`

- `StorageCompensationDbResult`
  - `Estado` (`OK|PARTIAL|FAILED`)
  - `AccionesEjecutadas`
  - `AccionesFallidas`
  - `DuracionMs`
  - `ErrorCompensacion`

### Integration

- `DocumentStorageOrchestrator` in `catch (StoragePhysicalException)`:
  1. Run existing physical compensation.
  2. Run DB compensation.
  3. Preserve original physical exception.

- `StorageTransactionResult` must contain enough data to build compensation plan without ambiguous lookups.

## Deterministic Compensation Order

1. `logdocuarchi` (if applicable)
2. `ra_cert_indice_expediente` (if applicable)
3. expediente/unidad folio reversion (if applicable)
4. inventario annulment (if applicable)
5. dynamic gabinete delete (`ID=idAlmacen`)
6. `disco_detalle` counter reversion

## Idempotency Rules

- Validate existence before each operation.
- Missing target is a no-op, not a failure.
- Re-execution must not duplicate side effects.
- `PARTIAL` preserves evidence of incomplete compensation.

## Data Access Rules

- Default: `DapperCrudEngine`.
- Exception allowed: direct Dapper for critical explicit transaction paths (`FOR UPDATE`, deterministic order, concurrency control).
- Do not mix engines in the same atomic operation.

## Invariants

- Never revert `system1.proxid`, `system1.numcarp`, `system1.NUMPAG_CARP`.
- Never hide original physical error.
- Every compensation execution is auditable.

## Observability

Required fields per compensation event:
- `requestId`
- `idAlmacen`
- `idRegistroProduccionDocumental`
- `gabinete`
- `usuario`
- `fase`
- `duracionMs`
- `resultado`
- `tipoEvento=COMPENSATION`

Target audit table:
- `ra_log_sotorage_compensacion` (when present in target environment).

## Risks and Mitigations

- Partial compensation due to concurrent locks.
  - Mitigation: `PARTIAL` state + detailed logs + retry workflow.
- Schema drift across environments.
  - Mitigation: startup validation and controlled error.
- Hard-delete constraints by business policy.
  - Mitigation: soft-delete for inventario when available.

## Test Strategy

Unit:
- FS failure triggers DB compensation.
- XML failure triggers DB compensation.
- Idempotent re-execution.
- Missing records do not break flow.
- Partial result when one step fails.

Integration:
- DB commit + physical failure simulation.
- Verify table-specific reversion.
- Verify `system1` invariants.

Regression:
- Successful flow unchanged.
- Legacy functional parity preserved.

## Documentation Outputs

Update under `Docs/GestorDocumental/AlmacenamientoDocumental/Arquitectura-Final/`:
- `SCRUM-189-Arquitectura-Compensacion-DB.md`
- `SCRUM-189-Implementacion-Compensacion-DB.md`
- `SCRUM-189-Pruebas-Compensacion-DB.md`
- `SCRUM-189-Observabilidad-Compensacion-DB.md`
- `SCRUM-189-Metadata.md`