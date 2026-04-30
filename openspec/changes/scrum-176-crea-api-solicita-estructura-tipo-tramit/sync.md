# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-176`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-176`
- Summary: `CREA-API-SOLICITA-ESTRUCTURA-TIPO-TRAMITE`
- Coordinator change: `openspec/changes/scrum-176-crea-api-solicita-estructura-tipo-tramit/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | endpoint y DI para solicitar estructura tipo doc entrante | done | pending | pending | in_review |
| DocuArchiCore | yes | implementation_required | coordinacion openspec, prueba y documentacion tecnica | done | https://github.com/miguelurueta/DocuArchiCore/pull/214 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | reutiliza DTO existente TipoDocEntranteParametroDto sin cambios | done | n/a | pending | tracked |
| MiApp.Services | yes | implementation_required | nuevo service de consulta de estructura tipo doc entrante | done | pending | pending | in_review |
| MiApp.Repository | yes | traceability_only | reutiliza ITipoDocEntranteR existente sin cambios | done | n/a | pending | tracked |
| MiApp.Models | yes | traceability_only | trazabilidad centralizada sin diff funcional | n/a | n/a | pending | tracked |

## Operating Rule

Run `opsxj:new SCRUM-176` only in rows with `Impacta? = yes`.
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
