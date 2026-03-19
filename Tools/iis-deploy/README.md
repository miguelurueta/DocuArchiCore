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

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -OutputPath C:\Entrega\DocuArchiApi-ready
```

## Flujo recomendado

1. Publicar desde Visual Studio a una carpeta local.
2. Ejecutar `opsxdeploy:doctor` sobre esa carpeta.
3. Ejecutar `opsxdeploy:prepare` para crear estructura del sitio y storage.
4. Ejecutar `opsxdeploy:publish-package` para generar un paquete limpio listo para IIS.
5. Aplicar el paquete al servidor o al sitio IIS de forma manual o con un paso posterior.

Alternativa:
1. Ejecutar `opsxdeploy:publish-package` con `-ProjectPath` para que el tool haga `dotnet publish` a un staging temporal y construya directamente el paquete final limpio.

## Alcance del MVP

- `doctor`
  - valida carpeta publish
  - revisa archivos obligatorios
  - no exige `web.config`, pero valida su estructura minima si existe
  - bloquea `appsettings.Development.json`
  - detecta artefactos prohibidos
  - detecta secretos evidentes en `appsettings.json`
- `prepare`
  - crea carpeta del sitio
  - crea carpetas operativas bajo `dataPath`
- `publish-package`
  - genera una copia limpia del publish o publica directamente desde un `.csproj`
  - genera un `web.config` base para IIS si el publish no lo trae
  - preserva `web.config` existente sin sobreescribirlo automaticamente
  - valida `aspNetCore/processPath`, `aspNetCore/arguments` y reporta advertencias sobre `environmentVariables`
  - excluye archivos de desarrollo y tooling no productivo
  - sanea `appsettings.json` del paquete final para no transportar secretos conocidos

## Modo ProjectPath

- `publish-package` acepta `-ProjectPath <ruta.csproj>` como alternativa a `-PublishPath`.
- En ese modo el tool ejecuta `dotnet publish` con `-ProjectConfiguration Release` por defecto.
- El publish bruto va a una carpeta temporal de staging y el paquete final se arma desde esa salida.
- El paquete final sigue excluyendo `appsettings.Development.json` y `Tools/**`.
- El `appsettings.json` final reemplaza secretos conocidos por `__SET_IN_IIS__` sin modificar el proyecto fuente.

## Manejo de web.config

- Si el publish no contiene `web.config`, `opsxdeploy:publish-package` genera uno base compatible con `AspNetCoreModuleV2`.
- El `arguments` generado intenta derivarse del ensamblado principal publicado, por ejemplo `.\DocuArchi.Api.dll`.
- La plantilla base incluye placeholders para:
  - `ASPNETCORE_ENVIRONMENT`
  - `ConnectionStrings__MySqlConnection_DA`
  - `ConnectionStrings__MySqlConnection_WFR`
  - `ConnectionStrings__MySqlConnection_WF`
  - `Jwt__Key`
  - `Jwt__Issuer`
  - `Jwt__Audience`
  - `StoragePaths__Temp`
  - `StoragePaths__Uploads`
  - `StoragePaths__Avatars`
  - `StoragePaths__Exports`
  - `StoragePaths__Logs`
- Si el publish ya trae `web.config`, el tool no lo reescribe en esta fase.
- Si falta `aspNetCore/processPath` o `aspNetCore/arguments`, `doctor` y `publish-package` fallan.
- Si falta el bloque `environmentVariables` o los placeholders esperados, el tool emite advertencias para correccion manual.

## Sanitizacion de appsettings.json

- `doctor` sigue fallando si inspecciona una carpeta publish con secretos reales.
- `publish-package` sanea la salida final aunque el publish de origen tenga valores reales conocidos.
- Claves saneadas actualmente:
  - `Jwt.Key`
  - campos `Secret`
  - `ConnectionStrings.MySqlConnection_*`
- El saneamiento ocurre solo en el paquete final; el repo y el publish bruto no se modifican.

## Manual obligatorio

El procedimiento operativo completo desde Visual Studio hasta IIS se documenta en [Docs/Publicacion/IIS-DocuArchiApi.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Publicacion/IIS-DocuArchiApi.md).

## Notas

- El MVP no hace obligatorio crear App Pool ni sitio IIS.
- El MVP no despliega directo a IIS; deja el paquete listo para aplicacion manual.
- Usa `-WhatIf` para simular acciones de `prepare` y `publish-package`.
