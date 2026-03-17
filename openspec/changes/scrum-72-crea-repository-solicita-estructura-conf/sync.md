# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-72`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-72`
- Summary: `CREA-REPOSITORY-SOLICITA-ESTRUCTURA-CONFIGURACIO-LISTADO-RUTA`
- Coordinator change: `openspec/changes/scrum-72-crea-repository-solicita-estructura-conf/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `MiApp.Models` | `yes` | Se actualiza el modelo `ConfiguracionListadoRuta` para mapear explicitamente `configuracion_listado_ruta`. | `pending` | `pending` | `n/a` | `implementado_local` |
| `MiApp.Repository` | `yes` | Se crea el repository `SolicitaEstructuraConfiguracionListadoRutaRepository` con consulta parametrizada por ruta. | `pending` | `pending` | `n/a` | `implementado_local` |
| `DocuArchi.Api` | `yes` | Solo registra el repository en DI para mantener el wiring del backend consistente. | `pending` | `pending` | `n/a` | `implementado_local` |
| `MiApp.Services` | `no` | El diseño final no requiere servicio intermedio; el alcance es repository-only. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `MiApp.DTOs` | `no` | No se agregan DTOs; el repository devuelve entidades `ConfiguracionListadoRuta`. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore` | `yes` | Coordinador: pruebas del repository, docs, spec, tasks y validacion OpenSpec. | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/60` | `pending` | `ready_for_git_flow` |
| `DocuArchiCore.Web` | `no` | No se expone endpoint ni cambio frontend para esta consulta interna. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore.Abstractions` | `no` | No se requiere contrato compartido adicional. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |

## Operating Rule

Run `opsxj:new SCRUM-72` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-72` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
