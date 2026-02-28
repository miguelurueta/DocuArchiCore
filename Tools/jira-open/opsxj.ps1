param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet("new", "archive")]
    [string]$Command,

    [Parameter(Mandatory = $true, Position = 1)]
    [string]$IssueOrChange,

    [switch]$SkipJira,
    [switch]$SelectRepos,
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

function Get-ImpactRepoCatalog {
    return @(
        "DocuArchi.Api",
        "DocuArchiCore",
        "DocuArchiCore.Abstractions",
        "DocuArchiCore.Web",
        "MiApp.DTOs",
        "MiApp.Services",
        "MiApp.Repository",
        "MiApp.Models"
    )
}

function Prompt-SelectImpactRepos {
    param(
        [string]$IssueKey,
        [string[]]$RepoCatalog
    )

    Write-Output "Select impacted repositories for $IssueKey."
    Write-Output "Use comma-separated numbers (example: 1,3,6). Type 'all' for all repositories."
    for ($i = 0; $i -lt $RepoCatalog.Count; $i++) {
        Write-Output ("[{0}] {1}" -f ($i + 1), $RepoCatalog[$i])
    }

    $inputValue = Read-Host "Impacted repos"
    if ([string]::IsNullOrWhiteSpace($inputValue)) {
        return @()
    }

    $trimmed = $inputValue.Trim()
    if ($trimmed.ToLowerInvariant() -eq "all") {
        return $RepoCatalog
    }

    $selected = New-Object System.Collections.Generic.List[string]
    $tokens = $trimmed -split ","
    foreach ($token in $tokens) {
        $candidate = $token.Trim()
        if (-not $candidate) { continue }
        if ($candidate -notmatch "^\d+$") {
            throw "Invalid selection '$candidate'. Use only numbers separated by commas, or 'all'."
        }
        $index = [int]$candidate
        if ($index -lt 1 -or $index -gt $RepoCatalog.Count) {
            throw "Selection '$candidate' is out of range (1-$($RepoCatalog.Count))."
        }
        $repoName = $RepoCatalog[$index - 1]
        if (-not $selected.Contains($repoName)) {
            $selected.Add($repoName)
        }
    }

    return @($selected)
}

function Apply-ImpactSelectionToSync {
    param(
        [string]$SyncText,
        [string[]]$SelectedRepos
    )

    if ($null -eq $SelectedRepos) {
        return $SyncText
    }

    $selectedSet = @{}
    foreach ($repo in $SelectedRepos) {
        if (-not [string]::IsNullOrWhiteSpace($repo)) {
            $selectedSet[$repo.ToLowerInvariant()] = $true
        }
    }

    $lines = $SyncText -split "`r?`n"
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        if ($line -match '^\|\s*(?<repo>[^|]+)\|') {
            $repo = ([string]$Matches["repo"]).Trim().Replace([string][char]96, "")
            $isImpacted = $selectedSet.ContainsKey($repo.ToLowerInvariant())
            $impact = if ($isImpacted) { "yes" } else { "no" }
            $motivo = if ($isImpacted) { "<definir alcance>" } else { "fuera de alcance" }
            $opsNew = if ($isImpacted) { "pending" } else { "n/a" }
            $pr = if ($isImpacted) { "pending" } else { "n/a" }
            $opsArchive = if ($isImpacted) { "pending" } else { "n/a" }
            $status = if ($isImpacted) { "todo" } else { "n_a" }
            $lines[$i] = "| `$($repo)` | `$($impact)` | `$($motivo)` | `$($opsNew)` | `$($pr)` | `$($opsArchive)` | `$($status)` |"
        }
    }

    return ($lines -join "`n")
}

function Write-OpsxjLog {
    param(
        [string]$RepoRoot,
        [string]$IssueKey,
        [string]$Step,
        [string]$Status,
        [string]$Message,
        [hashtable]$Data
    )

    if ([string]::IsNullOrWhiteSpace($RepoRoot)) { return }
    if ([string]::IsNullOrWhiteSpace($IssueKey)) { $IssueKey = "unknown" }
    if ([string]::IsNullOrWhiteSpace($Step)) { $Step = "unknown" }
    if ([string]::IsNullOrWhiteSpace($Status)) { $Status = "unknown" }
    if ($null -eq $Data) { $Data = @{} }

    $logsDir = Join-Path $RepoRoot "openspec\\logs"
    New-Item -ItemType Directory -Path $logsDir -Force | Out-Null
    $logPath = Join-Path $logsDir "$($IssueKey.ToUpperInvariant()).log.jsonl"

    $entry = @{
        timestampUtc = (Get-Date).ToUniversalTime().ToString("o")
        issueKey = $IssueKey.ToUpperInvariant()
        step = $Step
        status = $Status
        message = $Message
        data = $Data
    } | ConvertTo-Json -Depth 8 -Compress

    Add-Content -Path $logPath -Value $entry
}

