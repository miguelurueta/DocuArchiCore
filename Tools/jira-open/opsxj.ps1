param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet("new", "archive")]
    [string]$Command,

    [Parameter(Mandatory = $true, Position = 1)]
    [string]$IssueOrChange,

    [switch]$SkipJira,
    [switch]$Yes,
    [switch]$SkipSpecs,
    [switch]$NoValidate
)

$ErrorActionPreference = "Stop"

function Get-JiraConfigValue {
    param(
        [string]$Key,
        [string]$ConfigPath
    )

    if (-not (Test-Path $ConfigPath)) {
        return $null
    }

    $line = Get-Content -Path $ConfigPath |
        Where-Object { $_ -match "^\s*$Key\s*=" } |
        Select-Object -First 1

    if (-not $line) { return $null }
    return (($line -split "=", 2)[1]).Trim()
}

function Get-ConfigValue {
    param(
        [string]$Key,
        [string]$ConfigPath,
        [string]$DefaultValue = ""
    )

    $fileValue = Get-JiraConfigValue -Key $Key -ConfigPath $ConfigPath
    if (-not [string]::IsNullOrWhiteSpace($fileValue)) {
        return $fileValue.Trim()
    }

    $envValue = [Environment]::GetEnvironmentVariable($Key)
    if (-not [string]::IsNullOrWhiteSpace($envValue)) {
        return $envValue.Trim()
    }

    return $DefaultValue
}

function Get-FirstConfigValue {
    param(
        [string[]]$Keys,
        [string]$ConfigPath,
        [string]$DefaultValue = ""
    )

    foreach ($k in $Keys) {
        $value = Get-ConfigValue -Key $k -ConfigPath $ConfigPath
        if (-not [string]::IsNullOrWhiteSpace($value)) {
            return $value.Trim()
        }
    }

    return $DefaultValue
}

function Get-ToolConfig {
    $configPath = Join-Path $PSScriptRoot ".jira-open.env"
    return @{
        jiraBaseUrl = Get-ConfigValue -Key "JIRA_BASE_URL" -ConfigPath $configPath
        jiraEmail = Get-ConfigValue -Key "JIRA_EMAIL" -ConfigPath $configPath
        jiraApiToken = Get-ConfigValue -Key "JIRA_API_TOKEN" -ConfigPath $configPath
        gitRemoteName = Get-ConfigValue -Key "GIT_REMOTE_NAME" -ConfigPath $configPath -DefaultValue "origin"
        gitBaseBranch = Get-ConfigValue -Key "GIT_BASE_BRANCH" -ConfigPath $configPath
        githubToken = Get-FirstConfigValue -Keys @("GITHUB_TOKEN") -ConfigPath $configPath
        githubRepo = Get-FirstConfigValue -Keys @("GITHUBREPO", "GITHUB_REPO") -ConfigPath $configPath
    }
}

function Get-IssueKeyFromChangeName {
    param([string]$ChangeName)
    if (-not $ChangeName) { return $null }

    if ($ChangeName -match "^([A-Za-z]+-\d+)$") {
        return $Matches[1].ToUpperInvariant()
    }
    if ($ChangeName -match "^([A-Za-z]+-\d+)-") {
        return $Matches[1].ToUpperInvariant()
    }

    return $null
}

function Get-RepoRoot {
    return (Resolve-Path (Join-Path $PSScriptRoot "..\\..")).Path
}

function To-KebabCase {
    param([string]$Value)
    if (-not $Value) { return "" }
    $normalized = $Value.ToLowerInvariant()
    $normalized = $normalized -replace "[^a-z0-9]+", "-"
    $normalized = $normalized.Trim("-")
    $normalized = $normalized -replace "-{2,}", "-"
    return $normalized
}

function Invoke-OpenSpec {
    param(
        [string]$RepoRoot,
        [string[]]$CliArgs
    )
    Push-Location $RepoRoot
    try {
        & openspec.cmd @CliArgs
        if ($LASTEXITCODE -ne 0) {
            throw "openspec.cmd failed: $($CliArgs -join ' ')"
        }
    }
    finally {
        Pop-Location
    }
}

