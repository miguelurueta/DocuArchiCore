param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet("doctor", "prepare", "publish-package")]
    [string]$Command,

    [string]$PublishPath,
    [string]$OutputPath,
    [string]$SitePath,
    [string]$DataPath,
    [switch]$WhatIf,
    [switch]$AllowSecrets
)

$ErrorActionPreference = "Stop"

function Assert-RequiredArgument {
    param(
        [string]$Name,
        [string]$Value
    )

    if ([string]::IsNullOrWhiteSpace($Value)) {
        throw "Missing required parameter --$Name."
    }
}

function Write-Section {
    param([string]$Title)
    Write-Output ""
    Write-Output $Title
}

function Add-ReportItem {
    param(
        [System.Collections.ArrayList]$Items,
        [string]$Level,
        [string]$Name,
        [string]$Message
    )

    [void]$Items.Add([pscustomobject]@{
        level = $Level
        name = $Name
        message = $Message
    })
}

function Write-Report {
    param([System.Collections.ArrayList]$Items)

    foreach ($item in $Items) {
        Write-Output ("[{0}] {1}: {2}" -f $item.level.ToUpperInvariant(), $item.name, $item.message)
    }
}

function Get-RequiredPublishFiles {
    return @("*.dll", "*.deps.json", "*.runtimeconfig.json")
}

function Test-PublishFileRequirements {
    param([string]$Path)

    $missing = New-Object System.Collections.ArrayList
    foreach ($pattern in Get-RequiredPublishFiles) {
        $matches = Get-ChildItem -Path $Path -Filter $pattern -File -ErrorAction SilentlyContinue
        if (-not $matches) {
            [void]$missing.Add($pattern)
        }
    }

    return $missing
}

function Get-ForbiddenArtifacts {
    param([string]$Path)

    $findings = New-Object System.Collections.ArrayList

    $developmentSettings = Join-Path $Path "appsettings.Development.json"
    if (Test-Path $developmentSettings) {
        [void]$findings.Add($developmentSettings)
    }

    $toolingPath = Join-Path $Path "Tools"
    if (Test-Path $toolingPath) {
        $toolingFiles = Get-ChildItem -Path $toolingPath -Recurse -File -ErrorAction SilentlyContinue
        foreach ($file in $toolingFiles) {
            [void]$findings.Add($file.FullName)
        }
    }

    return $findings
}

function Get-PublishAssemblyName {
    param([string]$Path)

    $runtimeConfig = Get-ChildItem -Path $Path -Filter "*.runtimeconfig.json" -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($runtimeConfig) {
        return $runtimeConfig.BaseName -replace '\.runtimeconfig$', ''
    }

    $deps = Get-ChildItem -Path $Path -Filter "*.deps.json" -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($deps) {
        return $deps.BaseName -replace '\.deps$', ''
    }

    $dll = Get-ChildItem -Path $Path -Filter "*.dll" -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($dll) {
        return $dll.BaseName
    }

    throw "Unable to derive assembly name from publish path: $Path"
}

function Get-ExpectedWebConfigEnvironmentVariableNames {
    return @(
        "ASPNETCORE_ENVIRONMENT",
        "ConnectionStrings__MySqlConnection_DA",
        "ConnectionStrings__MySqlConnection_WFR",
        "ConnectionStrings__MySqlConnection_WF",
        "Jwt__Key",
        "Jwt__Issuer",
        "Jwt__Audience",
        "StoragePaths__Temp",
        "StoragePaths__Uploads",
        "StoragePaths__Avatars",
        "StoragePaths__Exports",
        "StoragePaths__Logs"
    )
}

function New-BaseWebConfigContent {
    param([string]$AssemblyName)

    $environmentVariableLines = foreach ($name in Get-ExpectedWebConfigEnvironmentVariableNames) {
        "          <environmentVariable name=""$name"" value=""__SET_IN_IIS__"" />"
    }

    return @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\$AssemblyName.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
        <environmentVariables>
$($environmentVariableLines -join [Environment]::NewLine)
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
"@
}

