# SCRUM-182 - Implementacion Detallada Tamano y Conteo Real de Paginas

## Repositorios Impactados
- `MiApp.Models`
- `MiApp.Services`
- `DocuArchi.Api`
- `DocuArchiCore` (tests + documentacion)

## Cambios por Componente

### MiApp.Models
- `Models/GestorDocumental/AlmacenamientoDocumental/Metadata/StorageDocumentPhysicalMetadata.cs`
  - Nuevo modelo: `TotalBytes`, `TamanoLegacy`, `Formato`, `NumeroPaginas`, `PaginasCalculadasDesdeArchivo`.
- `Models/GestorDocumental/AlmacenamientoDocumental/StorageContext.cs`
  - Nueva propiedad `PhysicalMetadata`.

### MiApp.Services
- `.../Metadata/IStorageSizeFormatter.cs`
- `.../Metadata/StorageSizeFormatter.cs`
  - Regla legacy: inicia desde bytes acumulados y formatea a `Kb` o `Mb` con 2 decimales.
- `.../Metadata/IStoragePageCountReader.cs`
- `.../Metadata/StoragePageCountReader.cs`
  - PDF: conteo por patron `/Type /Page`.
  - TIFF: lectura de cadena IFD.
  - Imagen comun: 1 pagina.
  - No soportado: `null`.
- `.../Metadata/IStorageDocumentMetadataAnalyzer.cs`
  - Implementa analisis integrado y fallback.
- `.../DocumentStorageOrchestrator.cs`
  - Inyecta analyzer opcional y llena `context.PhysicalMetadata`.
- `.../Identity/StorageIdentityAllocator.cs`
  - Prioriza `context.PhysicalMetadata.NumeroPaginas`.
- `.../Transaction/StorageTransactionCoordinator.cs`
  - Usa paginas fisicas para actualizacion de contadores.
- `.../Builders/StorageXmlBuilder.cs`
  - Agrega `Formato`, `Tamano`, `NumeroPaginasDocumento` al metadata map.
- `.../Expediente/IndiceElectronicoBuilder.cs`
  - Propaga `Formato` y `TamanoLegacy` al indice.
- `.../AlmacenarDocumentoUseCase.cs`
  - Garantiza set de `Command` en `StorageContext`.

### DocuArchi.Api
- `Program.cs`
  - Registro DI:
    - `IStorageDocumentMetadataAnalyzer`
    - `IStoragePageCountReader`
    - `IStorageSizeFormatter`

## Paridad vs Legacy
- Legacy calcula tamano antes de insertar inventario/log: replicado en analyzer.
- Legacy intenta numero de paginas real para PDF: replicado.
- Legacy usa fallback `Numero_Pag` cuando no hay conteo: replicado.