function Invoke-Git {
    param(
        [string[]]$CliArgs,
        [switch]$IgnoreExitCode
    )

    $previousEap = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    $output = & git @CliArgs 2>&1
    $exitCode = $LASTEXITCODE
    $ErrorActionPreference = $previousEap
    $text = ($output | ForEach-Object { "$_" }) -join "`n"

    if (-not $IgnoreExitCode -and $exitCode -ne 0) {
        throw "git $($CliArgs -join ' ') failed. $text"
    }

    return $text.Trim()
}

function Invoke-Gh {
    param(
        [string[]]$CliArgs,
        [switch]$IgnoreExitCode
    )

    function Resolve-GhExecutable {
        $cmd = Get-Command gh -ErrorAction SilentlyContinue
        if ($cmd -and $cmd.Source) { return $cmd.Source }
        $fallback = "C:\\Program Files\\GitHub CLI\\gh.exe"
        if (Test-Path $fallback) { return $fallback }
        return $null
    }

    $toolConfig = Get-ToolConfig
    $token = [string]$toolConfig.githubToken
    $ghExe = Resolve-GhExecutable
    if (-not $ghExe) {
        throw "GitHub CLI (gh) is not installed or not in PATH. Install gh and retry."
    }

    $previousEap = $ErrorActionPreference
    $previousGhToken = [Environment]::GetEnvironmentVariable("GH_TOKEN")
    $previousGithubToken = [Environment]::GetEnvironmentVariable("GITHUB_TOKEN")
    try {
        if (-not [string]::IsNullOrWhiteSpace($token)) {
            $env:GH_TOKEN = $token
            $env:GITHUB_TOKEN = $token
        }

        $ErrorActionPreference = "Continue"
        $output = & $ghExe @CliArgs 2>&1
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previousEap
        if ($null -eq $previousGhToken) { Remove-Item Env:GH_TOKEN -ErrorAction SilentlyContinue } else { $env:GH_TOKEN = $previousGhToken }
        if ($null -eq $previousGithubToken) { Remove-Item Env:GITHUB_TOKEN -ErrorAction SilentlyContinue } else { $env:GITHUB_TOKEN = $previousGithubToken }
    }
    $text = ($output | ForEach-Object { "$_" }) -join "`n"

    if (-not $IgnoreExitCode -and $exitCode -ne 0) {
        throw "gh $($CliArgs -join ' ') failed. $text"
    }

    return $text.Trim()
}

function Get-DefaultBaseBranch {
    param(
        [string]$RemoteName,
        [string]$ConfiguredBaseBranch
    )

    function Test-GitRef {
        param([string]$RefName)
        & git show-ref --verify --quiet $RefName
        return ($LASTEXITCODE -eq 0)
    }

    if (-not [string]::IsNullOrWhiteSpace($ConfiguredBaseBranch)) {
        return $ConfiguredBaseBranch.Trim()
    }

    try {
        $remoteHead = Invoke-Git -CliArgs @("symbolic-ref", "refs/remotes/$RemoteName/HEAD")
        if ($remoteHead -match "refs/remotes/$RemoteName/(.+)$") {
            return $Matches[1]
        }
    }
    catch {
        # Fallback below
    }

    if (Test-GitRef -RefName "refs/heads/main") {
        return "main"
    }
    if (Test-GitRef -RefName "refs/heads/master") {
        return "master"
    }

    return "main"
}

function Convert-RemoteUrlToGitHubRepo {
    param([string]$RemoteUrl)

    if ([string]::IsNullOrWhiteSpace($RemoteUrl)) {
        return $null
    }

    $value = $RemoteUrl.Trim()

    if ($value -match "^git@github\.com:(?<repo>[^/]+/[^/]+?)(?:\.git)?/?$") {
        return $Matches["repo"]
    }
    if ($value -match "^https://github\.com/(?<repo>[^/]+/[^/]+?)(?:\.git)?/?$") {
        return $Matches["repo"]
    }
    if ($value -match "^ssh://git@github\.com/(?<repo>[^/]+/[^/]+?)(?:\.git)?/?$") {
        return $Matches["repo"]
    }

    return $null
}

