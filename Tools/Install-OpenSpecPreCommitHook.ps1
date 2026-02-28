param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$gitDir = Join-Path $repoRoot ".git"
$hooksDir = Join-Path $gitDir "hooks"
$hookPath = Join-Path $hooksDir "pre-commit"

if (-not (Test-Path $gitDir)) {
    throw "No .git directory found at $gitDir"
}

New-Item -ItemType Directory -Path $hooksDir -Force | Out-Null

$hookScript = @'
#!/bin/sh
set -e

STAGED_FILES=$(git diff --cached --name-only)

# No OpenSpec changes staged, skip checks.
echo "$STAGED_FILES" | grep -q '^openspec/changes/' || exit 0

CHANGES=$(echo "$STAGED_FILES" | awk -F/ '/^openspec\/changes\// && $3 != "archive" { print $3 }' | sort -u)

if [ -z "$CHANGES" ]; then
  exit 0
fi

for CHANGE in $CHANGES; do
  echo "[pre-commit] validating OpenSpec change: $CHANGE"

  powershell -NoProfile -ExecutionPolicy Bypass -File "Tools/Validate-OpenSpecContextReference.ps1" -ChangeName "$CHANGE"
  openspec.cmd validate "$CHANGE"
done
'@

if ((Test-Path $hookPath) -and -not $Force) {
    throw "Hook already exists at $hookPath. Re-run with -Force to overwrite."
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($hookPath, $hookScript, $utf8NoBom)

Write-Output "Installed pre-commit hook at: $hookPath"
Write-Output "It validates OpenSpec changes staged under openspec/changes/."
