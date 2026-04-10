# Manual Tecnico Completo de `opsxdeploy:publish-package`

## 1. Proposito

`opsxdeploy:publish-package` es el comando del tooling `opsxdeploy` encargado de producir una carpeta final lista para IIS a partir de:

- un `publish` bruto ya generado, o
- un proyecto `.csproj` desde el cual el tool ejecuta `dotnet publish`

El objetivo del comando es reducir errores manuales en despliegue, remover artefactos no productivos, sanear configuracion sensible y dejar un `web.config` consistente para hospedaje en IIS con `AspNetCoreModuleV2`.

No despliega directamente a IIS. Su salida es una carpeta lista para copiar al sitio.

## 2. Ubicacion tecnica

Script principal:
- [opsxdeploy.ps1](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Tools/iis-deploy/opsxdeploy.ps1)

Wrapper npm:
- [package.json](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Tools/iis-deploy/package.json)

Documentacion base:
- [README.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Tools/iis-deploy/README.md)
- [IIS-DocuArchiApi.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Publicacion/IIS-DocuArchiApi.md)
- [Manual-tecnico-opsxdeploy-publish-package.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Publicacion/Manual-tecnico-opsxdeploy-publish-package.md)

## 3. Componentes necesarios en el servidor

Antes de usar el paquete final en IIS, el servidor debe tener instalados o configurados estos componentes.

### 3.1 Windows e IIS

Componentes mínimos recomendados:

- `Web-Server`
- `Web-Common-Http`
- `Web-Default-Doc`
- `Web-Static-Content`
- `Web-Http-Errors`
- `Web-Http-Redirect`
- `Web-Health`
- `Web-Http-Logging`
- `Web-Performance`
- `Web-Stat-Compression`
- `Web-Security`
- `Web-Filtering`
- `Web-Windows-Auth` si el entorno lo requiere
- `Web-App-Dev`
- `Web-Net-Ext45`
- `Web-Asp-Net45`
- `Web-ISAPI-Ext`
- `Web-ISAPI-Filter`
- `Web-Mgmt-Tools`
- `Web-Mgmt-Console`

Comando PowerShell de referencia:

```powershell
Install-WindowsFeature Web-Server,Web-Common-Http,Web-Default-Doc,Web-Static-Content,Web-Http-Errors,Web-Http-Redirect,Web-Health,Web-Http-Logging,Web-Performance,Web-Stat-Compression,Web-Security,Web-Filtering,Web-ISAPI-Ext,Web-ISAPI-Filter,Web-Mgmt-Tools,Web-Mgmt-Console
```

### 3.2 Hosting Bundle de ASP.NET Core

Debe instalarse el Hosting Bundle compatible con la versión de la aplicación para que IIS pueda cargar `AspNetCoreModuleV2`.

Para este proyecto, la referencia actual es instalar el Hosting Bundle de **.NET 10**.

Descarga oficial:

- https://dotnet.microsoft.com/en-US/download/dotnet/10.0

En esa página buscar:

- `ASP.NET Core Runtime`
- `Hosting Bundle`
- `Windows x64`

El instalador suele llamarse parecido a:

- `dotnet-hosting-10.0.x-win.exe`

Validación rápida:

```powershell
Get-ChildItem 'C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App'
```

Si el runtime/hosting bundle no está, IIS puede devolver errores de arranque aunque el sitio y el `web.config` sean correctos.

### 3.3 .NET SDK o Runtime

Para ejecutar `opsxdeploy:publish-package` con `-ProjectPath` en el mismo servidor, se necesita `dotnet` disponible en PATH.

Validación:

```powershell
dotnet --info
```

### 3.4 Node.js y npm

