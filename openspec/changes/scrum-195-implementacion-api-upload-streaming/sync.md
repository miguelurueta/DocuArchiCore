# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-195`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-195`
- Summary: `IMPLEMENTACION-API-UPLOAD-STREAMING`
- Coordinator change: `openspec/changes/scrum-195-implementacion-api-upload-streaming/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | nuevos endpoints upload-temporal + integración validación previa almacenamiento | pending | n/a | pending | pending |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/259 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | DTOs init/chunk/status/complete/cancel y respuestas estándar upload | pending | n/a | pending | pending |
| MiApp.Services | yes | implementation_required | servicios de streaming/chunk, ensamblaje, hash, política y cleanup | pending | n/a | pending | pending |
| MiApp.Repository | yes | implementation_required | persistencia de sesión upload temporal y estado de chunks | pending | n/a | pending | pending |
| MiApp.Models | yes | implementation_required | modelos de sesión temporal, estados y metadatos upload | pending | n/a | pending | pending |

## Operating Rule

Run `opsxj:new SCRUM-195` only in rows with `Impacta? = yes`.
Only repos marked `implementation_required` should open branch/commit/PR.
Repos marked `traceability_only` stay in `sync.md` without opening empty PRs.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: yes (upload endpoints and integration guard)
- `MiApp.Services`: yes (streaming/chunk/upload orchestration)
- `MiApp.Repository`: yes (session state persistence)
- `MiApp.DTOs`: yes (upload request/response contracts)
- `MiApp.Models`: yes (domain models for upload state)
- `DocuArchiCore`: yes (OpenSpec + docs + orchestrated flow)
