# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-85`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-85`
- Summary: `ACTUALIZACION-ValidarActividadInicioFlujoAsync`
- Coordinator change: `openspec/changes/scrum-85-actualizacion-validaractividadiniciofluj/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| DocuArchiCore | yes | Repo coordinador del cambio, artifacts OpenSpec, tests y documentacion tecnica del ticket. | done | https://github.com/miguelurueta/DocuArchiCore/pull/79 | pending | in-review |
| MiApp.Services | yes | Cambio funcional real en `RegistrarRadicacionEntranteService`. Tooling `opsxj` alineado para soportar `-NonInteractive` y plantilla `sync.md` corregida en la misma rama del ticket. | done | https://github.com/miguelurueta/MiApp.Services/pull/38 | pending | in-review |
| DocuArchi.Api | no | No hubo delta de codigo para `SCRUM-85`; el wiring DI ya existia desde `SCRUM-84`. | n/a | n/a | n/a | out-of-scope |
| MiApp.Repository | no | No se requirio cambio de consulta ni contrato de repositorio en este ticket. | n/a | n/a | n/a | out-of-scope |
| MiApp.DTOs | no | No se requirio ajuste de DTOs publicos para el delta implementado. | n/a | n/a | n/a | out-of-scope |
| MiApp.Models | no | No se requirio cambio de modelos para el delta implementado. | n/a | n/a | n/a | out-of-scope |
| DocuArchiCore.Web | no | Sin impacto UI ni contrato frontend en este ticket. | n/a | n/a | n/a | out-of-scope |
| Observacion | yes | Seguimiento resuelto el 23/03/2026: se sincronizo `Tools/jira-open`, se habilito `-NonInteractive`, se corrigio la generacion de `sync.md` y la rama activa de `MiApp.Services` quedo publicada en el PR actual. El PR previo `#37` ya fue mergeado. | done | https://github.com/miguelurueta/MiApp.Services/pull/38 | pending | resolved |

## Operating Rule

Run `opsxj:new SCRUM-85` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-85` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