Se necesitan para usar el wrapper:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package ...
```

Validación:

```powershell
node --version
npm --version
```

### 3.5 Certificado SSL si se usa HTTPS

Para un host como `docuarchi.local` se requiere:

- certificado en `Cert:\LocalMachine\My`
- binding HTTPS correcto en IIS
- registro `http.sys` para el `hostname:port`

Validaciones útiles:

```powershell
Get-ChildItem Cert:\LocalMachine\My
netsh http show sslcert hostnameport=docuarchi.local:443
```

### 3.6 Base de datos y conectividad

Si la aplicación usa MySQL, el servidor debe poder resolver y alcanzar el host configurado en:

- `ConnectionStrings__MySqlConnection_DA`
- `ConnectionStrings__MySqlConnection_WFR`
- `ConnectionStrings__MySqlConnection_WF`

Validación mínima:

```powershell
Test-NetConnection localhost -Port 3306
```

### 3.7 Carpetas operativas

El servidor debe tener o poder crear:

- `C:\inetpub\DocuArchiApi`
- `C:\AppData\DocuArchiApi\temp`
- `C:\AppData\DocuArchiApi\uploads`
- `C:\AppData\DocuArchiApi\avatars`
- `C:\AppData\DocuArchiApi\exports`
- `C:\AppData\DocuArchiApi\logs`

`opsxdeploy:prepare` cubre esta parte.

## 4. Comando exacto

Desde la raíz del repositorio:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- <argumentos>
```

Internamente el wrapper ejecuta:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ./opsxdeploy.ps1 publish-package
```

## 5. Firma funcional

Los parámetros disponibles en el script son:

- `-PublishPath`
- `-ProjectPath`
- `-OutputPath`
- `-ProjectConfiguration`
- `-WhatIf`
- `-AllowSecrets`

## 6. Explicacion tecnica de cada parametro

### 5.1 `-PublishPath`

Tipo:
- `string`

Uso:
- ruta a una carpeta publish ya generada

Ejemplo:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\publis\publis -OutputPath C:\publis\publis-ready
```

Cuándo usarlo:
- cuando ya hiciste `dotnet publish`
- cuando Visual Studio ya generó una carpeta `Folder Publish`
- cuando quieres limpiar y convertir una salida existente en paquete final para IIS

Regla:
- no puede usarse al mismo tiempo que `-ProjectPath`

### 5.2 `-ProjectPath`

Tipo:
- `string`

Uso:
- ruta a un `.csproj`

Ejemplo:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -OutputPath C:\publis\publis-ready
```

Comportamiento:
- el tool resuelve la ruta
- crea una carpeta temporal de staging
- ejecuta `dotnet publish`
- usa esa salida como fuente

Regla:
- no puede usarse al mismo tiempo que `-PublishPath`

Observación real:
- si usas ruta relativa, la resolución ocurre desde `Tools/iis-deploy`
- para evitar ambigüedad, conviene usar ruta absoluta

### 5.3 `-OutputPath`

Tipo:
- `string`

Uso:
- carpeta donde se genera el paquete final listo para IIS

Ejemplo:

```powershell
-OutputPath C:\publis\publis-ready
```

Regla:
- es obligatorio

Comportamiento:
- si existe, el tool la elimina y la recrea
- luego copia la salida limpia final

### 5.4 `-ProjectConfiguration`

Tipo:
- `string`

Valor por defecto:
- `Release`

Uso:
- define la configuración usada por `dotnet publish` cuando se usa `-ProjectPath`

Ejemplo:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\...\DocuArchi.Api.csproj -ProjectConfiguration Debug -OutputPath C:\publis\debug-ready
```

### 5.5 `-WhatIf`

Tipo:
- `switch`

Uso:
- simula acciones en vez de ejecutarlas

