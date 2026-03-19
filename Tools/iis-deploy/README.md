# opsxdeploy

Herramienta interna para preparar una publicacion ASP.NET Core orientada a IIS sin hacer obligatorio el despliegue directo al sitio.

## Comandos

Desde la raiz del repo:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:doctor -- -PublishPath C:\SalidaApiCore
```

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:prepare -- -SitePath C:\inetpub\DocuArchiApi -DataPath C:\AppData\DocuArchiApi
```

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\SalidaApiCore -OutputPath C:\Entrega\DocuArchiApi-ready
```

## Flujo recomendado

1. Publicar desde Visual Studio a una carpeta local.
2. Ejecutar `opsxdeploy:doctor` sobre esa carpeta.
3. Ejecutar `opsxdeploy:prepare` para crear estructura del sitio y storage.
4. Ejecutar `opsxdeploy:publish-package` para generar un paquete limpio listo para IIS.
5. Aplicar el paquete al servidor o al sitio IIS de forma manual o con un paso posterior.

## Alcance del MVP

- `doctor`
  - valida carpeta publish
  - revisa archivos obligatorios
  - bloquea `appsettings.Development.json`
  - detecta artefactos prohibidos
  - detecta secretos evidentes en `appsettings.json`
- `prepare`
  - crea carpeta del sitio
  - crea carpetas operativas bajo `dataPath`
- `publish-package`
  - genera una copia limpia del publish
  - excluye archivos de desarrollo y tooling no productivo

## Manual obligatorio

El procedimiento operativo completo desde Visual Studio hasta IIS se documenta en [Docs/Publicacion/IIS-DocuArchiApi.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Publicacion/IIS-DocuArchiApi.md).

## Notas

- El MVP no hace obligatorio crear App Pool ni sitio IIS.
- El MVP no despliega directo a IIS; deja el paquete listo para aplicacion manual.
- Usa `-WhatIf` para simular acciones de `prepare` y `publish-package`.
