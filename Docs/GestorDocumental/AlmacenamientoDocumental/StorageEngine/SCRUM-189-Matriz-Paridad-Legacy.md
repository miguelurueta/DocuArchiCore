# SCRUM-189 — Matriz de Paridad Legacy (VB vs C#)

| Comportamiento VB | Implementación C# | Prompt | Estado | Evidencia | Brecha | Acción |
|---|---|---|---|---|---|---|
| Reserva `PROXID` + carpeta + páginas | `StorageIdentityAllocator` + `SystemStorageRepository` | 5 | CUMPLE | tests `StorageIdentityAllocatorTests` | ninguna | mantener |
| Lock/Update disco detalle | `StorageDiskQuotaRepository` + `StorageTransactionCoordinator` | 6 | CUMPLE | tests `StorageDiskQuotaRepositoryTests` + tx tests | ninguna | mantener |
| Campos obligatorios gabinete | `StorageGabineteMetadataProvider` + `GabineteRequiredFieldsValidator` | 11/177 | CUMPLE | tests `StorageValidationPipelineTests` | ninguna | mantener |
| Preindex TXT/XMLS | `StoragePreindexResolver/Reader/Integrator` + `PreindexValidator` | 12/178 | CUMPLE | tests `StorageValidationPipelineTests` + suite paridad | baja cobertura XMLS real | ampliar integración |
| Naming DIG/FXL | `StorageNamingService` + `StorageXmlWriter` | 13/179 | CUMPLE | tests `StorageNamingServiceTests` | ninguna | mantener |
| Ruta SYSTEM1RUT | `StorageRouteRepository` + `StoragePhysicalPathService` | 14/180 | CUMPLE | tests `StorageRouteRepositoryTests` | ninguna | mantener |
| Opciones `system1` (inventario/TRD/unidad) | `StorageOptionsResolver` + validadores | 15/188 | CUMPLE | tests `StorageOptionsResolverTests` | ninguna | mantener |
| Tamaño y páginas reales | `StorageDocumentMetadataAnalyzer` + `StoragePageCountReader` | 16/182 | CUMPLE | tests `StoragePhysicalMetadataAnalyzerTests` | conteo no-PDF limitado | mejora incremental |
| Inventario documental | `InventarioDocumentalBuilder` + repo | 17/183 | CUMPLE | tests `Inventario...Tests` | ninguna | mantener |
| Expediente/unidad | `ExpedienteUnidadLegacyService` + repos legacy | 18/184 | CUMPLE | tests `ExpedienteUnidadLegacyServiceTests` | cobertura concurrencia real | ejecutar docker |
| Índice expediente XML | `ExpedienteIndiceXmlService/Writer` | 19/185 | CUMPLE | tests `ExpedienteIndiceXml...Tests` | manejo de recovery avanzado | reforzar runbook |
| Logdocuarchi workflow | `WorkflowStorageLogService/Builder/Repository` | 20/186 | CUMPLE | tests `WorkflowStorageLog...Tests` | ninguna | mantener |
| Validación final paridad | suite `StorageEngine/Parity` | 21/187 | CUMPLE CON MEJORA | `5 pass / 2 skip` | faltan escenarios Docker E2E | cerrar brecha en pipeline |

## Resumen
- **CUMPLE**: 12
- **CUMPLE CON MEJORA**: 1
- **PARCIAL**: 0
- **NO CUMPLE**: 0
- **PENDIENTE VALIDACIÓN**: 0 (pero con pendientes operativos en integración Docker)