Ejemplo:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\publis\publis -OutputPath C:\publis\publis-ready -WhatIf
```

Limitación observada:
- `-ProjectPath + -WhatIf` no representa bien todo el flujo actual
- la simulación puede fallar al intentar derivar el ensamblado desde un staging no generado

### 5.6 `-AllowSecrets`

Tipo:
- `switch`

Uso:
- suaviza el nivel de algunos hallazgos de secretos durante validación

Importante:
- `publish-package` igual sanea el paquete final
- este switch no debe confundirse con “permitir producción insegura”

## 7. Reglas duras del comando

1. `-OutputPath` es obligatorio
2. debes usar uno y solo uno entre:
   - `-PublishPath`
   - `-ProjectPath`
3. si falta la fuente, el comando falla
4. si sobran ambas fuentes, el comando falla

## 8. Flujo interno real

La función principal es `Invoke-PublishPackage`.

Secuencia:

1. valida argumentos
2. resuelve la fuente
3. si la fuente es proyecto, ejecuta `Invoke-ProjectPublish`
4. ejecuta precheck estructural del publish fuente
5. recrea la carpeta de salida
6. copia archivos con exclusiones
7. sanea `appsettings.json`
8. asegura `web.config`
9. ejecuta `doctor` sobre la carpeta final
10. reporta que el paquete está listo

## 9. Precheck de la fuente

Antes de construir el paquete final, el tool inspecciona la fuente.

### 8.1 Archivos obligatorios

Debe existir al menos:

- `*.dll`
- `*.deps.json`
- `*.runtimeconfig.json`

Si falta alguno, el comando falla.

### 8.2 Artefactos prohibidos

Se marcan como prohibidos:

- `appsettings.Development.json`
- cualquier archivo bajo `Tools\`

### 8.3 Hallazgos de secretos

Busca en `appsettings.json`:

- `Jwt.Key`
- cualquier campo `Secret`
- `pwd=` o `password=`
- `uid=root`

### 8.4 Qué bloquea realmente

`publish-package` tolera temporalmente en la fuente:
- artefactos prohibidos
- secretos

pero no tolera:
- faltantes de runtime
- `web.config` inválido
- `processPath` faltante
- `arguments` faltante

## 10. Copia del paquete final

La función `Copy-PublishPackage` copia todo el árbol del publish salvo:

- `appsettings.Development.json`
- el directorio raíz `Tools`

Esto significa que el paquete final:
- sigue manteniendo la estructura del publish
- pero elimina lo que no debe llegar a IIS

## 11. Saneamiento de `appsettings.json`

La función `Protect-PackageAppSettings` modifica solo el `appsettings.json` de la salida final.

### 10.1 Qué reemplaza

1. `Jwt.Key` -> `__SET_IN_IIS__`
2. `Secret` -> `__SET_IN_IIS__`
3. `ConnectionStrings.MySqlConnection_*` -> `__SET_IN_IIS__`

### 10.2 Qué no cambia

- el proyecto fuente
- el repositorio
- el publish bruto

## 12. Manejo de `web.config`

La función `Ensure-PackageWebConfig` cubre dos escenarios.

### 11.1 El publish fuente ya tiene `web.config`

Entonces:
- lo conserva
- valida XML
- valida estructura mínima
- agrega `environmentVariables` si no existe
- agrega variables faltantes si el bloque está incompleto

### 11.2 El publish fuente no tiene `web.config`

Entonces:
- deriva el nombre del ensamblado principal
- genera un archivo base para IIS

## 13. Estructura mínima exigida a `web.config`

El XML debe tener:

1. `<?xml version="1.0" encoding="utf-8"?>` sin espacios antes
2. nodo raíz `<configuration>`
3. `<system.webServer>`
4. `<aspNetCore>`
5. atributo `processPath`
6. atributo `arguments`

Si el XML no es válido, `doctor` y `publish-package` fallan.

## 14. Variables esperadas en el `web.config`

El tool espera estas variables en el bloque `environmentVariables`:

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

Valor placeholder usado por el tool:

```text
__SET_IN_IIS__
```

## 15. Explicacion tecnica de cada variable del `web.config`

### 14.1 `ASPNETCORE_ENVIRONMENT`

Define el ambiente de ejecución.

Valores típicos:
- `Development`
- `Staging`
- `Production`

Ejemplo:

```xml
<environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
```

### 14.2 `ConnectionStrings__MySqlConnection_DA`

Cadena de conexión principal a la base `docuarchi`.

Ejemplo:

```xml
<environmentVariable name="ConnectionStrings__MySqlConnection_DA" value="server=localhost;database=docuarchi;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
```

### 14.3 `ConnectionStrings__MySqlConnection_WFR`

Cadena de conexión a la base `workflowdocument`.

Ejemplo:

```xml
<environmentVariable name="ConnectionStrings__MySqlConnection_WFR" value="server=localhost;database=workflowdocument;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
```

### 14.4 `ConnectionStrings__MySqlConnection_WF`

Cadena de conexión a la base `workflowtconta`.

Ejemplo:

```xml
<environmentVariable name="ConnectionStrings__MySqlConnection_WF" value="server=localhost;database=workflowtconta;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
```

### 14.5 `Jwt__Key`

Clave simétrica usada para firmar y validar JWT.

Ejemplo:

```xml
<environmentVariable name="Jwt__Key" value="DocuArchiCore_JWT_Key_2026_Super_Segura_123456" />
```

### 14.6 `Jwt__Issuer`

Issuer esperado del token JWT.

Ejemplo:

```xml
<environmentVariable name="Jwt__Issuer" value="DocuArchiCore" />
```

### 14.7 `Jwt__Audience`

Audience esperado del token JWT.

Ejemplo:

```xml
<environmentVariable name="Jwt__Audience" value="DocuArchiCore.Client" />
```

### 14.8 `StoragePaths__Temp`

Ruta para archivos temporales de la API.

Ejemplo:

```xml
<environmentVariable name="StoragePaths__Temp" value="C:\AppData\DocuArchiApi\temp" />
```

### 14.9 `StoragePaths__Uploads`

Ruta para archivos cargados al sistema.

Ejemplo:

```xml
<environmentVariable name="StoragePaths__Uploads" value="C:\AppData\DocuArchiApi\uploads" />
```

### 14.10 `StoragePaths__Avatars`

Ruta para imágenes o avatares asociados a usuarios u objetos.

Ejemplo:

```xml
<environmentVariable name="StoragePaths__Avatars" value="C:\AppData\DocuArchiApi\avatars" />
```

### 14.11 `StoragePaths__Exports`

Ruta para exportaciones generadas por la aplicación.

Ejemplo:

```xml
<environmentVariable name="StoragePaths__Exports" value="C:\AppData\DocuArchiApi\exports" />
```

### 14.12 `StoragePaths__Logs`

Ruta lógica de logs de aplicación si el código la usa como storage path.

Ejemplo:

```xml
<environmentVariable name="StoragePaths__Logs" value="C:\AppData\DocuArchiApi\logs" />
```

## 16. Variables de CORS del `web.config` actual

Estas variables **no las genera hoy** `opsxdeploy`, pero sí pueden existir en el `web.config` real del sitio.

En tu caso actual existen:

- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1`
- `Cors__AllowedOrigins__2`

