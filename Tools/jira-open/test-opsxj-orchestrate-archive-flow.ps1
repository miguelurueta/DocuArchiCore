$ErrorActionPreference = "Stop"

function Assert-Contains {
    param(
        [string]$Value,
        [string]$Expected
    )
    if (-not $Value.Contains($Expected)) {
        throw "Expected output to contain '$Expected'. Actual: $Value"
    }
}

function Run-Git {
    param(
        [string]$RepoRoot,
        [string[]]$CliArgs
    )
    Push-Location $RepoRoot
    try {
        $previousEap = $ErrorActionPreference
        $ErrorActionPreference = "Continue"
        $output = & git @CliArgs 2>&1
        $ErrorActionPreference = $previousEap
        if ($LASTEXITCODE -ne 0) {
            $text = ($output | ForEach-Object { "$_" }) -join "`n"
            throw "git $($CliArgs -join ' ') failed. $text"
        }
    }
    finally {
        Pop-Location
    }
}

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("opsxj-orchestrate-archive-" + [System.Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null

try {
    $workspaceRoot = Join-Path $tempRoot "workspace"
    $repoGroupRoot = Join-Path $workspaceRoot "DocuArchiCore"
    $orchestratorRoot = Join-Path $repoGroupRoot "DocuArchiCore"
    $satelliteRoot = Join-Path $repoGroupRoot "MiApp.Services"
    $toolDir = Join-Path $orchestratorRoot "Tools/jira-open"
    $fakeBin = Join-Path $tempRoot "fake-bin"
    $issueKey = "OPSXJ-4000"
    $changeName = "opsxj-4000-orchestrated-archive-cleanup"

    New-Item -ItemType Directory -Path $toolDir -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $orchestratorRoot "openspec/changes/$changeName") -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $orchestratorRoot "openspec/logs") -Force | Out-Null
    New-Item -ItemType Directory -Path $satelliteRoot -Force | Out-Null
    New-Item -ItemType Directory -Path $fakeBin -Force | Out-Null

    Copy-Item -Path (Join-Path $PSScriptRoot "opsxj.ps1") -Destination (Join-Path $toolDir "opsxj.ps1") -Force
    Set-Content -Path (Join-Path $fakeBin "openspec.cmd") -Value "@echo off`r`nexit /b 0" -NoNewline

    Set-Content -Path (Join-Path $orchestratorRoot "README.md") -Value "orchestrator repo" -NoNewline
    Set-Content -Path (Join-Path $orchestratorRoot "openspec/changes/$changeName/.openspec.yaml") -Value "schema: spec-driven`ncreated: 2026-03-24" -NoNewline
    Set-Content -Path (Join-Path $orchestratorRoot "openspec/changes/$changeName/sync.md") -Value @'
## Repo Impact Plan

| Repo | Impacta? | Tipo impacto | Motivo | opsxj:new | PR | opsxj:archive | Estado |
|---|---|---|---|---|---|---|---|
| `DocuArchiCore` | `yes` | `implementation_required` | `orquestador openspec central` | `done` | `https://github.com/example/DocuArchiCore/pull/500` | `pending` | `in_review` |
| `MiApp.Services` | `yes` | `implementation_required` | `service implementation` | `done` | `https://github.com/example/MiApp.Services/pull/321` | `pending` | `in_review` |
'@ -NoNewline
    Set-Content -Path (Join-Path $orchestratorRoot "openspec/logs/$issueKey.log.jsonl") -Value '{"step":"before-cleanup"}' -NoNewline

    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("init")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("config", "user.email", "opsxj-test@example.com")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("config", "user.name", "opsxj-test")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("add", ".")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("commit", "-m", "init")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("branch", "-M", "main")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("remote", "add", "origin", "https://github.com/example/DocuArchiCore.git")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("checkout", "-b", $changeName)
    Set-Content -Path (Join-Path $orchestratorRoot "archive-feature.txt") -Value "archive change" -NoNewline
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("add", "archive-feature.txt")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("commit", "-m", "OPSXJ-4000 archive feature")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("checkout", "main")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("merge", "--no-ff", $changeName, "-m", "merge $changeName")

    Set-Content -Path (Join-Path $satelliteRoot "service.txt") -Value "service repo" -NoNewline
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("init")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("config", "user.email", "opsxj-test@example.com")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("config", "user.name", "opsxj-test")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("add", ".")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("commit", "-m", "init")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("branch", "-M", "main")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("remote", "add", "origin", "https://github.com/example/MiApp.Services.git")

    $worktreePath = Join-Path $workspaceRoot ".tmp/opsxj/opsxj-40-miapp-se-opsxj-4000-o"
    $worktreeParent = Split-Path -Parent $worktreePath
    New-Item -ItemType Directory -Path $worktreeParent -Force | Out-Null
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("worktree", "add", "--detach", $worktreePath, "HEAD")
    Set-Content -Path (Join-Path $worktreePath ".opsxj-marker.txt") -Value "managed worktree" -NoNewline

    $metadataRoot = Join-Path $orchestratorRoot ".opsxj/orchestrator/worktrees/opsxj-4000"
    New-Item -ItemType Directory -Path $metadataRoot -Force | Out-Null
    Set-Content -Path (Join-Path $metadataRoot "miapp-services.json") -Value @"
{
  "issueKey": "$issueKey",
  "repo": "MiApp.Services",
  "repoRoot": "$($satelliteRoot -replace '\\','\\\\')",
  "changeName": "$changeName",
  "worktreePath": "$($worktreePath -replace '\\','\\\\')",
  "createdUtc": "2026-03-24T00:00:00.0000000Z",
  "lastUsedUtc": "2026-03-24T00:00:00.0000000Z",
  "checkoutState": {
    "currentBranch": "main",
    "reasons": [ "branch_in_use" ]
  }
}
"@ -NoNewline

    $previousPath = $env:PATH
    $env:PATH = "$fakeBin;$previousPath"
    $env:JIRA_BASE_URL = "https://example.atlassian.net"
    $env:JIRA_EMAIL = "bot@example.com"
    $env:JIRA_API_TOKEN = "token"
    $env:GIT_BASE_BRANCH = "main"
    $env:GITHUB_TOKEN = "test-token"
    $env:OPSXJ_TEST_SKIP_REMOTE_FETCH = "1"
    $env:OPSXJ_TEST_SKIP_GIT_PUSH = "1"

    function global:Invoke-RestMethod {
        param(
            [string]$Method,
            [string]$Uri,
            [hashtable]$Headers,
            [string]$Body
        )

        if ($Method -ieq "Get" -and $Uri -like "https://api.github.com/repos/*/pulls/*") {
            return @{
                merged = $true
                merged_at = "2026-03-24T00:00:00Z"
                state = "closed"
                html_url = if ($Uri -like "*/500") { "https://github.com/example/DocuArchiCore/pull/500" } else { "https://github.com/example/MiApp.Services/pull/321" }
            }
        }

        if ($Method -ieq "Get" -and $Uri -like "*/rest/api/3/issue/*?fields=summary,description*") {
            return @{
                fields = @{
                    summary = "Archive orchestration cleanup"
                    description = @{
                        type = "doc"
                        content = @(
                            @{
                                type = "paragraph"
                                content = @(
                                    @{ type = "text"; text = "Test description" }
                                )
                            }
                        )
                    }
                }
            }
        }

        if ($Method -ieq "Get" -and $Uri -like "*/rest/api/3/issue/*/transitions") {
            return @{
                transitions = @(
                    @{
                        id = "31"
                        to = @{ name = "Done" }
                    }
                )
            }
        }

        if ($Method -ieq "Post" -and $Uri -like "*/rest/api/3/issue/*/transitions") {
            return @{}
        }

        throw "Unexpected Invoke-RestMethod call: $Method $Uri"
    }

    Push-Location $orchestratorRoot
    try {
        $scriptPath = Join-Path $toolDir "opsxj.ps1"
        $output = (& $scriptPath "orchestrate:archive" $issueKey -Yes -SkipSpecs -NonInteractive 2>&1 | Out-String)
        Assert-Contains -Value $output -Expected "Archived change: $changeName"
        Assert-Contains -Value $output -Expected "Jira issue transitioned: $issueKey -> Done"

        if (Test-Path $worktreePath) {
            throw "Managed worktree should have been cleaned: $worktreePath"
        }
        if (Test-Path (Join-Path $metadataRoot "miapp-services.json")) {
            throw "Worktree metadata should have been cleaned."
        }
        if (Test-Path (Join-Path $orchestratorRoot "openspec/logs/$issueKey.log.jsonl")) {
            throw "Issue log should have been cleaned."
        }
    }
    finally {
        Pop-Location
        Remove-Item Function:\Invoke-RestMethod -ErrorAction SilentlyContinue
        $env:PATH = $previousPath
        Remove-Item Env:JIRA_BASE_URL -ErrorAction SilentlyContinue
        Remove-Item Env:JIRA_EMAIL -ErrorAction SilentlyContinue
        Remove-Item Env:JIRA_API_TOKEN -ErrorAction SilentlyContinue
        Remove-Item Env:GIT_BASE_BRANCH -ErrorAction SilentlyContinue
        Remove-Item Env:GITHUB_TOKEN -ErrorAction SilentlyContinue
        Remove-Item Env:OPSXJ_TEST_SKIP_REMOTE_FETCH -ErrorAction SilentlyContinue
        Remove-Item Env:OPSXJ_TEST_SKIP_GIT_PUSH -ErrorAction SilentlyContinue
    }

    Write-Output "PASS: opsxj:orchestrate:archive trusts merged PRs and cleans issue artifacts."
}
finally {
    Remove-Item -Path $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
}
