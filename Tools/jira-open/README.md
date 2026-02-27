# jira-open Commands

This folder exposes PowerShell-based integration commands for OpenSpec + Jira.

Guia recomendada de operacion multi-repo:
- `Docs/Guia_Opsxj_MultiRepo.md`

## Prerequisites

- `openspec.cmd` available in PATH
- `git` with a configured `origin` remote
- `gh` (GitHub CLI) available in PATH and authenticated (`gh auth login`)
- PowerShell execution with `-ExecutionPolicy Bypass`
- Jira config:
  - Fill `Tools/jira-open/.jira-open.env` (local file), or
  - Export env vars `JIRA_BASE_URL`, `JIRA_EMAIL`, `JIRA_API_TOKEN`
- Git/GitHub connection config in the same `Tools/jira-open/.jira-open.env`:
  - `GIT_REMOTE_NAME` (default: `origin`)
  - `GIT_BASE_BRANCH` (optional, overrides base branch auto-detection for PRs)
  - `GITHUB_TOKEN` (optional, enables token-based `gh` auth for non-interactive environments)
  - `GITHUBREPO` or `GITHUB_REPO` (optional). If empty, `opsxj` auto-detects `owner/repo` from `git remote get-url <GIT_REMOTE_NAME>`.

## Commands

Run from repository root:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-jira
```

Then create a change:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:new -- <ISSUE-KEY> [-SkipJira]
```

Examples:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:new -- OPSXJ-200 -SkipJira
npm.cmd --prefix Tools/jira-open run opsxj:new -- OPSXJ-200
npm.cmd --prefix Tools/jira-open run opsxj:new -- OPSXJ-200 -SelectRepos
```

Run PR flow tests:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-pr
```

Run archive flow tests:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:test-archive
```

Archive by issue key (or explicit change name):

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:archive -- <ISSUE-KEY|CHANGE-NAME> [-Yes] [-SkipSpecs] [-NoValidate]
```

Example:

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:archive -- OPSXJ-200 -Yes -SkipSpecs
```

## Notes

- `opsxj:new` creates/updates `openspec/changes/<change-name>/` artifacts (including `sync.md` impact matrix), runs `openspec.cmd validate`, creates/pushes a branch, and opens a GitHub PR.
- `opsxj:new -SelectRepos` asks interactively which repositories are impacted and pre-fills `sync.md` (`yes/no`, `pending/n/a`).
- PR title is based on Jira ticket `summary`.
- If a PR already exists for the change branch, `opsxj:new` reports that PR instead of creating a duplicate.
- `opsxj:archive` validates that the change branch is merged into base branch (`GIT_BASE_BRANCH` or auto-detected) before archiving.
- After successful archive, `opsxj:archive` transitions Jira issue to a done state (e.g. `Done`, `Terminado`, `Closed`).
- `-SkipJira` allows offline flow when Jira credentials are not configured.
- In this environment, running via `npm --prefix Tools/jira-open` from repo root is more reliable than running from `Tools/jira-open` directly.