function Get-DetectedImpactRepos {
    param(
        [hashtable]$Issue,
        [string[]]$RepoCatalog
    )

    $text = ("{0} {1}" -f ([string]$Issue.summary), ([string]$Issue.description)).ToLowerInvariant()
    $keywords = @{
        "DocuArchi.Api" = @("api", "controller", "endpoint", "swagger", "program.cs")
        "DocuArchiCore" = @("openspec", "orquestador", "orchestrator", "sync.md", "contexto", "jira")
        "MiApp.DTOs" = @("dto", "dtos", "contract", "contrato", "request", "response")
        "MiApp.Models" = @("model", "modelo", "dapper", "tabla", "entity", "mapeo")
        "MiApp.Repository" = @("repository", "repositorio", "query", "consulta", "sql", "mysql", "insert", "update", "delete")
        "MiApp.Services" = @("service", "servicio", "automapper", "mapping", "business logic")
        "DocuArchiCore.Abstractions" = @("abstraction", "abstraccion", "interface contract", "shared interface")
        "DocuArchiCore.Web" = @("web", "frontend", "ui", "razor", "view")
    }

    $selected = New-Object System.Collections.Generic.List[string]
    foreach ($repo in $RepoCatalog) {
        $repoWords = $keywords[$repo]
        if ($null -eq $repoWords) { continue }
        foreach ($kw in $repoWords) {
            if ($text.Contains($kw.ToLowerInvariant())) {
                if (-not $selected.Contains($repo)) {
                    $selected.Add($repo)
                }
                break
            }
        }
    }

    $confidence = if ($selected.Count -gt 0) { "high" } else { "low" }
    return @{
        repos = @($selected)
        confidence = $confidence
    }
}

function Resolve-ImpactReposForIssue {
    param(
        [hashtable]$Issue,
        [string[]]$RepoCatalog,
        [switch]$SelectRepos
    )

    if ($SelectRepos) {
        return @(Prompt-SelectImpactRepos -IssueKey $Issue.key -RepoCatalog $RepoCatalog)
    }

    $detected = Get-DetectedImpactRepos -Issue $Issue -RepoCatalog $RepoCatalog
    $repos = @($detected.repos)
    if ($repos.Count -gt 0) {
        Write-Output ("Auto-detected impacted repos: {0}" -f ($repos -join ", "))
        return $repos
    }

    Write-Output "No se pudo detectar automaticamente el repositorio. Se requiere seleccion manual."
    $manual = @(Prompt-SelectImpactRepos -IssueKey $Issue.key -RepoCatalog $RepoCatalog)
    if ($manual.Count -eq 0) {
        throw "No se pudo detectar el repositorio, especifique la plantilla de cambios."
    }

    return $manual
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
    if (-not $summary) { $summary = "" }

    $description = Read-AdfNodeText -Node $response.fields.description
    if (-not $description) { $description = "" }

    return @{
        key         = $IssueKey
        summary     = $summary.Trim()
        description = $description.Trim()
        url         = "$baseUrl/browse/$IssueKey"
    }
}

function Assert-IssueHasText {
    param([hashtable]$Issue)

    $summary = [string]$Issue.summary
    $description = [string]$Issue.description
    if ([string]::IsNullOrWhiteSpace($summary) -and [string]::IsNullOrWhiteSpace($description)) {
        throw "El ticket no tiene texto, no puede archivarse."
    }
}

