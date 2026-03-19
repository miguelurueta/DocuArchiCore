## 1. Discovery and Contract

- [x] 1.1 Confirmar la interfaz final del comando (`-ProjectPath` vs comando nuevo) y documentar compatibilidad con `-PublishPath`.
- [x] 1.2 Identificar en `DocuArchi.Api` por que el publish incluye `appsettings.Development.json` y `Tools/**`.
- [x] 1.3 Definir el set inicial de claves sensibles que el paquete final debe sanear sin tocar el source repo.

## 2. Tool Implementation

- [x] 2.1 Extender `Tools/iis-deploy/opsxdeploy.ps1` para soportar `publish-package` desde `-ProjectPath`.
- [x] 2.2 Implementar carpeta de staging temporal para `dotnet publish` interno.
- [x] 2.3 Implementar sanitizacion de artefactos no productivos antes de construir el paquete final.
- [x] 2.4 Implementar saneamiento controlado de `appsettings.json` en la salida final.
- [x] 2.5 Mantener validacion estricta del paquete final y compatibilidad con el modo actual `-PublishPath`.

## 3. Verification and Docs

- [x] 3.1 Actualizar `Tools/iis-deploy/test-opsxdeploy-flow.ps1` con casos de publish desde proyecto y sanitizacion.
- [x] 3.2 Actualizar `Tools/iis-deploy/README.md` con ejemplos reales de `DocuArchi.Api`.
- [x] 3.3 Actualizar `Docs/Publicacion/IIS-DocuArchiApi.md` para reflejar el nuevo flujo end-to-end.
- [x] 3.4 Ejecutar validaciones locales relevantes (`dotnet publish`, `opsxdeploy:doctor`, `opsxdeploy:publish-package`) y documentar evidencia.
