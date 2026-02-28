param()

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

$configPath = Join-Path $PSScriptRoot ".jira-open.env"
$baseUrl = Get-JiraConfigValue -Key "JIRA_BASE_URL" -ConfigPath $configPath
$email = Get-JiraConfigValue -Key "JIRA_EMAIL" -ConfigPath $configPath
$token = Get-JiraConfigValue -Key "JIRA_API_TOKEN" -ConfigPath $configPath

if (-not $baseUrl) { $baseUrl = $env:JIRA_BASE_URL }
if (-not $email) { $email = $env:JIRA_EMAIL }
if (-not $token) { $token = $env:JIRA_API_TOKEN }

if (-not $baseUrl -or -not $email -or -not $token) {
    throw "Set JIRA_BASE_URL, JIRA_EMAIL and JIRA_API_TOKEN environment variables."
}

$baseUrl = $baseUrl.TrimEnd("/")
$uri = "$baseUrl/rest/api/3/myself"

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
    throw "Jira connection failed. $($_.Exception.Message)"
}

Write-Output "Jira connection successful."
Write-Output "User: $($response.displayName)"
Write-Output "Account ID: $($response.accountId)"