function Resolve-GitHubRepoFromRemote {
    param([string]$RemoteName)

    $remoteUrl = Invoke-Git -CliArgs @("remote", "get-url", $RemoteName) -IgnoreExitCode
    if ([string]::IsNullOrWhiteSpace($remoteUrl)) {
        return $null
    }

    return Convert-RemoteUrlToGitHubRepo -RemoteUrl $remoteUrl
}

function Assert-GitHubPrerequisites {
    param(
        [switch]$SkipGhAuth,
        [string]$RemoteName
    )

    $toolConfig = Get-ToolConfig
    $githubToken = [string]$toolConfig.githubToken

    $ghCommand = Get-Command gh -ErrorAction SilentlyContinue
    $ghFallback = "C:\\Program Files\\GitHub CLI\\gh.exe"
    $ghExists = [bool]$ghCommand -or (Test-Path $ghFallback)

    if (-not $ghExists) {
        if (-not $SkipGhAuth) {
            throw "GitHub CLI (gh) is not installed or not in PATH. Install gh and retry."
        }
    }

    $insideWorkTree = Invoke-Git -CliArgs @("rev-parse", "--is-inside-work-tree")
    if ($insideWorkTree -ne "true") {
        throw "Current path is not inside a git repository."
    }

    $originUrl = Invoke-Git -CliArgs @("remote", "get-url", $RemoteName) -IgnoreExitCode
    if (-not $originUrl) {
        throw "Git remote '$RemoteName' is not configured. Configure it before creating a PR."
    }

    if (-not $SkipGhAuth -and [string]::IsNullOrWhiteSpace($githubToken)) {
        try {
            Invoke-Gh -CliArgs @("auth", "status") | Out-Null
        }
        catch {
            throw "GitHub CLI is not authenticated. Run 'gh auth login' and retry."
        }
    }
}

function Ensure-ChangeBranch {
    param([string]$BranchName)

    $currentBranch = Invoke-Git -CliArgs @("rev-parse", "--abbrev-ref", "HEAD")
    if ($currentBranch -eq $BranchName) {
        return $currentBranch
    }

    Invoke-Git -CliArgs @("show-ref", "--verify", "--quiet", "refs/heads/$BranchName") -IgnoreExitCode | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Invoke-Git -CliArgs @("checkout", $BranchName) | Out-Null
    }
    else {
        Invoke-Git -CliArgs @("checkout", "-b", $BranchName) | Out-Null
    }

    return (Invoke-Git -CliArgs @("rev-parse", "--abbrev-ref", "HEAD"))
}

function Ensure-CommitForChangeArtifacts {
    param(
        [string]$ChangeName,
        [string]$CommitMessage
    )

    $changePath = "openspec/changes/$ChangeName"
    Invoke-Git -CliArgs @("add", "--", $changePath) | Out-Null

    $staged = Invoke-Git -CliArgs @("diff", "--cached", "--name-only", "--", $changePath)
    if (-not $staged) {
        return $false
    }

    Invoke-Git -CliArgs @("commit", "-m", $CommitMessage) | Out-Null
    return $true
}

function Get-ExistingPullRequest {
    param(
        [string]$BranchName,
        [string]$Repo
    )

    $args = @("pr", "list", "--head", $BranchName, "--state", "open", "--json", "number,title,url")
    if (-not [string]::IsNullOrWhiteSpace($Repo)) {
        $args += @("--repo", $Repo)
    }

    $json = Invoke-Gh -CliArgs $args
    if (-not $json) { return $null }

    $items = @($json | ConvertFrom-Json)
    if ($items.Count -gt 0) {
        return $items[0]
    }

    return $null
}

