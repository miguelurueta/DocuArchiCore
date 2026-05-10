## Why

The current flow can leave operational inconsistency when DB transaction is already committed and the physical phase fails (file copy or XML generation). In that case only FileSystem/XML compensation runs, so active DB records may remain without physical representation.

This change implements post-commit DB logical compensation to preserve consistency without introducing `PENDIENTE_FISICO` and without redesigning the full flow.

## What Changes

- Add DB compensation service for `StoragePhysicalException` after transaction commit.
- Execute compensation in deterministic, idempotent order using `StorageTransactionResult` data.
- Keep existing physical compensation and chain both compensations in a controlled failure path.
- Add traceability for compensation outcomes (`OK|PARTIAL|FAILED`) without hiding original physical error.
- Update technical documentation and test evidence.

## Scope

Included:
- Revert `disco_detalle` (`NUMERO_IMAGENES`, `NUMPAG_CARP`).
- Delete dynamic gabinete row by `ID = idAlmacen`.
- Annul inventario with existing status fields (soft-delete preferred).
- Revert expediente/unidad folios when applicable.
- Delete `logdocuarchi` when inserted.

Excluded:
- Reverting `system1.proxid`.
- Reverting `system1.numcarp`.
- Reverting `system1.NUMPAG_CARP`.
- API contract changes.
- Introducing `PENDIENTE_FISICO`.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-193
- OpenSpec change: `openspec/changes/scrum-193-implementacion-compensacion-bd-fallo-fis/`
- Implementation repo: `DocuArchiCore`
- Satellite repos: `traceability_only` in `sync.md`