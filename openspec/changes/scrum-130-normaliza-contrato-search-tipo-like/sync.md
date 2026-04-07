# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-130`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-130`
- Summary: `NORMALIZA-CONTRATO-SEARCH-TIPO-LIKE`
- Coordinator change: `openspec/changes/scrum-130-normaliza-contrato-search-tipo-like/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/170 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |
| MiApp.Services | yes | implementation_required | normalizacion SearchType y LIKE global seguro en Workflow Inbox | done | pending | pending | in_progress |
| MiApp.Repository | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |
| MiApp.Models | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |

## Operating Rule

Run `opsxj:new SCRUM-130` only in rows with `Impacta? = yes`.
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
