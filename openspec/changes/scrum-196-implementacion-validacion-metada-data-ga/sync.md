# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-196`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-196`
- Summary: `IMPLEMENTACION-VALIDACION-METADA-DATA-GABINETE`
- Coordinator change: `openspec/changes/scrum-196-implementacion-validacion-metada-data-ga/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | registro DI, contrato API y validaciones de almacenamiento | done | n/a | pending | in_progress |
| DocuArchiCore | yes | implementation_required | coordinacion OpenSpec, docs y trazabilidad de flujo | done | https://github.com/miguelurueta/DocuArchiCore/pull/260 | pending | in_progress |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | ajuste de contrato de errores si aplica | done | n/a | pending | no_changes_required |
| MiApp.Services | yes | implementation_required | provider real metadata, parser y validadores | done | n/a | pending | in_progress |
| MiApp.Repository | yes | implementation_required | consultas SQL metadata legacy + esquema fisico opcional | done | n/a | pending | in_progress |
| MiApp.Models | yes | implementation_required | ampliacion modelo metadata de campo | done | n/a | pending | in_progress |

## Operating Rule

Run `opsxj:new SCRUM-196` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `traceability_only` stay in `sync.md` without opening empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