Ejemplos reales:

```xml
<environmentVariable name="Cors__AllowedOrigins__0" value="http://localhost:5174" />
<environmentVariable name="Cors__AllowedOrigins__1" value="https://docuarchi.local" />
<environmentVariable name="Cors__AllowedOrigins__2" value="http://localhost:5173" />
```

Interpretación:
- `5174` para un frontend local
- `docuarchi.local` para consumo por host publicado
- `5173` para otro frontend local

## 17. Ejemplo técnico de `web.config` base que genera el tool

Cuando falta `web.config`, la plantilla base es conceptualmente esta:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\DocuArchi.Api.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="__SET_IN_IIS__" />
          <environmentVariable name="ConnectionStrings__MySqlConnection_DA" value="__SET_IN_IIS__" />
          <environmentVariable name="ConnectionStrings__MySqlConnection_WFR" value="__SET_IN_IIS__" />
          <environmentVariable name="ConnectionStrings__MySqlConnection_WF" value="__SET_IN_IIS__" />
          <environmentVariable name="Jwt__Key" value="__SET_IN_IIS__" />
          <environmentVariable name="Jwt__Issuer" value="__SET_IN_IIS__" />
          <environmentVariable name="Jwt__Audience" value="__SET_IN_IIS__" />
          <environmentVariable name="StoragePaths__Temp" value="__SET_IN_IIS__" />
          <environmentVariable name="StoragePaths__Uploads" value="__SET_IN_IIS__" />
          <environmentVariable name="StoragePaths__Avatars" value="__SET_IN_IIS__" />
          <environmentVariable name="StoragePaths__Exports" value="__SET_IN_IIS__" />
          <environmentVariable name="StoragePaths__Logs" value="__SET_IN_IIS__" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

