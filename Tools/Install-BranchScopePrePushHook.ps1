param(
    [Parameter(Mandatory = $true)]
    [string]$IssueKey,

    [string]$AllowedPathsFile = "",

    [switch]$Force
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$gitDir = Join-Path $repoRoot ".git"
$hooksDir = Join-Path $gitDir "hooks"
$hookPath = Join-Path $hooksDir "pre-push"

if (-not (Test-Path $gitDir)) {
    throw "No se encontro .git en $repoRoot"
}

New-Item -ItemType Directory -Path $hooksDir -Force | Out-Null

$allowedArg = ""
if (-not [string]::IsNullOrWhiteSpace($AllowedPathsFile)) {
    $allowedArg = " -AllowedPathsFile `"$AllowedPathsFile`""
}

$hookScript = @"
#!/bin/sh
set -e
powershell -NoProfile -ExecutionPolicy Bypass -File "Tools/Check-BranchScope.ps1" -IssueKey "$IssueKey"$allowedArg
"@

if ((Test-Path $hookPath) -and -not $Force) {
    throw "El hook pre-push ya existe en $hookPath. Use -Force para sobrescribir."
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($hookPath, $hookScript, $utf8NoBom)

Write-Output "Hook pre-push instalado en: $hookPath"
Write-Output "IssueKey configurado: $IssueKey"
if (-not [string]::IsNullOrWhiteSpace($AllowedPathsFile)) {
    Write-Output "AllowedPathsFile configurado: $AllowedPathsFile"
}

