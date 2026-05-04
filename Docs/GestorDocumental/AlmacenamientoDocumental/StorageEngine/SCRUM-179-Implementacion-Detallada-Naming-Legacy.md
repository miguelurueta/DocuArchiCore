# SCRUM-179 — Implementación Detallada Naming Legacy (DIG/FXL)

## Resumen
Se implementó la paridad de nomenclatura legacy del StorageEngine para eliminar el esquema `alm_{id}` y adoptar el esquema documental histórico:

- Archivo principal: `DIG########.{ext}`
- XML de metadata física: `FXL########.xml`
- Extensión resuelta desde `DA_EXTENSION` por `IdTipoDocumento`

## Archivos creados

### MiApp.Models
- `Models/GestorDocumental/AlmacenamientoDocumental/StorageNamingResult.cs`

### MiApp.Repository
- `Repositorio/GestorDocumental/AlmacenamientoDocumental/Extension/IStorageExtensionRepository.cs`

### MiApp.Services
- `Service/GestorDocumental/AlmacenamientoDocumental/Naming/IStorageExtensionResolver.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/Naming/IStorageNamingService.cs`

### DocuArchiCore (tests)
- `tests/TramiteDiasVencimiento.Tests/StorageNamingServiceTests.cs`

## Archivos modificados

### MiApp.Models
- `Models/GestorDocumental/AlmacenamientoDocumental/StorageFilePlanModel.cs`
  - Nuevos campos: `NombreXml`, `SegundoNombreDocumental`.

### MiApp.Services
- `Service/GestorDocumental/AlmacenamientoDocumental/Builders/IStoragePlanBuilder.cs`
  - Firma migrada a `BuildFilePlanAsync(...)`.
- `Service/GestorDocumental/AlmacenamientoDocumental/Builders/StoragePlanBuilder.cs`
  - Integra `IStorageExtensionResolver` + `IStorageNamingService`.
  - Valida `IdTipoDocumento` como requisito para naming.
  - Genera nombres legacy `DIG/FXL` en el plan físico.
- `Service/GestorDocumental/AlmacenamientoDocumental/Physical/StoragePhysicalPhaseExecutor.cs`
  - Consume el nuevo builder asíncrono.
- `Service/GestorDocumental/AlmacenamientoDocumental/Physical/StorageXmlWriter.cs`
  - Usa `plan.NombreXml` (FXL) en lugar de derivar el XML desde nombre principal.

### DocuArchi.Api
- `Program.cs`
  - Registro DI:
    - `IStorageExtensionRepository`
    - `IStorageExtensionResolver`
    - `IStorageNamingService`

### DocuArchiCore (tests)
- `tests/TramiteDiasVencimiento.Tests/StoragePhysicalPhaseExecutorTests.cs`
  - Ajuste a `BuildFilePlanAsync`.
  - Expectativas actualizadas a nombres legacy `DIG...` y `FXL...`.

## Flujo final de naming
1. `StoragePlanBuilder` obtiene `IdTipoDocumento` desde `context.Command.Trd`.
2. `StorageExtensionResolver` consulta `DA_EXTENSION` vía `StorageExtensionRepository`.
3. `StorageNamingService` construye:
   - `NombreArchivoPrincipal = DIG########.{ext}`
   - `NombreXml = FXL########.xml`
   - `SegundoNombre = DIG########.{ext}` (fallback)
4. `StorageFileWriter` copia el binario al nombre `DIG...`.
5. `StorageXmlWriter` escribe el XML físico con nombre `FXL...`.

## Reglas implementadas
- Prefijo fijo `DIG` para archivo principal.
- Prefijo fijo `FXL` para XML.
- Padding determinístico a 8 dígitos (`D8`).
- Extensión obligatoria desde base de datos (`DA_EXTENSION`).
- Normalización de extensión a minúsculas y con punto.

## Compatibilidad legacy cubierta
- `Ceros_Imagen_Almacenada(...)` equivalente funcional con `idAlmacen.ToString("D8")`.
- `RetornaExtensionTipoDocumento(...)` equivalente funcional mediante repositorio `DA_EXTENSION`.
- `Generando_Archivo_Dat_Xml(...)` alineado en convención de nombre XML `FXL########.xml`.

## Riesgos / pendientes controlados
- El `segundo_nombre` en índice electrónico transaccional aún no consume explícitamente `StorageNamingResult`.
- Validación automática no ejecutada en esta sesión por error de entorno SDK/MSBuild (`MSB4276`), no por lógica de negocio del cambio.

