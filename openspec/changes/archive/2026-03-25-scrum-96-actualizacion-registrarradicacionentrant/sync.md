# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-96`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-96`
- Summary: `ACTUALIZACION-RegistrarRadicacionEntranteAsync-existenciaWorkflowResult`
- Coordinator change: `openspec/changes/scrum-96-actualizacion-registrarradicacionentrant/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | no | no_code_change | sin cambios funcionales para este ticket | n/a | n/a | n/a | n_a |
| DocuArchiCore | yes | implementation_required | actualiza pruebas, spec, tasks y evidencia tecnica | done | https://github.com/miguelurueta/DocuArchiCore/pull/110 | done | archived |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | no | no_code_change | sin cambios funcionales para este ticket | n/a | n/a | n/a | n_a |
| MiApp.Services | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Services/pull/50 | done | archived |
| MiApp.Repository | no | no_code_change | sin cambios funcionales para este ticket | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | sin cambios funcionales para este ticket | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-96` only in rows with `Impacta? = yes`.
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