param(
    [Parameter(Mandatory = $true)]
    [string]$IssueKey,

    [string]$BaseRef = "origin/main",

    [string]$AllowedPathsFile = "",

    [switch]$SkipBranchNameCheck,

    [switch]$SkipForeignIssueCheck
)

$ErrorActionPreference = "Stop"

function Normalize-PathForGit {
    param([string]$Value)
    return ($Value -replace "\\", "/").Trim()
}

function Read-AllowedPrefixes {
    param([string]$FilePath)

    if ([string]::IsNullOrWhiteSpace($FilePath)) {
        return @()
    }

    if (-not (Test-Path $FilePath)) {
        throw "AllowedPathsFile no existe: $FilePath"
    }

    $rawLines = Get-Content -Path $FilePath
    $prefixes = @()
    foreach ($line in $rawLines) {
        $trimmed = $line.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmed)) { continue }
        if ($trimmed.StartsWith("#")) { continue }
        $prefixes += (Normalize-PathForGit $trimmed).TrimEnd("/")
    }

    return $prefixes | Select-Object -Unique
}

function Match-AnyPrefix {
    param(
        [string]$Path,
        [string[]]$Prefixes
    )

    if ($Prefixes.Count -eq 0) { return $true }
    foreach ($prefix in $Prefixes) {
        if ($Path -eq $prefix -or $Path.StartsWith($prefix + "/")) {
            return $true
        }
    }
    return $false
}

$issueUpper = $IssueKey.Trim().ToUpperInvariant()
if ($issueUpper -notmatch "^[A-Z]+-\d+$") {
    throw "IssueKey invalido: $IssueKey"
}

$branch = (& git rev-parse --abbrev-ref HEAD).Trim()
if ($LASTEXITCODE -ne 0) {
    throw "No se pudo determinar branch actual."
}

$mergeBase = (& git merge-base HEAD $BaseRef).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($mergeBase)) {
    throw "No se pudo calcular merge-base con '$BaseRef'. Ejecuta git fetch origin y reintenta."
}

$changedRaw = (& git diff --name-only "$BaseRef...HEAD")
$changedFiles = @()
if (-not [string]::IsNullOrWhiteSpace($changedRaw)) {
    $changedFiles = @($changedRaw -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | ForEach-Object { Normalize-PathForGit $_ })
}

$allowedPrefixes = Read-AllowedPrefixes -FilePath $AllowedPathsFile
$outOfScope = @()
foreach ($file in $changedFiles) {
    if (-not (Match-AnyPrefix -Path $file -Prefixes $allowedPrefixes)) {
        $outOfScope += $file
    }
}

$subjectsRaw = (& git log --format=%s "$BaseRef..HEAD")
$subjects = @()
if (-not [string]::IsNullOrWhiteSpace($subjectsRaw)) {
    $subjects = @($subjectsRaw -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

$issueRegex = [regex]"\b[A-Z]+-\d+\b"
$foreignIssueKeys = New-Object System.Collections.Generic.HashSet[string]
foreach ($subject in $subjects) {
    $matches = $issueRegex.Matches($subject.ToUpperInvariant())
    foreach ($match in $matches) {
        $value = $match.Value
        if ($value -ne $issueUpper) {
            [void]$foreignIssueKeys.Add($value)
        }
    }
}

$errors = New-Object System.Collections.Generic.List[string]

if (-not $SkipBranchNameCheck) {
    $branchLower = $branch.ToLowerInvariant()
    $expectedPrefix = $issueUpper.ToLowerInvariant() + "-"
    if (-not $branchLower.StartsWith($expectedPrefix)) {
        $errors.Add("Branch actual '$branch' no inicia con '$expectedPrefix'.")
    }
}

if (-not $SkipForeignIssueCheck -and $foreignIssueKeys.Count -gt 0) {
    $errors.Add("Commits contienen otros issue keys: $($foreignIssueKeys -join ', ').")
}

if ($outOfScope.Count -gt 0) {
    $errors.Add("Hay archivos fuera del alcance permitido (AllowedPathsFile).")
}

Write-Output "=== Scope Check Report ==="
Write-Output "IssueKey: $issueUpper"
Write-Output "Branch: $branch"
Write-Output "BaseRef: $BaseRef"
Write-Output "MergeBase: $mergeBase"
Write-Output "ChangedFiles: $($changedFiles.Count)"
Write-Output "Commits: $($subjects.Count)"
if ($allowedPrefixes.Count -gt 0) {
    Write-Output "AllowedPrefixes: $($allowedPrefixes -join ', ')"
}
if ($foreignIssueKeys.Count -gt 0) {
    Write-Output "ForeignIssueKeys: $($foreignIssueKeys -join ', ')"
}
if ($outOfScope.Count -gt 0) {
    Write-Output "OutOfScopeFiles:"
    $outOfScope | ForEach-Object { Write-Output " - $_" }
}

if ($errors.Count -gt 0) {
    Write-Output ""
    Write-Output "Scope check FAILED:"
    $errors | ForEach-Object { Write-Output " - $_" }
    exit 1
}

Write-Output "Scope check PASSED."
exit 0

