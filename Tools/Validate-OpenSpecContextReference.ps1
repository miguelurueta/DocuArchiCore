param(
    [Parameter(Mandatory = $true)]
    [string]$ChangeName,
    [string]$OpenSpecRoot
)

$ErrorActionPreference = "Stop"

if (-not $OpenSpecRoot) {
    $scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
    $OpenSpecRoot = (Resolve-Path (Join-Path $scriptRoot "..\\openspec")).Path
}

$changeRoot = Join-Path $OpenSpecRoot ("changes\\" + $ChangeName)
$proposalPath = Join-Path $changeRoot "proposal.md"
$designPath = Join-Path $changeRoot "design.md"
$contextPath = Join-Path $OpenSpecRoot "context\\multi-repo-context.md"
$requiredRef = "openspec/context/multi-repo-context.md"

if (-not (Test-Path $changeRoot)) {
    throw "Change not found: $changeRoot"
}

if (-not (Test-Path $proposalPath)) {
    throw "Missing proposal.md in change: $proposalPath"
}

if (-not (Test-Path $designPath)) {
    throw "Missing design.md in change: $designPath"
}

if (-not (Test-Path $contextPath)) {
    throw "Missing context file: $contextPath"
}

$proposal = Get-Content -Path $proposalPath -Raw
$design = Get-Content -Path $designPath -Raw

$proposalHasRef = $proposal.Contains($requiredRef)
$designHasRef = $design.Contains($requiredRef)
$designHasContextSection = $design -match "(?m)^##\s+Context Reference\s*$"

$errors = New-Object System.Collections.Generic.List[string]
if (-not $proposalHasRef) {
    $errors.Add("proposal.md does not reference '$requiredRef'.")
}
if (-not $designHasRef) {
    $errors.Add("design.md does not reference '$requiredRef'.")
}
if (-not $designHasContextSection) {
    $errors.Add("design.md is missing '## Context Reference' section.")
}

if ($errors.Count -gt 0) {
    Write-Output "FAILED: Context-reference checks"
    foreach ($e in $errors) {
        Write-Output ("- " + $e)
    }
    exit 1
}

Write-Output "OK: Context-reference checks passed for change '$ChangeName'."
Write-Output "- proposal.md references context"
Write-Output "- design.md references context"
Write-Output "- design.md includes '## Context Reference'"