function Ensure-GitHubPullRequest {
    param(
        [string]$RepoRoot,
        [string]$ChangeName,
        [hashtable]$Issue
    )

    Push-Location $RepoRoot
    try {
        $toolConfig = Get-ToolConfig
        $remoteName = $toolConfig.gitRemoteName
        if (-not $remoteName) { $remoteName = "origin" }
        $githubRepo = [string]$toolConfig.githubRepo
        if ([string]::IsNullOrWhiteSpace($githubRepo)) {
            $githubRepo = Resolve-GitHubRepoFromRemote -RemoteName $remoteName
        }

        $fakePrUrl = [string]$env:OPSXJ_TEST_FAKE_PR_URL
        Assert-GitHubPrerequisites -SkipGhAuth:([bool]$fakePrUrl) -RemoteName $remoteName

        $branchName = Ensure-ChangeBranch -BranchName $ChangeName
        $commitTitle = "$($Issue.key): $($Issue.summary)"
        if ($commitTitle.Length -gt 120) {
            $commitTitle = $commitTitle.Substring(0, 120)
        }

        try {
            $createdCommit = Ensure-CommitForChangeArtifacts -ChangeName $ChangeName -CommitMessage $commitTitle
        }
        catch {
            throw "Unable to create commit for change artifacts. $($_.Exception.Message)"
        }

        if ($env:OPSXJ_TEST_SKIP_GIT_PUSH -eq "1") {
            # Test-only escape hatch to avoid network/remote side effects in automated tests.
        }
        else {
            try {
                Invoke-Git -CliArgs @("push", "--set-upstream", $remoteName, $branchName) | Out-Null
            }
            catch {
                throw "Unable to push branch '$branchName' to remote '$remoteName'. $($_.Exception.Message)"
            }
        }

        if ($fakePrUrl) {
            $fakePrPath = Join-Path $RepoRoot "openspec\\changes\\$ChangeName\\.opsxj-pr-url.txt"
            if (Test-Path $fakePrPath) {
                return @{
                    created = $false
                    url = (Get-Content -Path $fakePrPath -Raw).Trim()
                    title = $Issue.summary
                    branch = $branchName
                    committed = $createdCommit
                }
            }

            $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
            [System.IO.File]::WriteAllText($fakePrPath, $fakePrUrl, $utf8NoBom)
            return @{
                created = $true
                url = $fakePrUrl
                title = $Issue.summary
                branch = $branchName
                committed = $createdCommit
            }
        }

        $existing = Get-ExistingPullRequest -BranchName $branchName -Repo $githubRepo
        if ($existing) {
            return @{
                created = $false
                url = [string]$existing.url
                title = [string]$existing.title
                branch = $branchName
                committed = $createdCommit
            }
        }

        $baseBranch = Get-DefaultBaseBranch -RemoteName $remoteName -ConfiguredBaseBranch $toolConfig.gitBaseBranch
        $prTitle = $Issue.summary
        if (-not $prTitle) { $prTitle = "Issue $($Issue.key)" }

        $bodyLines = @(
            "Generated by opsxj:new from Jira issue $($Issue.key).",
            "",
            "- Jira: $($Issue.url)",
            "- OpenSpec change: openspec/changes/$ChangeName/"
        )
        $prBody = $bodyLines -join "`n"

        try {
            $createArgs = @("pr", "create", "--base", $baseBranch, "--head", $branchName, "--title", $prTitle, "--body", $prBody)
            if (-not [string]::IsNullOrWhiteSpace($githubRepo)) {
                $createArgs += @("--repo", $githubRepo)
            }
            $prUrl = Invoke-Gh -CliArgs $createArgs
        }
        catch {
            throw "Unable to create pull request in GitHub. $($_.Exception.Message)"
        }

        if (-not $prUrl) {
            $createdPr = Get-ExistingPullRequest -BranchName $branchName -Repo $githubRepo
            if ($createdPr) {
                $prUrl = [string]$createdPr.url
            }
        }
        if (-not $prUrl) {
            throw "Pull request was created but URL could not be resolved from GitHub CLI output."
        }

        return @{
            created = $true
            url = $prUrl
            title = $prTitle
            branch = $branchName
            committed = $createdCommit
        }
    }
    finally {
        Pop-Location
    }
}

function Assert-ChangeMergedInGit {
    param(
        [string]$RepoRoot,
        [string]$ChangeName
    )

    Push-Location $RepoRoot
    try {
        $toolConfig = Get-ToolConfig
        $remoteName = $toolConfig.gitRemoteName
        if (-not $remoteName) { $remoteName = "origin" }
        $baseBranch = Get-DefaultBaseBranch -RemoteName $remoteName -ConfiguredBaseBranch $toolConfig.gitBaseBranch

        & git show-ref --verify --quiet "refs/heads/$ChangeName"
        if ($LASTEXITCODE -ne 0) {
            throw "Change branch '$ChangeName' does not exist locally."
        }

        & git show-ref --verify --quiet "refs/heads/$baseBranch"
        if ($LASTEXITCODE -ne 0) {
            throw "Base branch '$baseBranch' does not exist locally."
        }

        & git merge-base --is-ancestor "refs/heads/$ChangeName" "refs/heads/$baseBranch"
        if ($LASTEXITCODE -ne 0) {
            throw "Change branch '$ChangeName' is not merged into '$baseBranch'."
        }

        return $baseBranch
    }
    finally {
        Pop-Location
    }
}