## 18. Copia exacta de ejemplo del `web.config` actual de IIS

Esta es la copia textual del `web.config` actual del sitio publicado en IIS que se usó como referencia operativa:

```xml
<?xml version="1.0" encoding="utf-8"?>
  <configuration>
    <location path="." inheritInChildApplications="false">
      <system.webServer>
        <handlers>
          <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
        </handlers>
        <aspNetCore processPath="dotnet" arguments=".\DocuArchi.Api.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
          <environmentVariables>
            <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />

            <environmentVariable name="ConnectionStrings__MySqlConnection_DA" value="server=localhost;database=docuarchi;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
            <environmentVariable name="ConnectionStrings__MySqlConnection_WFR" value="server=localhost;database=workflowdocument;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
            <environmentVariable name="ConnectionStrings__MySqlConnection_WF" value="server=localhost;database=workflowtconta;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />

            <environmentVariable name="Jwt__Key" value="DocuArchiCore_JWT_Key_2026_Super_Segura_123456" />
            <environmentVariable name="Jwt__Issuer" value="DocuArchiCore" />
            <environmentVariable name="Jwt__Audience" value="DocuArchiCore.Client" />

            <environmentVariable name="Cors__AllowedOrigins__0" value="http://localhost:5174" />
            <environmentVariable name="Cors__AllowedOrigins__2" value="http://localhost:5173" />
            <environmentVariable name="Cors__AllowedOrigins__1" value="https://docuarchi.local" />

            <environmentVariable name="StoragePaths__Temp" value="C:\AppData\DocuArchiApi\temp" />
            <environmentVariable name="StoragePaths__Uploads" value="C:\AppData\DocuArchiApi\uploads" />
            <environmentVariable name="StoragePaths__Avatars" value="C:\AppData\DocuArchiApi\avatars" />
            <environmentVariable name="StoragePaths__Exports" value="C:\AppData\DocuArchiApi\exports" />
            <environmentVariable name="StoragePaths__Logs" value="C:\AppData\DocuArchiApi\logs" />
          </environmentVariables>
        </aspNetCore>
      </system.webServer>
    </location>
  </configuration>
```

## 19. Ejemplo de `web.config` operativo listo para copiar

