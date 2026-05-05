# SCRUM-182 - Arquitectura Tamano y Conteo Real de Paginas

## Objetivo
Restaurar paridad funcional VB -> C# para metadata fisica previa a persistencia:
- tamano legacy (`Kb`/`Mb`),
- formato de archivo,
- conteo real de paginas,
- reutilizacion de metadatos en allocator/coordinator/xml/indice.

## Referencia Legacy
Funciones VB usadas como base:
- `Almacenamiento(...)`
- `Retorna_numero_paginas_documentos_unificados(...)`
- `Retorna_numero_paginas_documentos_pdf(...)`

## Diseno de Componentes
- `IStorageDocumentMetadataAnalyzer`: orquesta calculo de bytes, formato y paginas.
- `IStoragePageCountReader`: obtiene paginas por tipo de archivo (`pdf`, `tif/tiff`, imagenes).
- `IStorageSizeFormatter`: aplica formato de tamano legacy.

## Flujo de Arquitectura
1. `DocumentStorageOrchestrator` prepara `StorageContext` y resuelve archivos temporales.
2. `StorageDocumentMetadataAnalyzer` calcula metadata fisica real.
3. `StorageContext.PhysicalMetadata` transporta datos al flujo transaccional.
4. `StorageIdentityAllocator` y `StorageTransactionCoordinator` consumen paginas calculadas.
5. `StorageXmlBuilder` e `IndiceElectronicoBuilder` propagan formato/tamano al output.

## Decision Arquitectonica
No usar defaults ciegos para paginas o tamano cuando existe archivo fisico disponible. El fallback se limita a valores del comando cuando no hay parser aplicable.
