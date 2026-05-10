## 1. Baseline and Scope Lock

- [x] 1.1 Confirm final scope of SCRUM-193: DB compensation after physical failure, no `PENDIENTE_FISICO`, no API changes.
- [x] 1.2 Confirm impact matrix in `sync.md` for satellite repos with implementation scope.
- [x] 1.3 Register non-negotiable invariants (`system1` counters not reverted).

## 2. Design Refinement

- [x] 2.1 Define `IStorageDbCompensationService` and models `StorageCompensationDbPlan/Result`.
- [x] 2.2 Define deterministic compensation order and idempotency rules.
- [x] 2.3 Define per-step data access strategy (DapperCrudEngine vs direct Dapper exception).

## 3. Service Implementation

- [x] 3.1 Implement `StorageDbCompensationService` in `MiApp.Services/.../Compensation/`.
- [x] 3.2 Integrate execution in `DocumentStorageOrchestrator` inside `catch (StoragePhysicalException)`.
- [x] 3.3 Preserve original physical exception while recording compensation result (`OK|PARTIAL|FAILED`).
- [x] 3.4 Add defensive validation for `StorageCompensationDbPlan`.

## 4. Repository/Transaction Actions

- [x] 4.1 Revert `disco_detalle.NUMERO_IMAGENES` and `disco_detalle.NUMPAG_CARP` in short transaction.
- [x] 4.2 Delete dynamic gabinete row by `ID=idAlmacen`.
- [x] 4.3 Annul inventario (soft-delete) using existing fields, documenting fallback behavior.
- [x] 4.4 Revert expediente/unidad folios only when affected by original transaction.
- [x] 4.5 Remove workflow log when insertion evidence exists.

## 5. Observability and Audit

- [x] 5.1 Emit structured compensation logs with required correlation fields.
- [x] 5.2 Integrate audit write to `ra_log_sotorage_compensacion` when available.
- [x] 5.3 Define behavior when audit write fails (`PARTIAL`, never hide root error).

## 6. Tests

- [x] 6.1 Unit tests: FS failure, XML failure, idempotency, missing records, partial result.
- [x] 6.2 Integration tests: DB commit + simulated physical failure with table-level assertions.
- [x] 6.3 Regression tests: successful path unchanged and legacy parity preserved.

## 7. Documentation and Closure

- [x] 7.1 Update SCRUM-189 docs for architecture, implementation, tests and observability.
- [x] 7.2 Update `SCRUM-189-Metadata.md` with SCRUM-193 evidence.
- [x] 7.3 Run `openspec validate scrum-193-implementacion-compensacion-bd-fallo-fis`.
- [x] 7.4 Complete review checklist and continue with `opsxj:orchestrate:publish` then `opsxj:orchestrate:archive`.