function Test-WebConfigMinimumStructure {
    param([string]$Path)

    $items = New-Object System.Collections.ArrayList

    if (-not (Test-Path $Path)) {
        Add-ReportItem -Items $items -Level "warn" -Name "web_config" -Message "web.config not found; publish-package will generate a base file."
        return $items
    }

    try {
        [xml]$xml = Get-Content -Path $Path -Raw
    }
    catch {
        Add-ReportItem -Items $items -Level "fail" -Name "web_config" -Message "web.config is not valid XML. $($_.Exception.Message)"
        return $items
    }

    $configuration = $xml.configuration
    if (-not $configuration) {
        Add-ReportItem -Items $items -Level "fail" -Name "web_config" -Message "web.config must include a <configuration> root node."
        return $items
    }

    $systemWebServer = $configuration.SelectSingleNode("location/system.webServer")
    if (-not $systemWebServer) {
        $systemWebServer = $configuration.SelectSingleNode("system.webServer")
    }

    if (-not $systemWebServer) {
        Add-ReportItem -Items $items -Level "fail" -Name "web_config" -Message "web.config must include <system.webServer>."
        return $items
    }

    $aspNetCore = $systemWebServer.SelectSingleNode("aspNetCore")
    if (-not $aspNetCore) {
        Add-ReportItem -Items $items -Level "fail" -Name "web_config" -Message "web.config must include <aspNetCore>."
        return $items
    }

    $processPath = $aspNetCore.GetAttribute("processPath")
    if ([string]::IsNullOrWhiteSpace($processPath)) {
        Add-ReportItem -Items $items -Level "fail" -Name "web_config.processPath" -Message "aspNetCore/processPath is required."
    }
    else {
        Add-ReportItem -Items $items -Level "pass" -Name "web_config.processPath" -Message "aspNetCore/processPath is configured."
    }

    $arguments = $aspNetCore.GetAttribute("arguments")
    if ([string]::IsNullOrWhiteSpace($arguments)) {
        Add-ReportItem -Items $items -Level "fail" -Name "web_config.arguments" -Message "aspNetCore/arguments is required."
    }
    else {
        Add-ReportItem -Items $items -Level "pass" -Name "web_config.arguments" -Message "aspNetCore/arguments is configured."
    }

    $environmentVariables = $aspNetCore.SelectSingleNode("environmentVariables")
    if (-not $environmentVariables) {
        Add-ReportItem -Items $items -Level "warn" -Name "web_config.environmentVariables" -Message "web.config does not include an <environmentVariables> block."
        return $items
    }

    Add-ReportItem -Items $items -Level "pass" -Name "web_config.environmentVariables" -Message "web.config includes an <environmentVariables> block."

    $actualNames = @(
        $environmentVariables.SelectNodes("environmentVariable") |
        ForEach-Object { $_.GetAttribute("name") } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    )

    $missingNames = @(Get-ExpectedWebConfigEnvironmentVariableNames | Where-Object { $_ -notin $actualNames })
    if ($missingNames.Count -gt 0) {
        Add-ReportItem -Items $items -Level "warn" -Name "web_config.environmentVariables" -Message ("Missing placeholder variables: {0}" -f ($missingNames -join ", "))
    }
    else {
        Add-ReportItem -Items $items -Level "pass" -Name "web_config.environmentVariables" -Message "Expected placeholder variables are present."
    }

    return $items
}

function Write-WebConfigReport {
    param(
        [string]$Title,
        [System.Collections.ArrayList]$Items
    )

    Write-Section $Title
    Write-Report -Items $Items
}

function Assert-NoFailingReportItems {
    param(
        [System.Collections.ArrayList]$Items,
        [string]$FailureMessage
    )

    $failed = @($Items | Where-Object { $_.level -eq "fail" })
    if ($failed.Count -gt 0) {
        throw $FailureMessage
    }
}

