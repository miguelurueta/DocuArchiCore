# SCRUM-30 - Commands Runbook (Multi-Repo)

Use this runbook to execute remaining tasks (`3.1`, `3.2`, `4.1`) in impacted repositories.

## Common Preconditions (each repo)

```powershell
git status --short
npm.cmd --prefix Tools/jira-open run opsxj:doctor -- SCRUM-30
```

If working tree is dirty, commit/stash before running `opsxj`.

## 1) Create/Open change per impacted repo

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:new -- SCRUM-30
```

Run in repos:

- `DocuArchi.Api`
- `MiApp.DTOs`
- `MiApp.Services`
- `MiApp.Repository`
- `MiApp.Models`

## 2) Implement and test

```powershell
openspec.cmd instructions apply --change "scrum-30" --json
dotnet test
```

If integration tests depend on Docker and are unavailable, mark `skipped` with explicit reason in test output/evidence.

## 3) Validate OpenSpec before PR

```powershell
openspec.cmd validate scrum-30
```

## 4) Create PR and capture evidence

```powershell
git push -u origin <branch>
gh pr create --fill
```

Update in coordinator repo:

- `openspec/changes/scrum-30/sync.md`
- `openspec/changes/scrum-30/evidence.md`

## 5) Archive after merge

In each impacted repo after PR merge:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:archive -- SCRUM-30 -Yes -SkipSpecs
```

Finally in coordinator repo:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:archive -- SCRUM-30 -Yes -SkipSpecs
```

