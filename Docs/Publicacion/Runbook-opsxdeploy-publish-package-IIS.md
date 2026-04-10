# Runbook Operativo de `opsxdeploy:publish-package` para IIS

## Objetivo

Este runbook resume el flujo mínimo y repetible para publicar `DocuArchi.Api` en IIS usando `opsxdeploy:publish-package`.

Documento técnico completo:
- [Manual-tecnico-completo-opsxdeploy-publish-package.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Publicacion/Manual-tecnico-completo-opsxdeploy-publish-package.md)

## Variables del entorno

Ejemplo usado en este entorno:

```powershell
$ProjectPath = 'D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj'
$PackagePath = 'C:\publis\publis-ready'
$SitePath = 'C:\inetpub\DocuArchiApi'
$DataPath = 'C:\AppData\DocuArchiApi'
$AppPool = 'DocuArchiApiPool'
$SiteName = 'Default Web Site'
$AppName = 'DocuArchiApi'
```

## Requisitos previos en el servidor IIS

Antes de ejecutar el flujo, el servidor debe cumplir estos prerrequisitos.

### 1. IIS instalado

Debe existir IIS con consola de administración y módulos HTTP básicos.

Validación:

```powershell
Get-WindowsFeature Web-Server,Web-Mgmt-Console
```

Instalación de referencia:

```powershell
Install-WindowsFeature Web-Server,Web-Common-Http,Web-Default-Doc,Web-Static-Content,Web-Http-Errors,Web-Http-Redirect,Web-Health,Web-Http-Logging,Web-Performance,Web-Stat-Compression,Web-Security,Web-Filtering,Web-ISAPI-Ext,Web-ISAPI-Filter,Web-Mgmt-Tools,Web-Mgmt-Console
```

### 2. ASP.NET Core Hosting Bundle

Debe estar instalado el Hosting Bundle compatible con la versión de la aplicación para habilitar `AspNetCoreModuleV2`.

Para este proyecto, la referencia actual es instalar el Hosting Bundle de **.NET 10**.

Descarga oficial:

- https://dotnet.microsoft.com/en-US/download/dotnet/10.0

En esa página buscar:

- `ASP.NET Core Runtime`
- `Hosting Bundle`
- `Windows x64`

El instalador suele tener un nombre parecido a:

- `dotnet-hosting-10.0.x-win.exe`

Validación:

```powershell
Get-ChildItem 'C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App'
```

Si no está, IIS puede levantar el sitio pero fallar al cargar la app publicada.

### 3. `dotnet` disponible

Necesario si el servidor va a ejecutar `opsxdeploy:publish-package` con `-ProjectPath` o si va a generar publish localmente.

Validación:

```powershell
dotnet --info
```

### 4. Node.js y npm

Necesarios para ejecutar el wrapper:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package ...
```

Validación:

```powershell
node --version
npm --version
```

### 5. PowerShell con `WebAdministration`

Necesario para automatizar App Pool, sitios, aplicaciones y bindings.

Validación:

```powershell
Import-Module WebAdministration
Get-Website
```

### 6. Certificado SSL si se usa HTTPS

Si el sitio se expondrá por `https://docuarchi.local` o similar, debe existir:

- certificado en `Cert:\LocalMachine\My`
- binding HTTPS en IIS
- asociación en `http.sys`

Validaciones:

```powershell
Get-ChildItem Cert:\LocalMachine\My | Select-Object Subject,Thumbprint,NotAfter
Get-WebBinding | Select-Object protocol,bindingInformation
netsh http show sslcert hostnameport=docuarchi.local:443
```

### 7. Carpetas operativas con permisos

Deben existir o poder crearse:

- `C:\inetpub\DocuArchiApi`
- `C:\AppData\DocuArchiApi\temp`
- `C:\AppData\DocuArchiApi\uploads`
- `C:\AppData\DocuArchiApi\avatars`
- `C:\AppData\DocuArchiApi\exports`
- `C:\AppData\DocuArchiApi\logs`

El flujo `opsxdeploy:prepare` las crea.

### 8. Permisos NTFS

La identidad `IIS AppPool\DocuArchiApiPool` debe tener:

- `RX` sobre el sitio
- `M` sobre la carpeta de datos

### 9. Conectividad a base de datos

Si la app usa MySQL, el servidor debe poder alcanzar el host y puerto configurados.

Validación local típica:

```powershell
Test-NetConnection localhost -Port 3306
```

### 10. Resolución DNS o hosts

