# SCRUM-35 - Commands Runbook

## New

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:new -- SCRUM-35
```

## Validate

```powershell
openspec validate scrum-35-documentacion-componente-dynamicuitable
openspec instructions apply --change "scrum-35-documentacion-componente-dynamicuitable" --json
```

## Jira Done

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:jira-done -- SCRUM-35 -Yes
```

## Archive

```powershell
npm.cmd --prefix Tools/jira-open run opsxj:archive -- SCRUM-35 -Yes
```
