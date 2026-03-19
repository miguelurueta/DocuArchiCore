Summary: OPSXDEPLOY: publicar desde proyecto y sanitizar paquete IIS final

Descripcion:

Se requiere extender `opsxdeploy` para que el flujo de empaquetado IIS pueda ejecutarse directamente desde el proyecto fuente (`.csproj`) sin depender de un `dotnet publish` manual previo, y para que el paquete final quede saneado frente a artefactos no productivos y configuracion sensible.

Contexto observado:
- En la primera prueba operativa, el usuario esperaba ejecutar el flujo de publicacion desde el repositorio/proyecto.
- El publish real de `DocuArchi.Api` siguio incluyendo `appsettings.Development.json`, `Tools/jira-open/**` y valores sensibles en `appsettings.json`.
- Eso hace que `opsxdeploy:doctor` bloquee el flujo antes de obtener un paquete IIS apto.

Objetivo:
- Permitir que `opsxdeploy publish-package` acepte un `.csproj` y ejecute `dotnet publish` internamente.
- Separar el publish intermedio del paquete final listo para IIS.
- Excluir automaticamente artefactos no productivos del paquete final.
- Sanear `appsettings.json` del paquete final para no transportar secretos reales.

Alcance:
- Extender `Tools/iis-deploy/opsxdeploy.ps1`
- Soportar `publish-package` desde `-ProjectPath`
- Mantener compatibilidad con `-PublishPath`
- Agregar staging temporal para `dotnet publish`
- Excluir `appsettings.Development.json`, `Tools/**` y equivalentes del paquete final
- Reemplazar o vaciar claves sensibles conocidas en `appsettings.json` del paquete final
- Actualizar pruebas y documentacion

Reglas funcionales:
- Si el operador entrega `-ProjectPath`, el tool debe ejecutar `dotnet publish` a una carpeta temporal controlada.
- El paquete final no debe contener `appsettings.Development.json`.
- El paquete final no debe contener `Tools/**`.
- El paquete final no debe transportar valores reales en `Jwt.Key`, claves `Secret` ni connection strings con credenciales conocidas.
- El saneamiento debe ocurrir solo en la salida final, sin modificar archivos del repositorio fuente.
- El modo actual con `-PublishPath` debe seguir funcionando.

Criterios de aceptacion:
- `opsxdeploy publish-package` funciona con `-ProjectPath <ruta.csproj>`.
- El comando genera un publish temporal y luego un paquete final listo para IIS.
- El paquete final excluye artefactos no productivos.
- El paquete final no contiene secretos conocidos poblados en `appsettings.json`.
- `opsxdeploy:doctor` y `publish-package` emiten mensajes accionables sobre limpieza/sanitizacion.
- Existen pruebas automatizadas para modo `-ProjectPath` y `-PublishPath`.
- `Tools/iis-deploy/README.md` y `Docs/Publicacion/IIS-DocuArchiApi.md` quedan actualizados.

Repos impactados:
- DocuArchiCore
- DocuArchi.Api