Si se usa `docuarchi.local`, debe resolver al servidor correcto.

Validación:

```powershell
Resolve-DnsName docuarchi.local
```

## Paso 1. Preparar carpetas del servidor

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:prepare -- -SitePath C:\inetpub\DocuArchiApi -DataPath C:\AppData\DocuArchiApi
```

## Paso 2. Generar paquete limpio listo para IIS

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -OutputPath C:\publis\publis-ready
```

## Paso 3. Validar el paquete final

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:doctor -- -PublishPath C:\publis\publis-ready
```

## Paso 4. Respaldar el sitio actual

```powershell
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
Copy-Item C:\inetpub\DocuArchiApi C:\inetpub\DocuArchiApi-backup-$timestamp -Recurse -Force
```

## Paso 5. Detener App Pool

```powershell
Import-Module WebAdministration
Stop-WebAppPool -Name 'DocuArchiApiPool'
```

## Paso 6. Reemplazar el contenido del sitio

```powershell
Get-ChildItem C:\inetpub\DocuArchiApi -Force | Remove-Item -Recurse -Force
Copy-Item C:\publis\publis-ready\* C:\inetpub\DocuArchiApi -Recurse -Force
```

## Paso 7. Crear o validar App Pool

```powershell
Import-Module WebAdministration

if (-not (Test-Path IIS:\AppPools\DocuArchiApiPool)) {
    New-Item IIS:\AppPools\DocuArchiApiPool
}

Set-ItemProperty IIS:\AppPools\DocuArchiApiPool -Name managedRuntimeVersion -Value ''
Set-ItemProperty IIS:\AppPools\DocuArchiApiPool -Name managedPipelineMode -Value 'Integrated'
Start-WebAppPool -Name 'DocuArchiApiPool'
```

## Paso 8. Crear o validar la app IIS

```powershell
Import-Module WebAdministration

if (-not (Get-WebApplication -Site 'Default Web Site' -Name 'DocuArchiApi' -ErrorAction SilentlyContinue)) {
    New-WebApplication -Site 'Default Web Site' -Name 'DocuArchiApi' -PhysicalPath 'C:\inetpub\DocuArchiApi' -ApplicationPool 'DocuArchiApiPool'
}
```

## Paso 9. Aplicar permisos

Lectura y ejecución al sitio:

```powershell
icacls C:\inetpub\DocuArchiApi /grant "IIS AppPool\DocuArchiApiPool:(OI)(CI)(RX)" /T
```

Modificación al storage:

```powershell
icacls C:\AppData\DocuArchiApi /grant "IIS AppPool\DocuArchiApiPool:(OI)(CI)(M)" /T
```

## Paso 10. Reciclar App Pool

```powershell
Import-Module WebAdministration
Restart-WebAppPool -Name 'DocuArchiApiPool'
```

## Paso 11. Validar arranque

```powershell
Invoke-WebRequest https://docuarchi.local/DocuArchiApi/swagger -UseBasicParsing
```

## Paso 12. Validar CORS preflight

```powershell
$headers = @{
  Origin = 'http://localhost:5173'
  'Access-Control-Request-Method' = 'GET'
  'Access-Control-Request-Headers' = 'content-type,authorization'
}

Invoke-WebRequest 'https://docuarchi.local/DocuArchiApi/api/accout/SolicitaEstructuraEmpresa?page=1&pageSize=1' -Method Options -Headers $headers -UseBasicParsing
```

## Resultado esperado

- `swagger` responde `200`
- el `OPTIONS` responde `204`
- existe header:

```text
Access-Control-Allow-Origin: http://localhost:5173
```

## Diagnóstico rápido

Validar estructura del sitio:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:doctor -- -PublishPath C:\inetpub\DocuArchiApi
```

Validar `web.config`:

```powershell
[xml](Get-Content C:\inetpub\DocuArchiApi\web.config -Raw) | Out-Null
```

Ver primeras líneas:

```powershell
Get-Content C:\inetpub\DocuArchiApi\web.config -TotalCount 3 | ForEach-Object { '[' + $_ + ']' }
```

Verificar App Pool:

```powershell
Import-Module WebAdministration
Get-Item IIS:\AppPools\DocuArchiApiPool | Select-Object Name,State,managedRuntimeVersion,managedPipelineMode
```

## Regla operativa

No copiar `publish` bruto a IIS.

Siempre copiar la salida final de:

```text
publish-package -> OutputPath
```

En este entorno, la carpeta correcta fue:

```text
C:\publis\publis-ready
```
