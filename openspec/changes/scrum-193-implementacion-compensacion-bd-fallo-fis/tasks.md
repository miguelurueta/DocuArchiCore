## 1. Baseline and Scope Lock

- [ ] 1.1 Confirm final scope of SCRUM-193: DB compensation after physical failure, no `PENDIENTE_FISICO`, no API changes.
- [ ] 1.2 Confirm impact matrix in `sync.md` and keep satellite repos as `traceability_only`.
- [ ] 1.3 Register non-negotiable invariants (`system1` counters not reverted).

## 2. Design Refinement

- [ ] 2.1 Define `IStorageDbCompensationService` and models `StorageCompensationDbPlan/Result`.
- [ ] 2.2 Define deterministic compensation order and idempotency rules.
- [ ] 2.3 Define per-step data access strategy (DapperCrudEngine vs direct Dapper exception).

## 3. Service Implementation

- [ ] 3.1 Implement `StorageDbCompensationService` in `MiApp.Services/.../Compensation/`.
- [ ] 3.2 Integrate execution in `DocumentStorageOrchestrator` inside `catch (StoragePhysicalException)`.
- [ ] 3.3 Preserve original physical exception while recording compensation result (`OK|PARTIAL|FAILED`).
- [ ] 3.4 Add defensive validation for `StorageCompensationDbPlan`.

## 4. Repository/Transaction Actions

- [ ] 4.1 Revert `disco_detalle.NUMERO_IMAGENES` and `disco_detalle.NUMPAG_CARP` in short transaction.
- [ ] 4.2 Delete dynamic gabinete row by `ID=idAlmacen`.
- [ ] 4.3 Annul inventario (soft-delete) using existing fields, documenting fallback behavior.
- [ ] 4.4 Revert expediente/unidad folios only when affected by original transaction.
- [ ] 4.5 Remove workflow log when insertion evidence exists.

## 5. Observability and Audit

- [ ] 5.1 Emit structured compensation logs with required correlation fields.
- [ ] 5.2 Integrate audit write to `ra_log_sotorage_compensacion` when available.
- [ ] 5.3 Define behavior when audit write fails (`PARTIAL`, never hide root error).

## 6. Tests

- [ ] 6.1 Unit tests: FS failure, XML failure, idempotency, missing records, partial result.
- [ ] 6.2 Integration tests: DB commit + simulated physical failure with table-level assertions.
- [ ] 6.3 Regression tests: successful path unchanged and legacy parity preserved.

## 7. Documentation and Closure

- [ ] 7.1 Update SCRUM-189 docs for architecture, implementation, tests and observability.
- [ ] 7.2 Update `SCRUM-189-Metadata.md` with SCRUM-193 evidence.
- [ ] 7.3 Run `openspec validate scrum-193-implementacion-compensacion-bd-fallo-fis`.
- [ ] 7.4 Complete review checklist and continue with `opsxj:orchestrate:publish` then `opsxj:orchestrate:archive`.