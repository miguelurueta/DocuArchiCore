# Repo Impact Plan Template

Use this template at the start of each Jira ticket (`SCRUM-126`) to decide exactly where to run `opsxj:new` and `opsxj:archive`.

## Ticket

- Jira key: `SCRUM-126`
- Summary: `[APPTABLE_EXPORT_19] Definir estrategia backend de exportacion total allMatching`
- Coordinator change: `openspec/changes/scrum-126-apptable-export-19-definir-estrategia-ba/`

## Impact Matrix

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/DocuArchi.Api/pull/45 | pending | in_review |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/166 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.DTOs/pull/34 | pending | in_review |
| MiApp.Services | yes | implementation_required | implementacion publicada desde diff real | done | https://github.com/miguelurueta/MiApp.Services/pull/73 | pending | in_review |
| MiApp.Repository | no | no_code_change | la implementacion actual relevante de repository/query builder vive en `MiApp.Services` | n/a | n/a | n/a | n_a |
| MiApp.Models | no | no_code_change | el ticket reutiliza `dat_adic_tar{ruta}` y metadata existente; no requiere modelo nuevo | n/a | n/a | n/a | n_a |

## Operating Rule

Run `opsxj:new SCRUM-126` only in rows with `Impacta? = yes`.
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

## Verification Evidence

- OpenSpec validation: `openspec.cmd validate scrum-126-apptable-export-19-definir-estrategia-ba` -> valid
- Targeted test run: `dotnet test tests\TramiteDiasVencimiento.Tests\TramiteDiasVencimiento.Tests.csproj --filter "WorkflowInbox"` -> 52 passed, 0 failed
- Implemented source changes:
  - `DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs`
  - `MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`
  - `MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs`
  - `MiApp.DTOs/DTOs/Workflow/BandejaCorrespondencia/WorkflowInboxExportRequestDto.cs`
  - `MiApp.DTOs/DTOs/Workflow/BandejaCorrespondencia/WorkflowInboxExportFileDto.cs`
- Added/updated tests:
  - `tests/TramiteDiasVencimiento.Tests/WorkflowInboxControllerTests.cs`
  - `tests/TramiteDiasVencimiento.Tests/WorkflowInboxServiceTests.cs`
  - `tests/TramiteDiasVencimiento.Tests/WorkflowInboxQueryBuilderTests.cs`

## Current Publish State

- `DocuArchi.Api`, `MiApp.Services` and `MiApp.DTOs` are now on branch `scrum-126-apptable-export-19-definir-estrategia-ba`.
- `MiApp.Services` still contains a pre-existing local modification in `WorkflowInboxContextResolverService.cs` that is not part of the export feature, so any commit/publish step must keep that in mind and avoid accidental mixing.
- `DocuArchi.Api` includes generated build artifacts under `bin/` and `obj/`; those should not be included in a source commit.
- Clean publish-ready worktrees were created from `origin/main`:
  - `..\DocuArchi.Api.scrum126-clean`
  - `..\MiApp.Services.scrum126-clean`
  - `..\MiApp.DTOs.scrum126-clean`
- Those clean worktrees contain only `SCRUM-126` source changes and no extra commits over `origin/main`; their PRs are now open in GitHub and recorded in the matrix above.