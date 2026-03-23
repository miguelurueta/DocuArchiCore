# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-86`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-86`
- Summary: `ACTUALIZACION-REPOSITORIO-RegistrarRadicacionEntranteAsync`
- Coordinator change: `openspec/changes/scrum-86-actualizacion-repositorio-registrarradic/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| DocuArchiCore | yes | Repo coordinador para OpenSpec, pruebas del servicio y trazabilidad multi-repo del ticket. | done | https://github.com/miguelurueta/DocuArchiCore/pull/80 | pending | in-review |
| MiApp.Repository | yes | Ajuste funcional del retorno de `RegistrarRadicacionEntranteAsync` para incluir el identificador persistido del radicado. | done | https://github.com/miguelurueta/MiApp.Repository/pull/20 | pending | in-review |
| MiApp.Services | yes | El servicio debe almacenar y normalizar el nuevo `ReturnRegistraRadicacionDto` dentro del flujo de `RegistrarRadicacionEntranteAsync`. | done | https://github.com/miguelurueta/MiApp.Services/pull/39 | pending | in-review |
| MiApp.DTOs | yes | Se agrega el DTO `ReturnRegistraRadicacionDto` y se amplía el contrato de respuesta compartido. | done | https://github.com/miguelurueta/MiApp.DTOs/pull/17 | pending | in-review |
| DocuArchi.Api | no | No se identificó cambio de controller ni de contrato HTTP en este ticket. | n/a | n/a | n/a | out-of-scope |
| MiApp.Models | no | El ticket no requiere nuevos modelos de dominio ni cambios de mapeo de base de datos. | n/a | n/a | n/a | out-of-scope |
| DocuArchiCore.Abstractions | no | Sin cambios de contratos cross-repo para este delta puntual. | n/a | n/a | n/a | out-of-scope |
| DocuArchiCore.Web | no | No hay cambio UI ni de consumo frontend requerido para el alcance actual. | n/a | n/a | n/a | out-of-scope |

## Operating Rule

Run `opsxj:new SCRUM-86` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-86` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
