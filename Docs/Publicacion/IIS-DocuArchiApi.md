# Manual de Publicacion en IIS

## DocuArchi.Api

### Objetivo

Publicar `DocuArchi.Api` en IIS con configuracion separada por entorno, sin dejar secretos en el repositorio y sin hacer obligatorio un despliegue automatico al sitio.

### 1. Prerrequisitos

- IIS instalado.
- ASP.NET Core Hosting Bundle compatible con la version de la app.
- Permisos de lectura sobre la carpeta del sitio.
- Permisos de escritura sobre carpetas operativas.

### 2. Publicacion desde Visual Studio

1. Abrir `DocuArchi.Api`.
2. Seleccionar `Publish`.
3. Elegir perfil `Folder`.
4. Publicar a una carpeta local, por ejemplo:

```text
C:\SalidaApiCore
```

5. Confirmar que la publicacion contiene:
- `DocuArchi.Api.dll`
- `DocuArchi.Api.deps.json`
- `DocuArchi.Api.runtimeconfig.json`
- `web.config`

### 3. Validacion del publish

Ejecutar:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:doctor -- -PublishPath C:\SalidaApiCore
```

El publish no debe incluir:
- `appsettings.Development.json`
- `Tools\...`
- secretos reales en `appsettings.json`

### 4. Preparar estructura del servidor

Ejecutar:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:prepare -- -SitePath C:\inetpub\DocuArchiApi -DataPath C:\AppData\DocuArchiApi
```

Esto crea:
- `C:\inetpub\DocuArchiApi`
- `C:\AppData\DocuArchiApi\temp`
- `C:\AppData\DocuArchiApi\uploads`
- `C:\AppData\DocuArchiApi\avatars`
- `C:\AppData\DocuArchiApi\exports`
- `C:\AppData\DocuArchiApi\logs`

### 5. Generar paquete listo para IIS

Ejecutar:

```powershell
npm.cmd --prefix Tools/iis-deploy run opsxdeploy:publish-package -- -PublishPath C:\SalidaApiCore -OutputPath C:\Entrega\DocuArchiApi-ready
```

Este paso:
- valida de nuevo la publicacion
- excluye `appsettings.Development.json`
- excluye `Tools\`
- deja una carpeta limpia lista para copiar al servidor

### 6. Crear App Pool y sitio IIS

Configurar manualmente en IIS:

- App Pool:
  - `Name`: `DocuArchiApiPool`
  - `.NET CLR Version`: `No Managed Code`
  - `Pipeline`: `Integrated`

- Sitio:
  - `Site name`: `DocuArchiApi`
  - `Physical path`: `C:\inetpub\DocuArchiApi`
  - binding segun ambiente

### 7. Configuracion por variables de entorno

No dejar secretos reales en `appsettings.json`.

Definir en `web.config` o en el sitio IIS:
- `ASPNETCORE_ENVIRONMENT=Production`
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

### 8. Permisos

La identidad del App Pool debe tener:
- lectura y ejecucion sobre `C:\inetpub\DocuArchiApi`
- `Modify` sobre `C:\AppData\DocuArchiApi`

### 9. Prueba inicial

1. Copiar el paquete limpio a `C:\inetpub\DocuArchiApi`.
2. Iniciar el sitio en IIS.
3. Probar `swagger` o el endpoint principal.
4. Validar acceso a MySQL y escritura en carpetas operativas.

### 10. Diagnostico

Si la app no arranca:
- habilitar temporalmente `stdoutLogEnabled="true"` en `web.config`
- crear `logs` bajo el sitio
- revisar errores de arranque

### 11. Checklist final

- publish validado
- carpeta limpia lista para IIS
- `appsettings.Development.json` excluido
- secretos fuera del repo
- App Pool en `No Managed Code`
- sitio creado
- permisos aplicados
- rutas operativas creadas
- app responde
