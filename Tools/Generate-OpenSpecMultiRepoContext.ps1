param(
    [string]$ScanRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\\..")).Path,
    [string]$OpenSpecRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\\openspec")).Path,
    [string]$OutputFileName = "multi-repo-context.md",
    [string]$OperationalSummaryPath = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot "..")) "Docs\\OpenSpec_Contexto_Operativo.md")
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $OpenSpecRoot)) {
    throw "OpenSpec root not found: $OpenSpecRoot"
}

$contextDir = Join-Path $OpenSpecRoot "context"
$outputPath = Join-Path $contextDir $OutputFileName
New-Item -ItemType Directory -Path $contextDir -Force | Out-Null

$gitDirs = Get-ChildItem -Path $ScanRoot -Recurse -Directory -Force |
    Where-Object { $_.Name -eq ".git" }
$repos = $gitDirs | ForEach-Object { $_.Parent.FullName } | Sort-Object -Unique |
    Where-Object {
        $repoPath = [string]$_
        $isTempRepo = $repoPath -match "\\\.tmp\\"
        $isFrontRepo = $repoPath -match "DocuArchiCore\.Front$"
        $isContainerRepo = ($repoPath -eq $ScanRoot)
        (-not $isTempRepo) -and (-not $isFrontRepo) -and (-not $isContainerRepo)
    }

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("# Multi-Repo Context")
$lines.Add("")
$lines.Add("- Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss K')")
$lines.Add("- OpenSpec root: $OpenSpecRoot")
$lines.Add("- Scan root: $ScanRoot")
$lines.Add("- Repositories found: $($repos.Count)")
$lines.Add("")
$lines.Add("## Repository Inventory")
$lines.Add("")

foreach ($repo in $repos) {
    $repoName = Split-Path $repo -Leaf
    $branch = git -c safe.directory=* -C $repo rev-parse --abbrev-ref HEAD 2>$null
    if (-not $branch) { $branch = "N/A" }

    $lastCommit = git -c safe.directory=* -C $repo log -1 --pretty=format:"%h | %ad" --date=short 2>$null
    if (-not $lastCommit) { $lastCommit = "N/A" }

    $remotesRaw = git -c safe.directory=* -C $repo remote -v 2>$null
    $remotes = @()
    if ($remotesRaw) {
        $remotes = $remotesRaw |
            Where-Object { $_ -match "\(fetch\)$" } |
            ForEach-Object { $_ -replace "\s+\(fetch\)$", "" } |
            Sort-Object -Unique
    }

    $slnFiles = Get-ChildItem -Path $repo -Recurse -File -Filter *.sln -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch "\\\.tmp\\" } |
        ForEach-Object { $_.FullName }
    $csprojFiles = Get-ChildItem -Path $repo -Recurse -File -Filter *.csproj -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch "\\\.tmp\\" } |
        ForEach-Object { $_.FullName }
    $packageFiles = Get-ChildItem -Path $repo -Recurse -File -Filter package.json -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch "\\\.tmp\\" } |
        ForEach-Object { $_.FullName }

    $lines.Add("### $repoName")
    $lines.Add("")
    $lines.Add("- Path: $repo")
    $lines.Add("- Branch: $branch")
    $lines.Add("- Last commit: $lastCommit")
    if ($remotes.Count -gt 0) {
        $lines.Add("- Remotes:")
        foreach ($remote in $remotes) {
            $lines.Add("  - $remote")
        }
    }
    else {
        $lines.Add("- Remotes: N/A")
    }

    $lines.Add("- Build artifacts:")
    if ($slnFiles.Count -gt 0) {
        foreach ($file in $slnFiles) { $lines.Add("  - .sln: $file") }
    }
    else {
        $lines.Add("  - .sln: none")
    }

    if ($csprojFiles.Count -gt 0) {
        $lines.Add("  - .csproj count: $($csprojFiles.Count)")
        foreach ($file in ($csprojFiles | Select-Object -First 12)) {
            $lines.Add("    - $file")
        }
        if ($csprojFiles.Count -gt 12) {
            $lines.Add("    - ... (+$($csprojFiles.Count - 12) more)")
        }
    }
    else {
        $lines.Add("  - .csproj: none")
    }

    if ($packageFiles.Count -gt 0) {
        foreach ($file in $packageFiles) { $lines.Add("  - package.json: $file") }
    }
    else {
        $lines.Add("  - package.json: none")
    }

    $lines.Add("")
}

