# SCRUM-182 - Regresion Legacy Tamano y Paginas

## Matriz de Equivalencia VB -> C#

| Legacy VB | Componente C# | Estado |
|---|---|---|
| `Retorna_numero_paginas_documentos_unificados` | `StoragePageCountReader.GetPageCountOrNullAsync` | Implementado |
| `Retorna_numero_paginas_documentos_pdf` | Parser PDF en `StoragePageCountReader` | Implementado |
| Calculo `tamano` en `Almacenamiento` | `StorageSizeFormatter` + `StorageDocumentMetadataAnalyzer` | Implementado |
| Fallback `Numero_Pag` | `StorageDocumentMetadataAnalyzer` | Implementado |
| Uso de paginas en contadores de disco/carpeta | `StorageIdentityAllocator` + `StorageTransactionCoordinator` | Implementado |

## Riesgos de Regresion Controlados
- Subconteo de paginas para documentos unificados.
- Rotacion de carpeta incorrecta por paginas no reales.
- Inconsistencia entre XML/indice y metadata fisica real.

## Riesgo Residual
El parser PDF basado en texto puede no cubrir variantes complejas de estructura PDF; en esos casos opera fallback del comando.
