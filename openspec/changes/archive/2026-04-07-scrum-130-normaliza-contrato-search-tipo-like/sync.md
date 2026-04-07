# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-130`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-130`
- Summary: `NORMALIZA-CONTRATO-SEARCH-TIPO-LIKE`
- Coordinator change: `openspec/changes/scrum-130-normaliza-contrato-search-tipo-like/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | sin diff funcional en este ticket | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/170 | done | archived |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | sin diff funcional en este ticket | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Services/pull/77 | done | archived |
| MiApp.Repository | no | no_code_change | sin diff funcional en este ticket | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | sin diff funcional en este ticket | n/a | n/a | n/a | n_a |

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