function Get-JiraAuthContext {
    $toolConfig = Get-ToolConfig
    $baseUrl = [string]$toolConfig.jiraBaseUrl
    $email = [string]$toolConfig.jiraEmail
    $token = [string]$toolConfig.jiraApiToken

    if (-not $baseUrl -or -not $email -or -not $token) {
        throw "Set JIRA_BASE_URL, JIRA_EMAIL and JIRA_API_TOKEN to update Jira issue status."
    }

    $baseUrl = $baseUrl.TrimEnd("/")
    $rawAuth = "$email`:$token"
    $authBytes = [System.Text.Encoding]::UTF8.GetBytes($rawAuth)
    $basic = [Convert]::ToBase64String($authBytes)

    return @{
        baseUrl = $baseUrl
        headers = @{
            "Authorization" = "Basic $basic"
            "Accept" = "application/json"
            "Content-Type" = "application/json"
        }
    }
}

function Get-JiraDoneTransition {
    param(
        [array]$Transitions
    )

    $doneNames = @(
        "done", "terminado", "terminada", "hecho", "closed", "cerrado", "cerrada",
        "resolved", "resuelto", "completado", "completada", "finalizado", "finalizada"
    )

    foreach ($t in $Transitions) {
        $toName = [string]$t.to.name
        if (-not $toName) { continue }
        $normalized = $toName.Trim().ToLowerInvariant()
        if ($doneNames -contains $normalized) {
            return $t
        }
    }

    return $null
}

function Set-JiraIssueToDone {
    param([string]$IssueKey)

    $ctx = Get-JiraAuthContext
    $transitionsUri = "$($ctx.baseUrl)/rest/api/3/issue/$IssueKey/transitions"

    try {
        $transitionsResponse = Invoke-RestMethod -Method Get -Uri $transitionsUri -Headers $ctx.headers
    }
    catch {
        throw "Jira transitions query failed for '$IssueKey'. $($_.Exception.Message)"
    }

    $transitions = @($transitionsResponse.transitions)
    if ($transitions.Count -eq 0) {
        throw "No transitions available for Jira issue '$IssueKey'."
    }

    $doneTransition = Get-JiraDoneTransition -Transitions $transitions
    if (-not $doneTransition) {
        $available = $transitions | ForEach-Object { [string]$_.to.name } | Where-Object { $_ } | Sort-Object -Unique
        throw "No Done transition found for '$IssueKey'. Available states: $($available -join ', ')"
    }

    $payload = @{
        transition = @{
            id = [string]$doneTransition.id
        }
    } | ConvertTo-Json -Depth 5

    try {
        Invoke-RestMethod -Method Post -Uri $transitionsUri -Headers $ctx.headers -Body $payload | Out-Null
    }
    catch {
        throw "Jira transition to Done failed for '$IssueKey'. $($_.Exception.Message)"
    }

    return [string]$doneTransition.to.name
}

function Read-AdfNodeText {
    param($Node)
    if ($null -eq $Node) { return "" }
    if ($Node -is [string]) { return $Node }

    $parts = New-Object System.Collections.Generic.List[string]

    if ($Node.text) {
        $parts.Add([string]$Node.text)
    }

    if ($Node.content -is [System.Collections.IEnumerable]) {
        foreach ($child in $Node.content) {
            $childText = Read-AdfNodeText -Node $child
            if ($childText) {
                $parts.Add($childText)
            }
        }
    }

    if ($Node.type -in @("paragraph", "heading", "bulletList", "orderedList", "listItem")) {
        $parts.Add("`n")
    }

    return ($parts -join " ").Trim()
}

