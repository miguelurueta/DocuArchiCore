## Why

El flujo actual de `opsxdeploy` parte de una carpeta de publish ya generada y limpia. En la práctica, el primer ejercicio operativo mostró dos fricciones: el operador espera que el tool pueda publicar directamente desde el proyecto objetivo y el publish real de `DocuArchi.Api` sigue incluyendo artefactos no productivos y configuracion sensible que bloquean `doctor`.

## What Changes

- Extender `opsxdeploy publish-package` para aceptar un proyecto origen (`.csproj`) y ejecutar `dotnet publish` como parte del flujo.
- Agregar una etapa de sanitizacion del publish para excluir `appsettings.Development.json`, `Tools/**` y otros artefactos no productivos antes de armar el paquete final.
- Definir una politica explicita para secretos/configuracion sensible en `appsettings.json` del paquete final, priorizando placeholders o blanking controlado sin modificar el source repo.
- Mantener compatibilidad con el modo actual basado en `-PublishPath`.
- Actualizar pruebas y documentacion operativa para reflejar el flujo repo/proyecto -> publish temporal -> paquete IIS listo.

## Capabilities

### New Capabilities
- None.

### Modified Capabilities
- `jira-scrum-78`: ampliar `opsxdeploy` para soportar publish directo desde proyecto y sanitizacion automatica del paquete final para IIS.

## Impact

- Repositorios impactados:
  - `DocuArchiCore` como coordinador del cambio y contenedor del tool `Tools/iis-deploy`
  - `DocuArchi.Api` como proyecto fuente para el publish operativo
- Referencias de contexto:
  - `openspec/context/multi-repo-context.md`
  - `openspec/context/OPSXJ_BACKEND_RULES.md`
- Codigo/documentacion afectada:
  - `Tools/iis-deploy/opsxdeploy.ps1`
  - `Tools/iis-deploy/test-opsxdeploy-flow.ps1`
  - `Tools/iis-deploy/README.md`
  - `Docs/Publicacion/IIS-DocuArchiApi.md`
