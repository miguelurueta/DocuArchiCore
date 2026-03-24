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

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("opsxj-orchestrate-impact-" + [System.Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null

try {
    $workspaceRoot = Join-Path $tempRoot "workspace"
    $repoGroupRoot = Join-Path $workspaceRoot "DocuArchiCore"
    $orchestratorRoot = Join-Path $repoGroupRoot "DocuArchiCore"
    $satelliteRoot = Join-Path $repoGroupRoot "MiApp.Services"
    $toolDir = Join-Path $orchestratorRoot "Tools/jira-open"
    $fakeBin = Join-Path $tempRoot "fake-bin"

    New-Item -ItemType Directory -Path $toolDir -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $orchestratorRoot "openspec/context") -Force | Out-Null
    New-Item -ItemType Directory -Path $satelliteRoot -Force | Out-Null
    New-Item -ItemType Directory -Path $fakeBin -Force | Out-Null

    Copy-Item -Path (Join-Path $PSScriptRoot "opsxj.ps1") -Destination (Join-Path $toolDir "opsxj.ps1") -Force
    Set-Content -Path (Join-Path $fakeBin "openspec.cmd") -Value "@echo off`r`nexit /b 0" -NoNewline

    Set-Content -Path (Join-Path $orchestratorRoot "README.md") -Value "orchestrator repo" -NoNewline
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("init")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("config", "user.email", "opsxj-test@example.com")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("config", "user.name", "opsxj-test")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("add", ".")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("commit", "-m", "init")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("branch", "-M", "main")
    Run-Git -RepoRoot $orchestratorRoot -CliArgs @("remote", "add", "origin", "https://github.com/example/DocuArchiCore.git")

    Set-Content -Path (Join-Path $satelliteRoot "service.txt") -Value "base" -NoNewline
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("init")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("config", "user.email", "opsxj-test@example.com")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("config", "user.name", "opsxj-test")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("add", ".")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("commit", "-m", "init")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("branch", "-M", "main")
    Run-Git -RepoRoot $satelliteRoot -CliArgs @("remote", "add", "origin", "https://github.com/example/MiApp.Services.git")

    $previousPath = $env:PATH
    $env:PATH = "$fakeBin;$previousPath"
    $env:OPSXJ_IMPACT_REPOS = "MiApp.Services"
    $env:OPSXJ_TEST_SKIP_GIT_PUSH = "1"
    $env:JIRA_BASE_URL = "https://example.atlassian.net"
    $env:JIRA_EMAIL = "bot@example.com"
    $env:JIRA_API_TOKEN = "token"
    $env:GITHUB_TOKEN = "test-token"

    $global:IssueStates = @{}
    $global:CreatedPullRequests = @{}
    function global:Invoke-RestMethod {
        param(
            [string]$Method,
            [string]$Uri,
            [hashtable]$Headers,
            [object]$Body,
            [string]$ContentType
        )

        if ($Uri -match "/rest/api/3/issue/(?<issue>[A-Z]+-\d+)") {
            $issueKey = $Matches["issue"]
        }

        if ($Method -ieq "Get" -and $Uri -like "*/rest/api/3/issue/*?fields=summary,description,status") {
            if (-not $global:IssueStates.ContainsKey($issueKey)) {
                $global:IssueStates[$issueKey] = "Por hacer"
            }
            return @{
                fields = @{
                    summary = "Impact classification validation"
                    description = @{
                        type = "doc"
                        content = @(
                            @{
                                type = "paragraph"
                                content = @(
                                    @{ type = "text"; text = "Impacts MiApp.Services only for orchestrated traceability." }
                                )
                            }
                        )
                    }
                    status = @{
                        name = $global:IssueStates[$issueKey]
                    }
                }
            }
        }

        if ($Method -ieq "Get" -and $Uri -like "*/rest/api/3/issue/*/transitions") {
            $currentState = [string]$global:IssueStates[$issueKey]
            if ($currentState -eq "En curso") {
                return @{
                    transitions = @(
                        @{
                            id = "21"
                            to = @{ name = "En Revision" }
                        }
                    )
                }
            }

            return @{
                transitions = @(
                    @{
                        id = "11"
                        to = @{ name = "En curso" }
                    }
                    @{
                        id = "21"
                        to = @{ name = "En Revision" }
                    }
                )
            }
        }

        if ($Method -ieq "Post" -and $Uri -like "*/rest/api/3/issue/*/transitions") {
            $bodyText = if ($Body -is [byte[]]) { [System.Text.Encoding]::UTF8.GetString($Body) } else { [string]$Body }
            if ($bodyText -like '*"11"*') {
                $global:IssueStates[$issueKey] = "En curso"
                return @{}
            }
            if ($bodyText -like '*"21"*') {
                $global:IssueStates[$issueKey] = "En Revision"
                return @{}
            }
            return @{}
        }

        if ($Method -ieq "Post" -and $Uri -like "*/rest/api/3/issue/*/comment") {
            return @{ id = "10001" }
        }

        if ($Method -ieq "Get" -and $Uri -like "https://api.github.com/repos/*/pulls?state=all*") {
            $headMatch = [regex]::Match($Uri, "head=[^:]+:(?<branch>[^&]+)")
            $branchName = if ($headMatch.Success) { [uri]::UnescapeDataString($headMatch.Groups["branch"].Value) } else { "" }
            if ($global:CreatedPullRequests.ContainsKey($branchName)) {
                return @(
                    @{
                        number = $global:CreatedPullRequests[$branchName].number
                        title = $global:CreatedPullRequests[$branchName].title
                        html_url = $global:CreatedPullRequests[$branchName].url
                    }
                )
            }
            return @()
        }

        if ($Method -ieq "Post" -and $Uri -like "https://api.github.com/repos/*/pulls") {
            $decodedBody = if ($Body -is [byte[]]) { [System.Text.Encoding]::UTF8.GetString($Body) } else { [string]$Body }
            $payload = $decodedBody | ConvertFrom-Json
            $prNumber = 100 + $global:CreatedPullRequests.Count + 1
            $prUrl = "https://github.com/example/repo/pull/$prNumber"
            $global:CreatedPullRequests[$payload.head] = @{
                number = $prNumber
                title = [string]$payload.title
                url = $prUrl
            }
            return @{
                html_url = $prUrl
            }
        }

        throw "Unexpected Invoke-RestMethod call: $Method $Uri"
    }

    try {
        $scriptPath = Join-Path $toolDir "opsxj.ps1"
        Push-Location $orchestratorRoot
        try {
            $output = (& $scriptPath orchestrate:new OPSXJ-2001 -NonInteractive 2>&1 | Out-String)
            Assert-Contains -Value $output -Expected "Orchestrated repos: DocuArchiCore, MiApp.Services"
            Assert-Contains -Value $output -Expected "Satellite repo tracked without PR [MiApp.Services]: traceability_only"
            if ($output.Contains("Satellite PR created [MiApp.Services]")) {
                throw "Satellite repo should not create a PR unless explicitly marked in OPSXJ_IMPLEMENTATION_REPOS."
            }

            $changeName = "opsxj-2001-impact-classification-validation"
            $syncPath = Join-Path $orchestratorRoot "openspec/changes/$changeName/sync.md"
            $syncText = Get-Content -Path $syncPath -Raw
            Assert-Contains -Value $syncText -Expected '| MiApp.Services | yes | traceability_only |'
            Assert-Contains -Value $syncText -Expected '| MiApp.Services | yes | traceability_only | trazabilidad centralizada sin diff funcional | n/a | n/a | pending |'

            $env:OPSXJ_IMPLEMENTATION_REPOS = "MiApp.Services"
            $outputWithImplementation = (& $scriptPath orchestrate:new OPSXJ-2002 -NonInteractive 2>&1 | Out-String)
            Assert-Contains -Value $outputWithImplementation -Expected "Satellite PR created [MiApp.Services]"

            $changeNameWithImplementation = "opsxj-2002-impact-classification-validation"
            $syncPathWithImplementation = Join-Path $orchestratorRoot "openspec/changes/$changeNameWithImplementation/sync.md"
            $syncTextWithImplementation = Get-Content -Path $syncPathWithImplementation -Raw
            Assert-Contains -Value $syncTextWithImplementation -Expected '| MiApp.Services | yes | implementation_required |'
            Assert-Contains -Value $syncTextWithImplementation -Expected '| MiApp.Services | yes | implementation_required | <definir alcance> | done | https://github.com/example/repo/pull/'
        }
        finally {
            Pop-Location
        }
    }
    finally {
        Remove-Item Function:\Invoke-RestMethod -ErrorAction SilentlyContinue
        $env:PATH = $previousPath
        Remove-Item Env:OPSXJ_IMPACT_REPOS -ErrorAction SilentlyContinue
        Remove-Item Env:OPSXJ_TRACEABILITY_REPOS -ErrorAction SilentlyContinue
        Remove-Item Env:OPSXJ_IMPLEMENTATION_REPOS -ErrorAction SilentlyContinue
        Remove-Item Env:OPSXJ_TEST_SKIP_GIT_PUSH -ErrorAction SilentlyContinue
        Remove-Item Env:JIRA_BASE_URL -ErrorAction SilentlyContinue
        Remove-Item Env:JIRA_EMAIL -ErrorAction SilentlyContinue
        Remove-Item Env:JIRA_API_TOKEN -ErrorAction SilentlyContinue
        Remove-Item Env:GITHUB_TOKEN -ErrorAction SilentlyContinue
    }

    Write-Output "PASS: opsxj:orchestrate:new only creates satellite PRs for OPSXJ_IMPLEMENTATION_REPOS."
}
finally {
    Remove-Item -Path $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
}