Esta es la forma correcta de dejarlo, sin placeholders y sin espacios antes de la declaración XML:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\DocuArchi.Api.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
          <environmentVariable name="ConnectionStrings__MySqlConnection_DA" value="server=localhost;database=docuarchi;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
          <environmentVariable name="ConnectionStrings__MySqlConnection_WFR" value="server=localhost;database=workflowdocument;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
          <environmentVariable name="ConnectionStrings__MySqlConnection_WF" value="server=localhost;database=workflowtconta;uid=root;pwd=5840;Pooling=true;Min Pool Size=5;Max Pool Size=50;" />
          <environmentVariable name="Jwt__Key" value="DocuArchiCore_JWT_Key_2026_Super_Segura_123456" />
          <environmentVariable name="Jwt__Issuer" value="DocuArchiCore" />
          <environmentVariable name="Jwt__Audience" value="DocuArchiCore.Client" />
          <environmentVariable name="Cors__AllowedOrigins__0" value="http://localhost:5174" />
          <environmentVariable name="Cors__AllowedOrigins__1" value="https://docuarchi.local" />
          <environmentVariable name="Cors__AllowedOrigins__2" value="http://localhost:5173" />
          <environmentVariable name="StoragePaths__Temp" value="C:\AppData\DocuArchiApi\temp" />
          <environmentVariable name="StoragePaths__Uploads" value="C:\AppData\DocuArchiApi\uploads" />
          <environmentVariable name="StoragePaths__Avatars" value="C:\AppData\DocuArchiApi\avatars" />
          <environmentVariable name="StoragePaths__Exports" value="C:\AppData\DocuArchiApi\exports" />
          <environmentVariable name="StoragePaths__Logs" value="C:\AppData\DocuArchiApi\logs" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

## 20. Procedimiento técnico completo de implementación en IIS

Esta sección complementa el manual operativo y aterriza el despliegue a un flujo shell paso a paso.

### 20.1 Preparar carpetas con `opsxdeploy`

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:prepare -- -SitePath C:\inetpub\DocuArchiApi -DataPath C:\AppData\DocuArchiApi
```

### 20.2 Generar paquete listo para IIS desde proyecto

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -OutputPath C:\publis\publis-ready
```

### 20.3 Validar el paquete final

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:doctor -- -PublishPath C:\publis\publis-ready
```

### 20.4 Respaldar el sitio actual

```powershell
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
Copy-Item C:\inetpub\DocuArchiApi C:\inetpub\DocuArchiApi-backup-$timestamp -Recurse -Force
```

### 20.5 Detener o reciclar App Pool antes de copiar

```powershell
Import-Module WebAdministration
Stop-WebAppPool -Name 'DocuArchiApiPool'
```

o, si prefieres solo reciclar después:

```powershell
Import-Module WebAdministration
Restart-WebAppPool -Name 'DocuArchiApiPool'
```

### 20.6 Reemplazar archivos del sitio

Opción simple:

```powershell
Copy-Item C:\publis\publis-ready\* C:\inetpub\DocuArchiApi -Recurse -Force
```

Opción recomendada con limpieza previa del sitio, preservando solo si sabes lo que haces:

```powershell
Get-ChildItem C:\inetpub\DocuArchiApi -Force | Remove-Item -Recurse -Force
Copy-Item C:\publis\publis-ready\* C:\inetpub\DocuArchiApi -Recurse -Force
```

### 20.7 Crear o validar App Pool

```powershell
Import-Module WebAdministration

if (-not (Test-Path IIS:\AppPools\DocuArchiApiPool)) {
    New-Item IIS:\AppPools\DocuArchiApiPool
}

Set-ItemProperty IIS:\AppPools\DocuArchiApiPool -Name managedRuntimeVersion -Value ''
Set-ItemProperty IIS:\AppPools\DocuArchiApiPool -Name managedPipelineMode -Value 'Integrated'
Start-WebAppPool -Name 'DocuArchiApiPool'
```

### 20.8 Crear o validar la aplicación IIS bajo Default Web Site

```powershell
Import-Module WebAdministration