function Get-SyncImpactEntries {
    param(
        [string]$RepoRoot,
        [string]$ChangeName
    )

    $syncPath = Join-Path $RepoRoot "openspec\\changes\\$ChangeName\\sync.md"
    if (-not (Test-Path $syncPath)) {
        return @()
    }

    $entries = New-Object System.Collections.ArrayList
    $lines = Get-Content -Path $syncPath
    foreach ($line in $lines) {
        if ($line -match '^\|\s*(?<repo>[^|]+)\|\s*(?<impact>[^|]+)\|\s*(?<motivo>[^|]*)\|\s*(?<opsnew>[^|]*)\|\s*(?<pr>[^|]*)\|\s*(?<opsarchive>[^|]*)\|\s*(?<status>[^|]*)\|') {
            $repo = ([string]$Matches["repo"]).Trim().Replace([string][char]96, "")
            if ($repo -eq "Repo") { continue }
            [void]$entries.Add([pscustomobject]@{
                    repo = $repo.Trim()
                    impact = ([string]$Matches["impact"]).Trim().Replace([string][char]96, "").ToLowerInvariant()
                    pr = ([string]$Matches["pr"]).Trim().Replace([string][char]96, "")
                    status = ([string]$Matches["status"]).Trim().Replace([string][char]96, "")
                })
        }
    }

    return @($entries.ToArray())
}

function Get-PullRequestMergeStatus {
    param(
        [string]$PrReference,
        [string]$Repo
    )

    if ([string]::IsNullOrWhiteSpace($PrReference)) {
        return @{
            merged = $false
            state = "missing"
            url = ""
        }
    }

    $trimmed = $PrReference.Trim()
    if ($trimmed -in @("pending", "n/a", "na", "-", "")) {
        return @{
            merged = $false
            state = $trimmed
            url = $trimmed
        }
    }

    $args = @("pr", "view", $trimmed, "--json", "state,mergedAt,url")
    if (-not [string]::IsNullOrWhiteSpace($Repo)) {
        $args += @("--repo", $Repo)
    }
    $json = Invoke-Gh -CliArgs $args
    $obj = $json | ConvertFrom-Json
    $isMerged = (-not [string]::IsNullOrWhiteSpace([string]$obj.mergedAt)) -or ([string]$obj.state -eq "MERGED")
    return @{
        merged = [bool]$isMerged
        state = [string]$obj.state
        url = [string]$obj.url
    }
}

function Assert-AllImpactedPullRequestsMerged {
    param(
        [string]$RepoRoot,
        [string]$ChangeName
    )

    $entries = @(Get-SyncImpactEntries -RepoRoot $RepoRoot -ChangeName $ChangeName)
    if ($entries.Count -eq 0) {
        return @()
    }

    $pending = New-Object System.Collections.Generic.List[string]
    foreach ($entry in $entries) {
        if ($entry.impact -ne "yes") { continue }
        $status = Get-PullRequestMergeStatus -PrReference $entry.pr -Repo ""
        if (-not $status.merged) {
            $pending.Add("$($entry.repo): $($entry.pr) [$($status.state)]")
        }
    }

    if ($pending.Count -gt 0) {
        throw "No se puede archivar: existen PRs sin merge. $($pending -join '; ')"
    }

    return $entries
}

function Push-OrchestratorArchive {
    param(
        [string]$RepoRoot,
        [string]$ChangeName
    )

    $repoName = Split-Path $RepoRoot -Leaf
    if ($repoName -ne "DocuArchiCore") {
        return @{
            pushed = $false
            reason = "not-orchestrator"
        }
    }

    Push-Location $RepoRoot
    try {
        Invoke-Git -CliArgs @("add", "--", "openspec/changes") | Out-Null
        $staged = Invoke-Git -CliArgs @("diff", "--cached", "--name-only", "--", "openspec/changes")
        if ([string]::IsNullOrWhiteSpace($staged)) {
            return @{
                pushed = $false
                reason = "no-changes"
            }
        }

        Invoke-Git -CliArgs @("commit", "-m", "opsxj: archive $ChangeName") | Out-Null
        $branch = Invoke-Git -CliArgs @("rev-parse", "--abbrev-ref", "HEAD")
        $toolConfig = Get-ToolConfig
        $remoteName = $toolConfig.gitRemoteName
        if ([string]::IsNullOrWhiteSpace($remoteName)) { $remoteName = "origin" }
        Invoke-Git -CliArgs @("push", $remoteName, $branch) | Out-Null
        return @{
            pushed = $true
            branch = $branch
            remote = $remoteName
        }
    }
    finally {
        Pop-Location
    }
}

