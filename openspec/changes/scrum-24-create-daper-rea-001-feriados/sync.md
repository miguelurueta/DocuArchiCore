# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-24`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-24`
- Summary: `CREATE-DAPER-rea_001_feriados`
- Coordinator change: `openspec/changes/scrum-24-create-daper-rea-001-feriados/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `DocuArchi.Api` | `no` | `Se usa como referencia de estructura y conexion existente` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore` | `yes` | `Coordinacion OpenSpec del ticket SCRUM-24` | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/3` | `pending` | `review` |
| `DocuArchiCore.Abstractions` | `no` | `Sin cambios de contratos en este alcance` | `n/a` | `n/a` | `n/a` | `n_a` |
| `DocuArchiCore.Web` | `no` | `Sin alcance frontend` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.DTOs` | `no` | `No se solicitan DTOs nuevos en este ticket` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Services` | `no` | `No hay logica de servicio nueva definida` | `n/a` | `n/a` | `n/a` | `n_a` |
| `MiApp.Repository` | `yes` | `Pruebas de mapeo Dapper y soporte de acceso a datos` | `pending` | `pending` | `pending` | `blocked` |
| `MiApp.Models` | `yes` | `Crear modelo rea_001_feriados en ruta Radicacion/Feriados` | `pending` | `pending` | `pending` | `blocked` |

## Operating Rule

Run `opsxj:new SCRUM-24` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-24` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Blockers

- Waiting for confirmed schema of `docuarchi.rea_001_feriados`.
- Use `table-schema-request.md` and `db-introspection.sql` to collect required DDL/columns.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
