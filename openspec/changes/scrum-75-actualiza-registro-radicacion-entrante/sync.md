# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-75`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-75`
- Summary: `ACTUALIZA-REGISTRO-RADICACION-ENTRANTE`
- Coordinator change: `openspec/changes/scrum-75-actualiza-registro-radicacion-entrante/`

## Impact Matrix

| Repo | Impacta? | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|
| `MiApp.DTOs` | `yes` | Se agrega `tipoModuloRadicacion` al request de registrar entrante. | `pending` | `pending` | `n/a` | `implementando` |
| `MiApp.Repository` | `yes` | Se ajusta el repository y la policy para usar `tipoModuloRadicacion` en el registro atomico. | `pending` | `pending` | `n/a` | `implementando` |
| `MiApp.Services` | `yes` | El service de registro debe propagar el nuevo parametro y decidir la prevalidacion workflow con ese valor. | `pending` | `pending` | `n/a` | `implementando` |
| `DocuArchi.Api` | `yes` | El controller `registrar-entrante` expone `tipoModuloRadicacion=1` por query string. | `pending` | `pending` | `n/a` | `implementando` |
| `MiApp.Models` | `no` | No se requiere nuevo modelo; el cambio usa contratos y entidades existentes. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore` | `yes` | Coordinador: tests, docs, spec, tasks y validacion OpenSpec. | `done` | `https://github.com/miguelurueta/DocuArchiCore/pull/66` | `pending` | `implementando` |
| `DocuArchiCore.Web` | `no` | No hay frontend nuevo; solo actualizacion de contrato documental. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |
| `DocuArchiCore.Abstractions` | `no` | No se requiere contrato compartido adicional. | `n/a` | `n/a` | `n/a` | `fuera de alcance` |

## Operating Rule

Run `opsxj:new SCRUM-75` only in rows with `Impacta? = yes`.
Run `opsxj:archive SCRUM-75` only after the repo PR is merged.
Rows with `Impacta? = no` must stay as `n/a` to make scope explicit.

## Typical API Change Pattern

- `DocuArchi.Api`: usually yes
- `MiApp.Services`: usually yes
- `MiApp.Repository`: usually yes
- `MiApp.DTOs`: usually yes
- `MiApp.Models`: optional (only if model changes)
- `DocuArchiCore` / `DocuArchiCore.Web`: only if coordinator or UI is in scope