function Get-JiraIssueData {
    param([string]$IssueKey)

    $toolConfig = Get-ToolConfig
    $baseUrl = $toolConfig.jiraBaseUrl
    $email = $toolConfig.jiraEmail
    $token = $toolConfig.jiraApiToken

    if (-not $baseUrl -or -not $email -or -not $token) {
        throw "Set JIRA_BASE_URL, JIRA_EMAIL and JIRA_API_TOKEN environment variables."
    }

    $baseUrl = $baseUrl.TrimEnd("/")
    $uri = "$baseUrl/rest/api/3/issue/${IssueKey}?fields=summary,description"

    $rawAuth = "$email`:$token"
    $authBytes = [System.Text.Encoding]::UTF8.GetBytes($rawAuth)
    $basic = [Convert]::ToBase64String($authBytes)

    $headers = @{
        "Authorization" = "Basic $basic"
        "Accept"        = "application/json"
    }

    try {
        $response = Invoke-RestMethod -Method Get -Uri $uri -Headers $headers
    }
    catch {
        $statusCode = ""
        try {
            if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
                $statusCode = [int]$_.Exception.Response.StatusCode
            }
        }
        catch {
            $statusCode = ""
        }
        throw "Jira request failed for '$IssueKey' (status: $statusCode, uri: $uri). $($_.Exception.Message)"
    }

    $summary = [string]$response.fields.summary
    if (-not $summary) { $summary = "Issue $IssueKey" }

    $description = Read-AdfNodeText -Node $response.fields.description
    if (-not $description) { $description = "(No description provided by Jira)" }

    return @{
        key         = $IssueKey
        summary     = $summary.Trim()
        description = $description.Trim()
        url         = "$baseUrl/browse/$IssueKey"
    }
}