function Write-ChangeArtifacts {
    param(
        [string]$RepoRoot,
        [string]$ChangeName,
        [hashtable]$Issue,
        [string[]]$SelectedRepos
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
        "- OpenSpec change path: openspec/changes/$ChangeName/",
        "- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md"
    ) -join "`n"

    $design = @(
        "## Context",
        "",
        "- Jira issue key: $($Issue.key)",
        "- Jira summary: $($Issue.summary)",
        "- Jira URL: $($Issue.url)",
        "",
        "## Context Reference",
        "",
        "- openspec/context/multi-repo-context.md",
        "- openspec/context/OPSXJ_BACKEND_RULES.md",
        "",
        "## Problem Statement",
        "",
        $Issue.description,
        "",
        "## Approach",
        "",
        "- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.",
        "- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.",
        "- Definir alcance y no-alcance antes de implementar.",
        "- Validar con openspec.cmd validate $ChangeName."
    ) -join "`n"

    $tasks = @(
        "## 1. Discovery",
        "",
        "- [ ] 1.1 Revisar el issue Jira $($Issue.key) y confirmar alcance.",
        "- [ ] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.",
        "- [ ] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo.",
        "",
        "## 2. Specs",
        "",
        "- [ ] 2.1 Completar specs/$capability/spec.md con requisitos finales.",
        "- [ ] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.",
        "- [ ] 2.3 Verificar escenarios testables por requisito.",
        "",
        "## 3. Application",
        "",
        "- [ ] 3.1 Aplicar patron ApiController + Service + AutoMapper + Repository con AppResponses y try/catch.",
        "- [ ] 3.2 Registrar interfaces en Program.cs (Services L / Repositories R).",
        "- [ ] 3.3 Integrar cambios de aplicacion y verificar compilacion local.",
        "",
        "## 4. Test",
        "",
        "- [ ] 4.1 Implementar Unit/Integration/Contract tests y documentar evidencia.",
        "- [ ] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible).",
        "- [ ] 4.3 Validar y archivar con OpenSpec."
    ) -join "`n"

    $spec = @(
        "## ADDED Requirements",
        "",
        "### Requirement: Traceability to Jira issue",
        "The system MUST keep traceability between this OpenSpec change and Jira issue $($Issue.key).",
        "",
        "#### Scenario: Change references Jira issue",
        "- **WHEN** a reviewer opens the change artifacts",
        "- **THEN** proposal and design identify Jira issue $($Issue.key)",
        "",
        "### Requirement: Backend update rules baseline",
        "Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.",
        "",
        "#### Scenario: Missing implementation constraints",
        "- **WHEN** proposal/design/tasks are reviewed",
        "- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements"
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
    $sync = Apply-ImpactSelectionToSync -SyncText $sync -SelectedRepos $SelectedRepos

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
        [switch]$SkipJira,
        [switch]$SelectRepos
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
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issue.key -Step "jira_issue_loaded" -Status "ok" -Message "Jira issue loaded for new." -Data @{ skipJira = [bool]$SkipJira }

    $summarySlug = To-KebabCase $issue.summary
    if (-not $summarySlug) { $summarySlug = "change" }
    if ($summarySlug.Length -gt 40) { $summarySlug = $summarySlug.Substring(0, 40).Trim("-") }

    $changeName = ((To-KebabCase $issue.key) + "-" + $summarySlug).Trim("-")
    $changeRoot = Join-Path $RepoRoot "openspec\\changes\\$changeName"
    $selectedRepos = Resolve-ImpactReposForIssue -Issue $issue -RepoCatalog (Get-ImpactRepoCatalog) -SelectRepos:$SelectRepos
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issue.key -Step "repo_detection" -Status "ok" -Message "Impacted repositories resolved." -Data @{ repos = @($selectedRepos) }

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

    Write-ChangeArtifacts -RepoRoot $RepoRoot -ChangeName $changeName -Issue $issue -SelectedRepos $selectedRepos
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issue.key -Step "openspec_artifacts" -Status "ok" -Message "OpenSpec artifacts generated." -Data @{ change = $changeName }
    Invoke-OpenSpec -RepoRoot $RepoRoot -CliArgs @("validate", $changeName)
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issue.key -Step "openspec_validate" -Status "ok" -Message "OpenSpec validation passed." -Data @{ change = $changeName }

    try {
        $pr = Ensure-GitHubPullRequest -RepoRoot $RepoRoot -ChangeName $changeName -Issue $issue
    }
    catch {
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issue.key -Step "pr_create" -Status "error" -Message "GitHub PR creation failed." -Data @{ error = $_.Exception.Message; change = $changeName }
        throw "OpenSpec artifacts were created, but GitHub PR creation failed. $($_.Exception.Message)"
    }
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issue.key -Step "pr_create" -Status "ok" -Message "PR ensured for change." -Data @{ url = $pr.url; branch = $pr.branch; created = [bool]$pr.created }

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

    $issueKey = if ($IssueOrChange -match "^[A-Za-z]+-\d+$") { $IssueOrChange.ToUpperInvariant() } else { $null }
    $changeName = $IssueOrChange
    if ($IssueOrChange -match "^[A-Za-z]+-\d+$") {
        $changeName = Resolve-ChangeNameFromIssueKey -RepoRoot $RepoRoot -IssueKey $IssueOrChange.ToUpperInvariant()
    }
    if (-not $issueKey) {
        $issueKey = Get-IssueKeyFromChangeName -ChangeName $changeName
    }
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "archive_start" -Status "ok" -Message "Archive flow started." -Data @{ change = $changeName; noValidate = [bool]$NoValidate; skipJira = [bool]$SkipJira }

    if (-not $NoValidate) {
        $baseBranch = Assert-ChangeMergedInGit -RepoRoot $RepoRoot -ChangeName $changeName
        Write-Output "Git merge validation passed: '$changeName' merged into '$baseBranch'."
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "git_merge_validation" -Status "ok" -Message "Local git merge validation passed." -Data @{ change = $changeName; baseBranch = $baseBranch }
    }

    try {
        $impactEntries = Assert-AllImpactedPullRequestsMerged -RepoRoot $RepoRoot -ChangeName $changeName
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "pr_merge_validation" -Status "ok" -Message "Impacted PRs merged validation passed." -Data @{ change = $changeName; entries = @($impactEntries) }
    }
    catch {
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "pr_merge_validation" -Status "error" -Message "Impacted PR merge validation failed." -Data @{ change = $changeName; error = $_.Exception.Message }
        throw
    }

    $args = @("archive", $changeName)
    if ($Yes) { $args += "-y" }
    if ($SkipSpecs) { $args += "--skip-specs" }
    if ($NoValidate) { $args += "--no-validate" }

    Invoke-OpenSpec -RepoRoot $RepoRoot -CliArgs $args
    Write-Output "Archived change: $changeName"
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "archive_local" -Status "ok" -Message "OpenSpec archive completed." -Data @{ change = $changeName }

    $pushResult = Push-OrchestratorArchive -RepoRoot $RepoRoot -ChangeName $changeName
    if ($pushResult.pushed) {
        Write-Output "Orchestrator push completed: $($pushResult.remote)/$($pushResult.branch)"
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "orchestrator_push" -Status "ok" -Message "Orchestrator archive push completed." -Data $pushResult
    }
    else {
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "orchestrator_push" -Status "skipped" -Message "Orchestrator push skipped." -Data $pushResult
    }

    if ($SkipJira) {
        Write-Output "Skipped Jira transition by flag."
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "jira_transition" -Status "skipped" -Message "Jira transition skipped by flag." -Data @{}
        return
    }

    if (-not $issueKey) {
        throw "Archive completed but Jira issue key could not be resolved from '$IssueOrChange'."
    }

    $issue = Get-JiraIssueData -IssueKey $issueKey
    Assert-IssueHasText -Issue $issue
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "jira_text_validation" -Status "ok" -Message "Jira ticket text validation passed." -Data @{}

    try {
        $doneState = Set-JiraIssueToDone -IssueKey $issueKey
    }
    catch {
        Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "jira_transition" -Status "error" -Message "Jira transition failed." -Data @{ error = $_.Exception.Message }
        throw "Archive completed but Jira status update failed for '$issueKey'. $($_.Exception.Message)"
    }

    Write-Output "Jira issue transitioned: $issueKey -> $doneState"
    Write-OpsxjLog -RepoRoot $RepoRoot -IssueKey $issueKey -Step "jira_transition" -Status "ok" -Message "Jira issue transitioned." -Data @{ state = $doneState }
}

$repoRoot = Get-RepoRoot

if ($Command -eq "new") {
    Invoke-New -RepoRoot $repoRoot -IssueKey $IssueOrChange -SkipJira:$SkipJira -SelectRepos:$SelectRepos
}
elseif ($Command -eq "archive") {
    Invoke-Archive -RepoRoot $repoRoot -IssueOrChange $IssueOrChange -Yes:$Yes -SkipSpecs:$SkipSpecs -NoValidate:$NoValidate -SkipJira:$SkipJira
}
