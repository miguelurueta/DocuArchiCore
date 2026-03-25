# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-100`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-100`
- Summary: `IMPORTAR-Actualiza_estado_registro_modulo_radicacion`
- Coordinator change: `openspec/changes/scrum-100-importar-actualiza-estado-registro-modul/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | traceability_only | sin diff real publicado | n/a | n/a | done | archived |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/118 | done | archived |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | sin diff real publicado | n/a | n/a | done | archived |
| MiApp.Services | yes | traceability_only | sin diff real publicado | n/a | n/a | done | archived |
| MiApp.Repository | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Repository/pull/24 | done | archived |
| MiApp.Models | yes | traceability_only | sin diff real publicado | n/a | n/a | done | archived |

## Operating Rule

Run `opsxj:new SCRUM-100` only in rows with `Impacta? = yes`.
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


## Migration Read-Only Repositories (MIGRACION-NET Actualiza_estado_registro_modulo_radicacion)

- `solo consulta`: `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net`