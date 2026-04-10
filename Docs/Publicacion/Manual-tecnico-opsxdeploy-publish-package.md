# Manual Tecnico de `opsxdeploy:publish-package`

## Objetivo

`opsxdeploy:publish-package` genera un paquete limpio y orientado a IIS a partir de:

- una carpeta de publish ya existente, o
- un proyecto `.csproj` desde el cual el tool ejecuta `dotnet publish`

El comando **no despliega directamente en IIS**. Su responsabilidad es dejar una carpeta final lista para copiar al sitio IIS, con validaciones estructurales, saneamiento de configuracion y manejo controlado de `web.config`.

## Ubicacion y punto de entrada

Script principal:
- [opsxdeploy.ps1](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Tools/iis-deploy/opsxdeploy.ps1)

Wrapper npm:
- [package.json](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Tools/iis-deploy/package.json)

Comando:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- <argumentos>
```

Internamente ejecuta:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ./opsxdeploy.ps1 publish-package
```

## Contrato de entrada

Parametros relevantes:

- `-PublishPath <ruta>`
- `-ProjectPath <ruta.csproj>`
- `-OutputPath <ruta>`
- `-ProjectConfiguration <config>`
- `-WhatIf`
- `-AllowSecrets`

### Reglas de entrada

1. `-OutputPath` es obligatorio.
2. Debe usarse **exactamente uno** entre `-PublishPath` y `-ProjectPath`.
3. Si faltan ambos, el comando falla.
4. Si se entregan ambos, el comando falla.

## Modos de operacion

### 1. Modo `PublishPath`

Entrada:
- una carpeta publish ya generada, por ejemplo con Visual Studio o `dotnet publish`

Ejemplo:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\SalidaApiCore -OutputPath C:\Entrega\DocuArchiApi-ready
```

Uso recomendado:
- cuando ya existe una salida de publicación que se quiere limpiar y convertir en paquete IIS

### 2. Modo `ProjectPath`

Entrada:
- un `.csproj`

Ejemplo:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -OutputPath C:\Entrega\DocuArchiApi-ready
```

Comportamiento:
- resuelve la ruta del proyecto
- crea una carpeta temporal de staging bajo `%TEMP%`
- ejecuta:

```powershell
dotnet publish <ProjectPath> -c <ProjectConfiguration> -o <stagingPath>
```

- usa ese staging como fuente real del paquete final
- elimina el staging al terminar, salvo error o simulacion

Configuracion por defecto:
- `ProjectConfiguration = Release`

## Flujo interno de `publish-package`

La funcion principal es `Invoke-PublishPackage`.

Orden real de ejecucion:

1. valida argumentos
2. resuelve entrada (`PublishPath` o `ProjectPath`)
3. si usa `ProjectPath`, ejecuta `dotnet publish`
4. ejecuta precheck sobre la fuente
5. recrea `OutputPath`
6. copia el publish excluyendo artefactos prohibidos
7. sanea `appsettings.json`
8. asegura `web.config`
9. ejecuta `doctor` sobre la salida final
10. informa `Publish package ready: <OutputPath>`

## Precheck de la fuente

Antes de armar el paquete final, el comando inspecciona la carpeta fuente con `Get-PublishValidationItems`.

### Archivos obligatorios

La fuente debe contener al menos:

- `*.dll`
- `*.deps.json`
- `*.runtimeconfig.json`

Si falta alguno, el comando falla.

### Artefactos prohibidos

Se consideran prohibidos:

- `appsettings.Development.json`
- cualquier archivo bajo `Tools\`

### Secretos detectados

Se buscan patrones obvios en `appsettings.json`:

- `Jwt.Key` poblado
- cualquier campo `Secret` poblado
- connection strings con `pwd` o `password`
- connection strings con `uid=root`

### Regla especial del precheck

En `publish-package`, el precheck **tolera temporalmente**:

- artefactos prohibidos
- hallazgos de secretos

pero **no tolera**:

- faltantes estructurales de runtime
- `web.config` inválido
- `web.config` sin `processPath`
- `web.config` sin `arguments`

Eso permite que el tool sanee el paquete final, pero bloquea fuentes estructuralmente rotas.

## Copia de archivos al paquete final

La funcion `Copy-PublishPackage` copia recursivamente todos los archivos del publish, excepto:

- `appsettings.Development.json`
- cualquier contenido bajo el directorio raíz `Tools`

Todo lo demás se conserva.

## Saneamiento de `appsettings.json`

La funcion `Protect-PackageAppSettings` modifica **solo** el `appsettings.json` del paquete final.

No modifica:
- el repo
- el proyecto fuente
- la carpeta publish de origen

### Reemplazos actuales

1. `Jwt.Key` -> `__SET_IN_IIS__`
2. cualquier campo `Secret` -> `__SET_IN_IIS__`
3. cualquier `ConnectionStrings.MySqlConnection_*` -> `__SET_IN_IIS__`

### Alcance real

El saneamiento es por patrones regex. No es un motor semantico general de secretos. Cubre los casos conocidos definidos hoy en el script.

## Manejo de `web.config`

La funcion `Ensure-PackageWebConfig` tiene dos ramas.

### Caso A. El publish fuente **ya trae** `web.config`

Comportamiento:
- copia el `web.config` existente al paquete final
- no lo reescribe completo
- lo abre como XML
- valida estructura mínima
- agrega `environmentVariables` si falta
- agrega placeholders faltantes si el bloque existe pero está incompleto

### Caso B. El publish fuente **no trae** `web.config`

Comportamiento:
- deriva el nombre del ensamblado principal
- genera un `web.config` base con:
  - `AspNetCoreModuleV2`
  - `processPath="dotnet"`
  - `arguments=".\<Assembly>.dll"`
  - bloque `environmentVariables` completo

## Estructura mínima exigida para `web.config`

La validación `Test-WebConfigMinimumStructure` exige:

1. XML válido
2. nodo raíz `<configuration>`
3. `<system.webServer>`
4. `<aspNetCore>`
5. atributo `processPath`
6. atributo `arguments`

El bloque `environmentVariables` no es obligatorio para pasar toda inspección estructural básica del publish fuente, pero:
- si no existe, se reporta `warn`
- en el paquete final, `publish-package` intenta dejarlo completo

## Variables esperadas en `environmentVariables`

El tool considera esperadas exactamente estas:

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

Valor por defecto al generarlas:

```text
__SET_IN_IIS__
```

## Validacion final del paquete

Al terminar el armado, `publish-package` ejecuta `Invoke-Doctor` sobre `OutputPath`.

Eso garantiza que la salida final:

- contiene runtime mínimo
- no contiene `appsettings.Development.json`
- no contiene `Tools\`
- no conserva secretos obvios en `appsettings.json`
- tiene `web.config` estructuralmente válido

Si esta validación falla, el comando termina con error.

## Ejemplos operativos

### Empaquetar desde carpeta publish

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\publis\publis -OutputPath C:\publis\publis-ready
```

