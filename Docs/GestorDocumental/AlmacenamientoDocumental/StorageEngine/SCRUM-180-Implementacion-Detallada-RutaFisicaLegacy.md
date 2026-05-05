# SCRUM-180 — Implementación Detallada Ruta Física Legacy

## Resumen de Cambios
- Se agregó repositorio de ruta legacy en `MiApp.Repository`.
- Se agregó servicio de resolución física legacy en `MiApp.Services`.
- Se reemplazó el cálculo de ruta final en `StoragePlanBuilder`.
- Se movió la resolución absoluta segura al `FileWriter`/`XmlWriter`.

## Cambios por Capa

### MiApp.Models
- `.../Physical/StorageRouteModel.cs`
- `.../Physical/StorageFolderResult.cs`
- `.../Physical/StoragePhysicalPathModel.cs`

### MiApp.Repository
- `.../StorageRoute/StorageRouteRepository.cs`
  - Consulta `system1rut` con filtros `gabinete`, `tipo_rut=1`, `est_rut=1`.
  - Mapea `ruta_gabi` -> `RutaAlmacenamiento`.

### MiApp.Services
- `IStorageFolderLegacyPolicy` / `StorageFolderLegacyPolicy`
  - Construye `{root}\{gabinete}{disco}\{carpetaLegacy}`.
  - `carpetaLegacy` con formato `D5`.
- `IStoragePhysicalPathService` / `StoragePhysicalPathService`
  - Obtiene root real desde DB.
  - Resuelve carpeta legacy.
  - Aplica hardening con `StoragePathResolver`.
- `IStoragePathResolver` / `StoragePathResolver`
  - Reemplaza APIs de root/folder por `ResolveSafePath`.
  - Conserva `GetTemporaryFilePath` para staging temporal.
- `StoragePlanBuilder`
  - Usa `IStoragePhysicalPathService` para `StorageRoot` y ruta final.
  - Guarda `RutaFinal` relativa a root.
- `StorageFileWriter` y `StorageXmlWriter`
  - Convierten `StorageRoot + RutaFinal` en ruta absoluta segura antes de IO.

### DocuArchi.Api (DI)
- Registro de:
  - `IStorageRouteRepository`
  - `IStorageFolderLegacyPolicy`
  - `IStoragePhysicalPathService`

## Compatibilidad con Prompt 9/13
- Separación preservada:
  - `StorageNamingService` -> nombre archivo.
  - `StoragePhysicalPathService` -> ruta física.
  - Writers -> copia y XML.

## Riesgos Atendidos
- Path traversal.
- Dependencia de rutas temporales no legacy.
- Inconsistencia visor legacy por estructura de carpetas.
