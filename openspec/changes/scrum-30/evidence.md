# SCRUM-30 - Evidence Template

Use this file to capture implementation evidence across repositories before archive.

## Coordinator Repo

- Repo: `DocuArchiCore`
- Change: `openspec/changes/scrum-30/`
- OpenSpec validate:
  - Command: `openspec.cmd validate scrum-30`
  - Result: `pass`
- Local tests:
  - Command: `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj`
  - Result: `pass (20/20)`
  - Added tests:
    - `ListaRadicadosPendientesServiceTests`
    - `ListaRadicadosPendientesRepositoryTests`
- API build:
  - Command: `dotnet build ../DocuArchi.Api/DocuArchi.Api.csproj`
  - Result: `pass`

## Jira Snapshot

- Captured at: `2026-03-02T10:16:45-05:00`
- Issue: `SCRUM-30`
- Status: `Por hacer`
- Summary: `CREA-API-LISTA-RADICADOS-PEDIENTES`
- Assignee: `Miguel Angel Urueta Miranda`
- Updated: `2026-03-02T09:57:34.675-0500`

## PR Evidence

| Repo | PR URL | Status (`open/review/merged`) | Commit SHA | Notes |
|---|---|---|---|---|
| `DocuArchi.Api` | `pending` | `todo` | `pending` | `Controller + Program.cs` |
| `MiApp.DTOs` | `pending` | `todo` | `pending` | `DTO salida MUI` |
| `MiApp.Services` | `pending` | `todo` | `pending` | `Service + mapping` |
| `MiApp.Repository` | `pending` | `todo` | `pending` | `Query parametrizada` |
| `MiApp.Models` | `pending` | `todo` | `pending` | `Modelo tabla` |

## Test Evidence by Repo

| Repo | Unit | Integration | Contract | Result |
|---|---|---|---|---|
| `DocuArchi.Api` | `pending` | `pending` | `pending` | `todo` |
| `MiApp.Services` | `pending` | `pending` | `pending` | `todo` |
| `MiApp.Repository` | `pending` | `pending` | `pending` | `todo` |

## Archive Checklist

- [ ] PRs de repos impactados en `merged`.
- [ ] `sync.md` actualizado con URLs y estado final.
- [ ] `opsxj:archive SCRUM-30` ejecutado en cada repo impactado.
- [ ] `opsxj:archive SCRUM-30` ejecutado en repo coordinador.