$lines.Add("## Cross-Repo Project References")
$lines.Add("")

$allCsproj = Get-ChildItem -Path $ScanRoot -Recurse -File -Filter *.csproj -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch "\\\.tmp\\" }
$projectRefs = foreach ($csproj in $allCsproj) {
    Select-String -Path $csproj.FullName -Pattern "<ProjectReference Include=" -SimpleMatch -ErrorAction SilentlyContinue
}

if ($projectRefs) {
    foreach ($match in $projectRefs) {
        $lines.Add("- $($match.Path):$($match.LineNumber): $($match.Line.Trim())")
    }
}
else {
    $lines.Add("- No ProjectReference entries found.")
}

$lines.Add("")
$lines.Add("## Suggested OpenSpec Usage")
$lines.Add("")
$lines.Add("- Reference this file from proposal and design docs for cross-repo changes.")
$lines.Add("- Regenerate this file before starting each relevant cross-repo change.")

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllLines($outputPath, $lines, $utf8NoBom)

Write-Output "Generated: $outputPath"

$summaryLines = New-Object System.Collections.Generic.List[string]
$summaryLines.Add("# OpenSpec Contexto Operativo")
$summaryLines.Add("")
$summaryLines.Add('Este archivo se actualiza automaticamente al ejecutar `Tools/Generate-OpenSpecMultiRepoContext.ps1`.')
$summaryLines.Add("")
$summaryLines.Add("- Updated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss K')")
$summaryLines.Add("- OpenSpec root: $OpenSpecRoot")
$summaryLines.Add("- Context source: openspec/context/$OutputFileName")
$summaryLines.Add("- Repositories found: $($repos.Count)")
$summaryLines.Add("")
$summaryLines.Add("## Reglas Obligatorias")
$summaryLines.Add("")
$summaryLines.Add("1. Antes de cambios cross-repo, regenerar el contexto multi-repo.")
$summaryLines.Add('2. Proposals y design deben referenciar `openspec/context/multi-repo-context.md`.')
$summaryLines.Add('3. Definir repos impactados en `sync.md` usando `openspec/context/REPO_IMPACT_TEMPLATE.md`.')
$summaryLines.Add('4. Ejecutar `opsxj:new` solo en repos con `Impacta? = yes`.')
$summaryLines.Add('5. Ejecutar `opsxj:archive` en cada repo solo despues del merge de su PR.')
$summaryLines.Add("")
$summaryLines.Add("## Inventario de Repos")
$summaryLines.Add("")

foreach ($repo in $repos) {
    $repoName = Split-Path $repo -Leaf
    $branch = git -c safe.directory=* -C $repo rev-parse --abbrev-ref HEAD 2>$null
    if (-not $branch) { $branch = "N/A" }
    $remote = git -c safe.directory=* -C $repo remote get-url origin 2>$null
    if (-not $remote) { $remote = "N/A" }
    $summaryLines.Add("- $repoName")
    $summaryLines.Add("  - Path: $repo")
    $summaryLines.Add("  - Branch: $branch")
    $summaryLines.Add("  - Remote(origin): $remote")
}

$summaryLines.Add("")
$summaryLines.Add("## Referencias")
$summaryLines.Add("")
$summaryLines.Add('- Context completo: `openspec/context/multi-repo-context.md`')
$summaryLines.Add('- Checklist: `openspec/context/CROSS_REPO_CHECKLIST.md`')
$summaryLines.Add('- Plantilla de impacto: `openspec/context/REPO_IMPACT_TEMPLATE.md`')
$summaryLines.Add('- Guia operativa: `Docs/Guia_Opsxj_MultiRepo.md`')

$summaryDir = Split-Path -Parent $OperationalSummaryPath
if (-not (Test-Path $summaryDir)) {
    New-Item -ItemType Directory -Path $summaryDir -Force | Out-Null
}
[System.IO.File]::WriteAllLines($OperationalSummaryPath, $summaryLines, $utf8NoBom)
Write-Output "Generated: $OperationalSummaryPath"