function Ensure-PackageWebConfig {
    param(
        [string]$PublishPath,
        [string]$OutputPath,
        [switch]$WhatIf
    )

    $sourceWebConfig = Join-Path $PublishPath "web.config"
    $targetWebConfig = Join-Path $OutputPath "web.config"

    if (Test-Path $sourceWebConfig) {
        $items = Test-WebConfigMinimumStructure -Path $targetWebConfig
        Write-WebConfigReport -Title "opsxdeploy web.config report" -Items $items
        Assert-NoFailingReportItems -Items $items -FailureMessage "web.config validation found failing checks."
        return
    }

    $assemblyName = Get-PublishAssemblyName -Path $PublishPath
    if ($WhatIf) {
        Write-Output "[WHATIF] Generate base web.config for assembly: $assemblyName"
        return
    }

    Set-Content -Path $targetWebConfig -Value (New-BaseWebConfigContent -AssemblyName $assemblyName) -Encoding UTF8
    Write-Output "[PASS] Generated base web.config: $targetWebConfig"

    $items = Test-WebConfigMinimumStructure -Path $targetWebConfig
    Write-WebConfigReport -Title "opsxdeploy web.config report" -Items $items
    Assert-NoFailingReportItems -Items $items -FailureMessage "Generated web.config validation found failing checks."
}

function Get-AppSettingsSecretFindings {
    param([string]$Path)

    $appSettingsPath = Join-Path $Path "appsettings.json"
    if (-not (Test-Path $appSettingsPath)) {
        return @()
    }

    $raw = Get-Content -Path $appSettingsPath -Raw
    $findings = New-Object System.Collections.ArrayList

    $patterns = @(
        @{ name = "jwt_key"; pattern = '"Key"\s*:\s*"(?!\s*")[^"]+'; message = "Jwt.Key appears populated." },
        @{ name = "permission_secret"; pattern = '"Secret"\s*:\s*"(?!\s*")[^"]+'; message = "A Secret field appears populated." },
        @{ name = "mysql_password"; pattern = '(pwd|password)\s*='; message = "Connection string contains password or pwd." },
        @{ name = "mysql_root"; pattern = 'uid\s*=\s*root'; message = "Connection string uses root user." }
    )

    foreach ($entry in $patterns) {
        if ($raw -match $entry.pattern) {
            [void]$findings.Add([pscustomobject]@{
                name = $entry.name
                message = $entry.message
            })
        }
    }

    return $findings
}

function Invoke-Doctor {
    param([string]$PublishPath, [switch]$AllowSecrets)

    Assert-RequiredArgument -Name "publishPath" -Value $PublishPath
    $resolvedPublishPath = (Resolve-Path -Path $PublishPath).Path
    $items = New-Object System.Collections.ArrayList

    Add-ReportItem -Items $items -Level "pass" -Name "publish_path" -Message "Publish path resolved: $resolvedPublishPath"

    $missing = Test-PublishFileRequirements -Path $resolvedPublishPath
    if ($missing.Count -gt 0) {
        foreach ($entry in $missing) {
            Add-ReportItem -Items $items -Level "fail" -Name "required_files" -Message "Missing required pattern: $entry"
        }
    }
    else {
        Add-ReportItem -Items $items -Level "pass" -Name "required_files" -Message "Required runtime files found."
    }

    $forbidden = Get-ForbiddenArtifacts -Path $resolvedPublishPath
    if ($forbidden.Count -gt 0) {
        foreach ($entry in $forbidden) {
            Add-ReportItem -Items $items -Level "fail" -Name "forbidden_artifact" -Message $entry
        }
    }
    else {
        Add-ReportItem -Items $items -Level "pass" -Name "forbidden_artifact" -Message "No forbidden development/tooling artifacts found."
    }

    $secretFindings = Get-AppSettingsSecretFindings -Path $resolvedPublishPath
    if ($secretFindings.Count -gt 0) {
        $level = if ($AllowSecrets) { "warn" } else { "fail" }
        foreach ($finding in $secretFindings) {
            Add-ReportItem -Items $items -Level $level -Name $finding.name -Message $finding.message
        }
    }
    else {
        Add-ReportItem -Items $items -Level "pass" -Name "appsettings" -Message "No obvious secrets found in appsettings.json."
    }

    $webConfigItems = Test-WebConfigMinimumStructure -Path (Join-Path $resolvedPublishPath "web.config")
    foreach ($item in $webConfigItems) {
        [void]$items.Add($item)
    }

    Write-Section "opsxdeploy doctor report"
    Write-Report -Items $items

    Assert-NoFailingReportItems -Items $items -FailureMessage ("Doctor found {0} failing check(s)." -f (@($items | Where-Object { $_.level -eq "fail" }).Count))

    Write-Output "Doctor passed. Publish is ready for packaging."
}