if (-not (Get-WebApplication -Site 'Default Web Site' -Name 'DocuArchiApi' -ErrorAction SilentlyContinue)) {
    New-WebApplication -Site 'Default Web Site' -Name 'DocuArchiApi' -PhysicalPath 'C:\inetpub\DocuArchiApi' -ApplicationPool 'DocuArchiApiPool'
}
```

### 20.9 Asignar permisos al App Pool

Lectura y ejecución al sitio:

```powershell
icacls C:\inetpub\DocuArchiApi /grant "IIS AppPool\DocuArchiApiPool:(OI)(CI)(RX)" /T
```

Modificación al storage:

```powershell
icacls C:\AppData\DocuArchiApi /grant "IIS AppPool\DocuArchiApiPool:(OI)(CI)(M)" /T
```

### 20.10 Validar bindings

```powershell
Import-Module WebAdministration
Get-WebBinding | Select-Object protocol,bindingInformation
```

### 20.11 Validar certificado HTTPS

```powershell
Get-ChildItem Cert:\LocalMachine\My | Select-Object Subject,Thumbprint,NotAfter
netsh http show sslcert hostnameport=docuarchi.local:443
```

### 20.12 Reciclar y validar el sitio

```powershell
Import-Module WebAdministration
Restart-WebAppPool -Name 'DocuArchiApiPool'
```

Luego:

```powershell
Invoke-WebRequest https://docuarchi.local/DocuArchiApi/swagger -UseBasicParsing
```

### 20.13 Validar preflight CORS

```powershell
$headers = @{
  Origin = 'http://localhost:5173'
  'Access-Control-Request-Method' = 'GET'
  'Access-Control-Request-Headers' = 'content-type,authorization'
}

Invoke-WebRequest 'https://docuarchi.local/DocuArchiApi/api/accout/SolicitaEstructuraEmpresa?page=1&pageSize=1' -Method Options -Headers $headers -UseBasicParsing
```

### 20.14 Diagnóstico de `web.config`

Validar XML:

```powershell
[xml](Get-Content C:\inetpub\DocuArchiApi\web.config -Raw) | Out-Null
```

Detectar espacios antes de `<?xml`:

```powershell
Get-Content C:\inetpub\DocuArchiApi\web.config -TotalCount 3 | ForEach-Object { '[' + $_ + ']' }
Format-Hex -Path C:\inetpub\DocuArchiApi\web.config | Select-Object -First 4
```

### 20.15 Diagnóstico del sitio con `opsxdeploy:doctor`

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:doctor -- -PublishPath C:\inetpub\DocuArchiApi
```

## 21. Ejemplos operativos completos

### 19.1 Publicar bruto y luego empaquetar

```powershell
dotnet publish D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -c Release -o C:\publis\publis
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\publis\publis -OutputPath C:\publis\publis-ready
```

### 19.2 Empaquetar directo desde proyecto

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -OutputPath C:\publis\publis-ready
```

### 19.3 Validar el paquete final

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:doctor -- -PublishPath C:\publis\publis-ready
```

## 22. Criterio técnico de paquete listo

Un paquete debe considerarse listo para copiar a IIS si:

1. `publish-package` terminó sin error
2. `doctor` de la salida final pasó
3. `web.config` es XML válido
4. `processPath` y `arguments` están definidos
5. `environmentVariables` está completo para el ambiente

## 23. Limitaciones reales

### 21.1 `ProjectPath + WhatIf`

No simula correctamente todo el flujo actual.

### 21.2 El tool no agrega variables de CORS por defecto

Las variables de CORS no están en la lista de placeholders esperados del script. Si el ambiente las necesita, deben agregarse manualmente o venir desde el `web.config` fuente.

### 21.3 No despliega directo a IIS

Solo deja la carpeta lista.

### 21.4 Si el `web.config` fuente es XML inválido, falla

Ejemplo real visto:
- salto de línea o espacios antes de `<?xml ...?>`

## 24. Recomendación final

Para `DocuArchi.Api`, el flujo recomendado es:

1. generar publish bruto o usar `ProjectPath`
2. ejecutar `publish-package`
3. revisar y completar `web.config`
4. copiar la salida final al sitio IIS
5. reciclar el app pool
6. validar `swagger` y el `OPTIONS` real del endpoint

En este entorno, la carpeta correcta resultante fue:

```text
C:\publis\publis-ready
```
