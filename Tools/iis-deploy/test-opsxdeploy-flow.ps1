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

function New-TestPublish {
    param(
        [string]$RootPath,
        [switch]$IncludeWebConfig,
        [string]$WebConfigContent = $null,
        [switch]$IncludeDevelopmentArtifacts,
        [switch]$PopulateSecrets
    )

    $publishPath = Join-Path $RootPath ([System.Guid]::NewGuid().ToString("N"))
    New-Item -ItemType Directory -Path $publishPath -Force | Out-Null

    Set-Content -Path (Join-Path $publishPath "DocuArchi.Api.dll") -Value "dll" -NoNewline
    Set-Content -Path (Join-Path $publishPath "DocuArchi.Api.deps.json") -Value "{}" -NoNewline
    Set-Content -Path (Join-Path $publishPath "DocuArchi.Api.runtimeconfig.json") -Value "{}" -NoNewline
    $appSettingsValue = if ($PopulateSecrets) {
        "{`"ConnectionStrings`":{`"MySqlConnection_DA`":`"server=localhost;uid=root;pwd=123;`"},`"Jwt`":{`"Key`":`"secret-key`"},`"PermissionTest`":{`"Secret`":`"secret-value`"}}"
    }
    else {
        "{`"ConnectionStrings`":{`"MySqlConnection_DA`":`"`"},`"Jwt`":{`"Key`":`"`"},`"PermissionTest`":{`"Secret`":`"`"}}"
    }
    Set-Content -Path (Join-Path $publishPath "appsettings.json") -Value $appSettingsValue -NoNewline

    if ($IncludeWebConfig) {
        Set-Content -Path (Join-Path $publishPath "web.config") -Value $WebConfigContent -NoNewline
    }

    if ($IncludeDevelopmentArtifacts) {
        New-Item -ItemType Directory -Path (Join-Path $publishPath "Tools/jira-open") -Force | Out-Null
        Set-Content -Path (Join-Path $publishPath "appsettings.Development.json") -Value "{}" -NoNewline
        Set-Content -Path (Join-Path $publishPath "Tools/jira-open/package.json") -Value "{}" -NoNewline
    }

    return $publishPath
}

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("opsxdeploy-test-" + [System.Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null

try {
    $toolDir = Join-Path $tempRoot "Tools/iis-deploy"
    $sitePath = Join-Path $tempRoot "site"
    $dataPath = Join-Path $tempRoot "data"

    New-Item -ItemType Directory -Path $toolDir -Force | Out-Null

    Copy-Item -Path (Join-Path $PSScriptRoot "opsxdeploy.ps1") -Destination (Join-Path $toolDir "opsxdeploy.ps1") -Force

    $scriptPath = Join-Path $toolDir "opsxdeploy.ps1"
    $fakeProjectPath = Join-Path $tempRoot "DocuArchi.Api.csproj"
    Set-Content -Path $fakeProjectPath -Value "<Project />" -NoNewline
    $validWebConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <aspNetCore processPath="dotnet" arguments=".\DocuArchi.Api.dll">
      <environmentVariables>
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
      </environmentVariables>
    </aspNetCore>
  </system.webServer>
</configuration>
"@

    $invalidArtifactsPublishPath = New-TestPublish -RootPath $tempRoot -IncludeWebConfig -WebConfigContent $validWebConfig -IncludeDevelopmentArtifacts -PopulateSecrets
    try {
        & $scriptPath doctor -PublishPath $invalidArtifactsPublishPath | Out-Null
        throw "Expected doctor to fail when development artifacts exist."
    }
    catch {
        Assert-Contains -Value $_.Exception.Message -Expected "Doctor found"
    }

    $noWebConfigPublishPath = New-TestPublish -RootPath $tempRoot
    $doctorOutput = (& $scriptPath doctor -PublishPath $noWebConfigPublishPath 2>&1 | Out-String)
    Assert-Contains -Value $doctorOutput -Expected "Doctor passed. Publish is ready for packaging."
    Assert-Contains -Value $doctorOutput -Expected "web.config not found; publish-package will generate a base file."

    $prepareOutput = (& $scriptPath prepare -SitePath $sitePath -DataPath $dataPath 2>&1 | Out-String)
    Assert-Contains -Value $prepareOutput -Expected "Prepare completed."

    foreach ($folder in @("temp", "uploads", "avatars", "exports", "logs")) {
        $target = Join-Path $dataPath $folder
        if (-not (Test-Path $target)) {
            throw "Expected folder not found: $target"
        }
    }

    $generatedOutputPath = Join-Path $tempRoot "ready-generated"
    $packageOutput = (& $scriptPath publish-package -PublishPath $noWebConfigPublishPath -OutputPath $generatedOutputPath 2>&1 | Out-String)
    Assert-Contains -Value $packageOutput -Expected "Publish package ready:"
    Assert-Contains -Value $packageOutput -Expected "Generated base web.config"

    if (-not (Test-Path (Join-Path $generatedOutputPath "DocuArchi.Api.dll"))) {
        throw "Expected package output missing DocuArchi.Api.dll"
    }

    if (-not (Test-Path (Join-Path $generatedOutputPath "web.config"))) {
        throw "Expected generated package output to include web.config"
    }

    $generatedWebConfig = Get-Content -Path (Join-Path $generatedOutputPath "web.config") -Raw
    Assert-Contains -Value $generatedWebConfig -Expected 'arguments=".\DocuArchi.Api.dll"'
    Assert-Contains -Value $generatedWebConfig -Expected 'name="ConnectionStrings__MySqlConnection_DA"'

    if (Test-Path (Join-Path $generatedOutputPath "appsettings.Development.json")) {
        throw "appsettings.Development.json should not be copied into the package output."
    }

    if (Test-Path (Join-Path $generatedOutputPath "Tools")) {
        throw "Tools directory should not be copied into the package output."
    }

    $existingWebConfigPublishPath = New-TestPublish -RootPath $tempRoot -IncludeWebConfig -WebConfigContent $validWebConfig -PopulateSecrets
    $existingOutputPath = Join-Path $tempRoot "ready-existing"
    $existingPackageOutput = (& $scriptPath publish-package -PublishPath $existingWebConfigPublishPath -OutputPath $existingOutputPath 2>&1 | Out-String)
    Assert-Contains -Value $existingPackageOutput -Expected "Publish package ready:"

    $copiedWebConfig = Get-Content -Path (Join-Path $existingOutputPath "web.config") -Raw
    if ($copiedWebConfig -ne $validWebConfig) {
        throw "Expected existing web.config to be preserved in package output."
    }

    $sanitizedAppSettings = Get-Content -Path (Join-Path $existingOutputPath "appsettings.json") -Raw
    Assert-Contains -Value $sanitizedAppSettings -Expected "__SET_IN_IIS__"
    if ($sanitizedAppSettings.Contains("uid=root") -or $sanitizedAppSettings.Contains("secret-key") -or $sanitizedAppSettings.Contains("secret-value")) {
        throw "Expected appsettings.json in package output to be sanitized."
    }

    $invalidWebConfigPublishPath = New-TestPublish -RootPath $tempRoot -IncludeWebConfig -WebConfigContent "<configuration><system.webServer><aspNetCore processPath=`"dotnet`" /></system.webServer></configuration>"
    try {
        & $scriptPath publish-package -PublishPath $invalidWebConfigPublishPath -OutputPath (Join-Path $tempRoot "ready-invalid") | Out-Null
        throw "Expected publish-package to fail when web.config is missing required arguments."
    }
    catch {
        Assert-Contains -Value $_.Exception.Message -Expected "Publish source failed blocking checks before packaging."
    }

    $projectSourcePublishPath = New-TestPublish -RootPath $tempRoot -IncludeWebConfig -WebConfigContent $validWebConfig -IncludeDevelopmentArtifacts -PopulateSecrets
    $projectOutputPath = Join-Path $tempRoot "ready-project"
    $env:OPSXDEPLOY_TEST_PUBLISH_SOURCE = $projectSourcePublishPath
    try {
        $projectPackageOutput = (& $scriptPath publish-package -ProjectPath $fakeProjectPath -OutputPath $projectOutputPath 2>&1 | Out-String)
        Assert-Contains -Value $projectPackageOutput -Expected "Using project source:"
        Assert-Contains -Value $projectPackageOutput -Expected "Publish package ready:"

        if (Test-Path (Join-Path $projectOutputPath "appsettings.Development.json")) {
            throw "Project-based package output should exclude appsettings.Development.json."
        }

        if (Test-Path (Join-Path $projectOutputPath "Tools")) {
            throw "Project-based package output should exclude Tools."
        }

        $projectAppSettings = Get-Content -Path (Join-Path $projectOutputPath "appsettings.json") -Raw
        Assert-Contains -Value $projectAppSettings -Expected "__SET_IN_IIS__"
    }
    finally {
        Remove-Item Env:\OPSXDEPLOY_TEST_PUBLISH_SOURCE -ErrorAction SilentlyContinue
    }

    Write-Output "PASS: opsxdeploy doctor, prepare and publish-package cover validation, sanitization and project-based packaging."
}
finally {
    Remove-Item -Path $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
}
