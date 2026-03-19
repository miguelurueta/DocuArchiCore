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

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("opsxdeploy-test-" + [System.Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null

try {
    $toolDir = Join-Path $tempRoot "Tools/iis-deploy"
    $publishPath = Join-Path $tempRoot "publish"
    $sitePath = Join-Path $tempRoot "site"
    $dataPath = Join-Path $tempRoot "data"
    $outputPath = Join-Path $tempRoot "ready"

    New-Item -ItemType Directory -Path $toolDir -Force | Out-Null
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $publishPath "Tools/jira-open") -Force | Out-Null

    Copy-Item -Path (Join-Path $PSScriptRoot "opsxdeploy.ps1") -Destination (Join-Path $toolDir "opsxdeploy.ps1") -Force

    Set-Content -Path (Join-Path $publishPath "DocuArchi.Api.dll") -Value "dll" -NoNewline
    Set-Content -Path (Join-Path $publishPath "DocuArchi.Api.deps.json") -Value "{}" -NoNewline
    Set-Content -Path (Join-Path $publishPath "DocuArchi.Api.runtimeconfig.json") -Value "{}" -NoNewline
    Set-Content -Path (Join-Path $publishPath "web.config") -Value "<configuration />" -NoNewline
    Set-Content -Path (Join-Path $publishPath "appsettings.json") -Value "{`"ConnectionStrings`":{`"MySqlConnection_DA`":`"`"},`"Jwt`":{`"Key`":`"`"},`"PermissionTest`":{`"Secret`":`"`"}}" -NoNewline
    Set-Content -Path (Join-Path $publishPath "appsettings.Development.json") -Value "{}" -NoNewline
    Set-Content -Path (Join-Path $publishPath "Tools/jira-open/package.json") -Value "{}" -NoNewline

    $scriptPath = Join-Path $toolDir "opsxdeploy.ps1"

    try {
        & $scriptPath doctor -PublishPath $publishPath | Out-Null
        throw "Expected doctor to fail when development artifacts exist."
    }
    catch {
        Assert-Contains -Value $_.Exception.Message -Expected "Doctor found"
    }

    Remove-Item -Path (Join-Path $publishPath "appsettings.Development.json") -Force
    Remove-Item -Path (Join-Path $publishPath "Tools") -Recurse -Force

    $doctorOutput = (& $scriptPath doctor -PublishPath $publishPath 2>&1 | Out-String)
    Assert-Contains -Value $doctorOutput -Expected "Doctor passed. Publish is ready for packaging."

    $prepareOutput = (& $scriptPath prepare -SitePath $sitePath -DataPath $dataPath 2>&1 | Out-String)
    Assert-Contains -Value $prepareOutput -Expected "Prepare completed."

    foreach ($folder in @("temp", "uploads", "avatars", "exports", "logs")) {
        $target = Join-Path $dataPath $folder
        if (-not (Test-Path $target)) {
            throw "Expected folder not found: $target"
        }
    }

    $packageOutput = (& $scriptPath publish-package -PublishPath $publishPath -OutputPath $outputPath 2>&1 | Out-String)
    Assert-Contains -Value $packageOutput -Expected "Publish package ready:"

    if (-not (Test-Path (Join-Path $outputPath "DocuArchi.Api.dll"))) {
        throw "Expected package output missing DocuArchi.Api.dll"
    }

    if (Test-Path (Join-Path $outputPath "appsettings.Development.json")) {
        throw "appsettings.Development.json should not be copied into the package output."
    }

    if (Test-Path (Join-Path $outputPath "Tools")) {
        throw "Tools directory should not be copied into the package output."
    }

    Write-Output "PASS: opsxdeploy doctor, prepare and publish-package cover the MVP flow."
}
finally {
    Remove-Item -Path $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
}
