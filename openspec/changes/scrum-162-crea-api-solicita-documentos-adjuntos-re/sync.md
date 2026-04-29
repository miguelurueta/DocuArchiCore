# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-162`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-162`
- Summary: `CREA-API-SOLICITA-DOCUMENTOS-ADJUNTOS-RESPUESTA-RADICADO`
- Coordinator change: `openspec/changes/scrum-162-crea-api-solicita-documentos-adjuntos-re/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | nuevo controller + DI | done | pending | pending | in_progress |
| DocuArchiCore | yes | implementation_required | orquestador openspec + pruebas + documentación | done | https://github.com/miguelurueta/DocuArchiCore/pull/209 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |
| MiApp.Services | yes | implementation_required | nuevo service + DTO + reglas de deduplicación | done | pending | pending | in_progress |
| MiApp.Repository | yes | implementation_required | nuevo repository SQL + logging | done | pending | pending | in_progress |
| MiApp.Models | yes | traceability_only | trazabilidad centralizada sin diff funcional | pending | n/a | pending | tracked |

## Operating Rule

Run `opsxj:new SCRUM-162` only in rows with `Impacta? = yes`.
Rows marked `implementation_required` require branch/commit/PR and test evidence.
Rows with `Impacta? = no` stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
