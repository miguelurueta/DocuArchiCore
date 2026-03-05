# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-42`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-42`
- Summary: `CREA-API-RADICACION`
- Coordinator change: `openspec/changes/scrum-42-crea-api-radicacion/`

## Impact Matrix

| Repo | Impacta? | Motivo | opsxj:new | PR | opsxj:archive | Estado |
|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | Definir alcance de controller/endpoint radicacion | pending | pending | pending | todo |
| DocuArchiCore | yes | Repo coordinador de artefactos OpenSpec y documentacion | done | n/a | pending | in-progress |
| DocuArchiCore.Abstractions | no | Fuera de alcance segun ticket actual | n/a | n/a | n/a | n/a |
| DocuArchiCore.Web | yes | Definir alcance frontend de consumo API | pending | pending | pending | todo |
| MiApp.DTOs | yes | Definir DTOs request/response de radicacion | pending | pending | pending | todo |
| MiApp.Services | yes | Definir servicios de validacion/registro/flujo | pending | pending | pending | todo |
| MiApp.Repository | yes | Definir repositorios Q01-Q09 y transaccion | pending | pending | pending | todo |
| MiApp.Models | yes | Definir cambios de modelos persistidos | pending | pending | pending | todo |

## Operating Rule

Run `opsxj:new SCRUM-42` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-42` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