function Ensure-Directory {
    param(
        [string]$Path,
        [switch]$WhatIf
    )

    if (Test-Path $Path) {
        return
    }

    if ($WhatIf) {
        Write-Output "[WHATIF] Create directory: $Path"
        return
    }

    New-Item -ItemType Directory -Path $Path -Force | Out-Null
    Write-Output "[PASS] Created directory: $Path"
}

function Invoke-Prepare {
    param(
        [string]$SitePath,
        [string]$DataPath,
        [switch]$WhatIf
    )

    Assert-RequiredArgument -Name "sitePath" -Value $SitePath
    Assert-RequiredArgument -Name "dataPath" -Value $DataPath

    Write-Section "opsxdeploy prepare"

    Ensure-Directory -Path $SitePath -WhatIf:$WhatIf
    Ensure-Directory -Path $DataPath -WhatIf:$WhatIf

    foreach ($folder in @("temp", "uploads", "avatars", "exports", "logs")) {
        Ensure-Directory -Path (Join-Path $DataPath $folder) -WhatIf:$WhatIf
    }

    Write-Output "Prepare completed."
}

function Copy-PublishPackage {
    param(
        [string]$PublishPath,
        [string]$OutputPath,
        [switch]$WhatIf
    )

    $excludedRelativePaths = @(
        "appsettings.Development.json"
    )

    $excludedRootDirs = @("Tools")

    $sourceFiles = Get-ChildItem -Path $PublishPath -Recurse -File
    foreach ($file in $sourceFiles) {
        $relativePath = $file.FullName.Substring($PublishPath.Length).TrimStart('\')

        if ($excludedRelativePaths -contains $relativePath) {
            continue
        }

        $rootSegment = ($relativePath -split '[\\/]')[0]
        if ($excludedRootDirs -contains $rootSegment) {
            continue
        }

        $targetPath = Join-Path $OutputPath $relativePath
        $targetDir = Split-Path -Path $targetPath -Parent

        if ($WhatIf) {
            Write-Output "[WHATIF] Copy $relativePath"
            continue
        }

        if (-not (Test-Path $targetDir)) {
            New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
        }

        Copy-Item -Path $file.FullName -Destination $targetPath -Force
    }
}

function Invoke-PublishPackage {
    param(
        [string]$PublishPath,
        [string]$OutputPath,
        [switch]$WhatIf,
        [switch]$AllowSecrets
    )

    Assert-RequiredArgument -Name "publishPath" -Value $PublishPath
    Assert-RequiredArgument -Name "outputPath" -Value $OutputPath

    $resolvedPublishPath = (Resolve-Path -Path $PublishPath).Path
    Invoke-Doctor -PublishPath $resolvedPublishPath -AllowSecrets:$AllowSecrets

    Write-Section "opsxdeploy publish-package"

    if (-not $WhatIf) {
        if (Test-Path $OutputPath) {
            Remove-Item -Path $OutputPath -Recurse -Force
        }
        New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    }
    else {
        Write-Output "[WHATIF] Recreate output folder: $OutputPath"
    }

    Copy-PublishPackage -PublishPath $resolvedPublishPath -OutputPath $OutputPath -WhatIf:$WhatIf
    Ensure-PackageWebConfig -PublishPath $resolvedPublishPath -OutputPath $OutputPath -WhatIf:$WhatIf
    Write-Output "Publish package ready: $OutputPath"
}

switch ($Command) {
    "doctor" {
        Invoke-Doctor -PublishPath $PublishPath -AllowSecrets:$AllowSecrets
    }
    "prepare" {
        Invoke-Prepare -SitePath $SitePath -DataPath $DataPath -WhatIf:$WhatIf
    }
    "publish-package" {
        Invoke-PublishPackage -PublishPath $PublishPath -OutputPath $OutputPath -WhatIf:$WhatIf -AllowSecrets:$AllowSecrets
    }
}