function Write-ChangeArtifacts {
    param(
        [string]$RepoRoot,
        [string]$ChangeName,
        [hashtable]$Issue
    )

    $changeRoot = Join-Path $RepoRoot "openspec\\changes\\$ChangeName"
    $proposalPath = Join-Path $changeRoot "proposal.md"
    $designPath = Join-Path $changeRoot "design.md"
    $tasksPath = Join-Path $changeRoot "tasks.md"
    $syncPath = Join-Path $changeRoot "sync.md"

    $capability = "jira-" + (To-KebabCase $Issue.key)
    $specDir = Join-Path $changeRoot ("specs\\" + $capability)
    $specPath = Join-Path $specDir "spec.md"
    New-Item -ItemType Directory -Path $specDir -Force | Out-Null

    $proposal = @(
        "## Why",
        "",
        "Se requiere acelerar la creacion de cambios OpenSpec basados en Jira para reducir trabajo manual y mantener consistencia.",
        "",
        "## What Changes",
        "",
        "- Crear artefactos base del cambio a partir del issue $($Issue.key).",
        "- Usar summary y description de Jira como contexto inicial de propuesta.",
        "- Dejar el cambio listo para refinamiento en design.md, tasks.md y specs/.",
        "",
        "## Capabilities",
        "",
        "### New Capabilities",
        "- ${capability}: Inicio de cambio OpenSpec originado en Jira issue $($Issue.key).",
        "",
        "### Modified Capabilities",
        "- None.",
        "",
        "## Impact",
        "",
        "- Jira issue: $($Issue.url)",
        "- OpenSpec change path: openspec/changes/$ChangeName/"
    ) -join "`n"

    $design = @(
        "## Context",
        "",
        "- Jira issue key: $($Issue.key)",
        "- Jira summary: $($Issue.summary)",
        "- Jira URL: $($Issue.url)",
        "",
        "## Problem Statement",
        "",
        $Issue.description,
        "",
        "## Approach",
        "",
        "- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.",
        "- Definir alcance y no-alcance antes de implementar.",
        "- Validar con openspec.cmd validate $ChangeName."
    ) -join "`n"

    $tasks = @(
        "## 1. Discovery",
        "",
        "- [ ] 1.1 Revisar el issue Jira $($Issue.key) y confirmar alcance.",
        "- [ ] 1.2 Ajustar propuesta con reglas de negocio faltantes.",
        "",
        "## 2. Specs",
        "",
        "- [ ] 2.1 Completar specs/$capability/spec.md con requisitos finales.",
        "- [ ] 2.2 Verificar escenarios testables por requisito.",
        "",
        "## 3. Execution",
        "",
        "- [ ] 3.1 Implementar el cambio en codigo.",
        "- [ ] 3.2 Ejecutar pruebas y documentar evidencia.",
        "- [ ] 3.3 Validar y archivar con OpenSpec."
    ) -join "`n"

    $spec = @(
        "## ADDED Requirements",
        "",
        "### Requirement: Traceability to Jira issue",
        "The system MUST keep traceability between this OpenSpec change and Jira issue $($Issue.key).",
        "",
        "#### Scenario: Change references Jira issue",
        "- **WHEN** a reviewer opens the change artifacts",
        "- **THEN** proposal and design identify Jira issue $($Issue.key)"
    ) -join "`n"

    $templatePath = Join-Path $RepoRoot "openspec\\context\\REPO_IMPACT_TEMPLATE.md"
    if (Test-Path $templatePath) {
        $sync = Get-Content -Path $templatePath -Raw
        $sync = $sync.Replace("ABC-123", $Issue.key)
        $sync = $sync.Replace("<short summary>", $Issue.summary)
        $sync = $sync.Replace("<change-name>", $ChangeName)
    }
    else {
        $sync = @(
            "## Repo Impact Plan",
            "",
            "Ticket: `$($Issue.key)`  ",
            "Summary: `$($Issue.summary)`  ",
            "Change: `openspec/changes/$ChangeName/`",
            "",
            "## Impact Matrix",
            "",
            "| Repo | Impacta? | Motivo | opsxj:new | PR | opsxj:archive | Estado |",
            "|---|---|---|---|---|---|---|",
            "| `DocuArchi.Api` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "| `DocuArchiCore` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "| `DocuArchiCore.Abstractions` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "| `DocuArchiCore.Web` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "| `MiApp.DTOs` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "| `MiApp.Services` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "| `MiApp.Repository` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "| `MiApp.Models` | `pending` | `pending` | `pending` | `pending` | `pending` | `todo` |",
            "",
            "## Rule",
            "",
            "- Mark impacted repos first (`Impacta?` = `yes/no`).",
            "- Run `opsxj:new $($Issue.key)` only in repos marked `yes`.",
            "- Run `opsxj:archive $($Issue.key)` only after PR is merged in that repo."
        ) -join "`n"
    }

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($proposalPath, $proposal, $utf8NoBom)
    [System.IO.File]::WriteAllText($designPath, $design, $utf8NoBom)
    [System.IO.File]::WriteAllText($tasksPath, $tasks, $utf8NoBom)
    [System.IO.File]::WriteAllText($specPath, $spec, $utf8NoBom)
    [System.IO.File]::WriteAllText($syncPath, $sync, $utf8NoBom)
}

function Resolve-ChangeNameFromIssueKey {
    param(
        [string]$RepoRoot,
        [string]$IssueKey
    )
    $changesRoot = Join-Path $RepoRoot "openspec\\changes"
    $prefix = (To-KebabCase $IssueKey) + "-"
    $matches = Get-ChildItem -Path $changesRoot -Directory -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -like "$prefix*" }

    if ($matches.Count -eq 0) {
        throw "No change found for issue key '$IssueKey'. Expected prefix '$prefix'."
    }
    if ($matches.Count -gt 1) {
        $names = $matches | ForEach-Object { $_.Name } | Sort-Object
        throw "Multiple changes found for '$IssueKey': $($names -join ', '). Use explicit change name."
    }

    return $matches[0].Name
}

