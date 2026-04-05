# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-128`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-128`
- Summary: `IMPLEMENTACION-EXTENDER-XLS-PDF`
- Coordinator change: `openspec/changes/scrum-128-implementacion-extender-xls-pdf/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | ajustar endpoint /api/AppTable/export para despachar csv/xlsx/pdf y mantener contrato FileResult | done | https://github.com/miguelurueta/DocuArchi.Api/pull/46 | done | archived |
| DocuArchiCore | yes | implementation_required | orquestador openspec, trazabilidad, tests de contrato y evidencia cross-repo | done | https://github.com/miguelurueta/DocuArchiCore/pull/167 | done | archived |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | extender contrato de exportacion y metadata reusable para branding/archivo si aplica | done | https://github.com/miguelurueta/MiApp.DTOs/pull/35 | done | archived |
| MiApp.Services | yes | implementation_required | extender servicio de exportacion, builders csv/xlsx/pdf y reutilizar query state actual | done | https://github.com/miguelurueta/MiApp.Services/pull/74 | done | archived |
| MiApp.Repository | no | no_code_change | la consulta exportable ya se reutiliza desde el pipeline actual sin nuevo repositorio dedicado | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | no se requiere modelo persistente nuevo para generar archivos exportables | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-128` only in rows with `Impacta? = yes`.
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