### Empaquetar directamente desde proyecto

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -OutputPath C:\publis\publis-ready
```

### Cambiar configuración de build

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -ProjectPath D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -ProjectConfiguration Debug -OutputPath C:\publis\publis-ready
```

### Simulación

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\publis\publis -OutputPath C:\publis\publis-ready -WhatIf
```

## Salidas esperadas

Mensajes frecuentes:

- `Using project source: ...`
- `Staging publish path: ...`
- `[PASS] Sanitized Jwt.Key.`
- `[PASS] Sanitized Secret field.`
- `[PASS] Sanitized MySQL connection string.`
- `[PASS] Added environmentVariables block to existing web.config.`
- `opsxdeploy web.config report`
- `opsxdeploy doctor report`
- `Publish package ready: ...`

## Criterios de “paquete listo”

Un paquete debe considerarse listo para copiar a IIS cuando:

1. `publish-package` terminó sin error
2. `doctor` del paquete final pasó
3. `web.config` existe y es XML válido
4. `processPath` y `arguments` están configurados
5. los placeholders o valores reales de `environmentVariables` están definidos correctamente para el ambiente

## Limitaciones y comportamiento observado

### 1. No despliega a IIS

`publish-package` solo construye la carpeta final. La copia al sitio IIS sigue siendo manual o delegada a otro paso.

### 2. `-WhatIf` con `-ProjectPath` tiene una limitación real

En el comportamiento actual, `-WhatIf` en combinación con `-ProjectPath` informa el `dotnet publish`, pero no genera staging real. Luego, si la lógica necesita derivar el ensamblado desde esa carpeta temporal vacía, la simulación puede fallar.

En la práctica:

```text
ProjectPath + WhatIf no representa hoy una simulacion completamente confiable del flujo completo.
```

Para inspección segura:
- usar `-WhatIf` con `-PublishPath`, o
- ejecutar el flujo real con `-ProjectPath`

### 3. El saneamiento no reemplaza toda configuración sensible posible

Solo cubre los patrones codificados hoy. Si aparecen nuevos secretos fuera de esos patrones, el tool no necesariamente los neutraliza.

### 4. Si el `web.config` fuente es XML inválido, falla

Ejemplo real observado:
- líneas o espacios antes de `<?xml ...?>`

Eso hace que:
- `doctor` falle
- `publish-package` también falle si usa ese `web.config` como fuente

## Recomendación operativa

Para `DocuArchi.Api`, el flujo más robusto es:

1. generar publish o usar `-ProjectPath`
2. ejecutar `publish-package`
3. revisar el `web.config` final del paquete
4. completar o reemplazar `__SET_IN_IIS__`
5. copiar el contenido al sitio IIS
6. reciclar el app pool
7. validar `swagger` y el `OPTIONS` real del endpoint

## Caso aplicado a `DocuArchi.Api`

Ejemplo funcional usado en este entorno:

```powershell
dotnet publish D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\DocuArchi.Api.csproj -c Release -o C:\publis\publis
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\publis\publis -OutputPath C:\publis\publis-ready
```

Resultado:
- `C:\publis\publis` = publish bruto
- `C:\publis\publis-ready` = paquete limpio listo para IIS

## Relacion con otra documentacion

Manual operativo general:
- [IIS-DocuArchiApi.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Docs/Publicacion/IIS-DocuArchiApi.md)

README del tool:
- [README.md](D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore/Tools/iis-deploy/README.md)

## Conclusión

`opsxdeploy:publish-package` no es un simple empaquetador. En su comportamiento actual:

- valida estructura mínima
- sanea configuración
- normaliza `web.config`
- genera una salida final auditable con `doctor`

Su propósito correcto es este:

```text
convertir un publish bruto o un proyecto fuente
en una carpeta final apta para ser copiada a IIS
con menor riesgo operativo y menor exposición de configuración sensible
```