function Invoke-New {
    param(
        [string]$RepoRoot,
        [string]$IssueKey,
        [switch]$SkipJira
    )

    if ($IssueKey -notmatch "^[A-Za-z]+-\d+$") {
        throw "Issue key must match format ABC-123."
    }

    if ($SkipJira) {
        $issue = @{
            key         = $IssueKey.ToUpperInvariant()
            summary     = "Issue $IssueKey"
            description = "Jira lookup skipped. Complete this content manually."
            url         = ""
        }
    }
    else {
        $issue = Get-JiraIssueData -IssueKey $IssueKey.ToUpperInvariant()
    }

    $summarySlug = To-KebabCase $issue.summary
    if (-not $summarySlug) { $summarySlug = "change" }
    if ($summarySlug.Length -gt 40) { $summarySlug = $summarySlug.Substring(0, 40).Trim("-") }

    $changeName = ((To-KebabCase $issue.key) + "-" + $summarySlug).Trim("-")
    $changeRoot = Join-Path $RepoRoot "openspec\\changes\\$changeName"

    if (-not (Test-Path $changeRoot)) {
        New-Item -ItemType Directory -Path $changeRoot -Force | Out-Null
        $metaPath = Join-Path $changeRoot ".openspec.yaml"
        if (-not (Test-Path $metaPath)) {
            $meta = @(
                "schema: spec-driven",
                "created: $(Get-Date -Format 'yyyy-MM-dd')"
            ) -join "`n"
            $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
            [System.IO.File]::WriteAllText($metaPath, $meta, $utf8NoBom)
        }
    }

    Write-ChangeArtifacts -RepoRoot $RepoRoot -ChangeName $changeName -Issue $issue
    Invoke-OpenSpec -RepoRoot $RepoRoot -CliArgs @("validate", $changeName)

    try {
        $pr = Ensure-GitHubPullRequest -RepoRoot $RepoRoot -ChangeName $changeName -Issue $issue
    }
    catch {
        throw "OpenSpec artifacts were created, but GitHub PR creation failed. $($_.Exception.Message)"
    }

    Write-Output "Created/updated OpenSpec change: $changeName"
    Write-Output "Path: openspec/changes/$changeName"
    Write-Output "Branch: $($pr.branch)"
    if ($pr.created) {
        Write-Output "Pull request created: $($pr.url)"
    }
    else {
        Write-Output "Pull request already exists: $($pr.url)"
    }
}

function Invoke-Archive {
    param(
        [string]$RepoRoot,
        [string]$IssueOrChange,
        [switch]$Yes,
        [switch]$SkipSpecs,
        [switch]$NoValidate,
        [switch]$SkipJira
    )

    $changeName = $IssueOrChange
    if ($IssueOrChange -match "^[A-Za-z]+-\d+$") {
        $changeName = Resolve-ChangeNameFromIssueKey -RepoRoot $RepoRoot -IssueKey $IssueOrChange.ToUpperInvariant()
    }

    if (-not $NoValidate) {
        $baseBranch = Assert-ChangeMergedInGit -RepoRoot $RepoRoot -ChangeName $changeName
        Write-Output "Git merge validation passed: '$changeName' merged into '$baseBranch'."
    }

    $args = @("archive", $changeName)
    if ($Yes) { $args += "-y" }
    if ($SkipSpecs) { $args += "--skip-specs" }
    if ($NoValidate) { $args += "--no-validate" }

    Invoke-OpenSpec -RepoRoot $RepoRoot -CliArgs $args
    Write-Output "Archived change: $changeName"

    $issueKey = if ($IssueOrChange -match "^[A-Za-z]+-\d+$") { $IssueOrChange.ToUpperInvariant() } else { Get-IssueKeyFromChangeName -ChangeName $changeName }

    if ($SkipJira) {
        Write-Output "Skipped Jira transition by flag."
        return
    }

    if (-not $issueKey) {
        throw "Archive completed but Jira issue key could not be resolved from '$IssueOrChange'."
    }

    try {
        $doneState = Set-JiraIssueToDone -IssueKey $issueKey
    }
    catch {
        throw "Archive completed but Jira status update failed for '$issueKey'. $($_.Exception.Message)"
    }

    Write-Output "Jira issue transitioned: $issueKey -> $doneState"
}

$repoRoot = Get-RepoRoot

if ($Command -eq "new") {
    Invoke-New -RepoRoot $repoRoot -IssueKey $IssueOrChange -SkipJira:$SkipJira
}
elseif ($Command -eq "archive") {
    Invoke-Archive -RepoRoot $repoRoot -IssueOrChange $IssueOrChange -Yes:$Yes -SkipSpecs:$SkipSpecs -NoValidate:$NoValidate -SkipJira:$SkipJira
}
