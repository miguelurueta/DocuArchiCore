# SCRUM-34 - Commands Runbook

## New

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:new -- SCRUM-34
```

## Validate

```powershell
openspec validate scrum-34-crea-componte-dymanic-ui-table
openspec instructions apply --change "scrum-34-crea-componte-dymanic-ui-table" --json
dotnet build D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj
dotnet test D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\tests\TramiteDiasVencimiento.Tests\TramiteDiasVencimiento.Tests.csproj --filter DynamicUiTableServiceTests
```

## Troubleshooting Build Locks

```powershell
Get-Process -Id 34204
Stop-Process -Id 34204 -Force
dotnet build D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj
```

## Jira Done

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:jira-done -- SCRUM-34 -Yes
```

## Archive

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:archive -- SCRUM-34 -Yes
```
