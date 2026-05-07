# SCRUM-189 — Auditoría de Código StorageEngine

## 1. Cobertura auditada
- Controller/API: `DocuArchi.Api\Controllers\GestorDocumental\AlmacenamientoDocumental`
- Servicios/Engine: `MiApp.Services\Service\GestorDocumental\AlmacenamientoDocumental`
- Repositorios: `MiApp.Repository\Repositorio\GestorDocumental\AlmacenamientoDocumental`
- Modelos/DTO: `MiApp.Models`, `MiApp.DTOs`
- Pruebas: `tests/TramiteDiasVencimiento.Tests`

Inventario automático base (180 coincidencias) generado con:
```powershell
rg -n "public (class|interface|...)" ..\MiApp.Services\...\AlmacenamientoDocumental ..\MiApp.Repository\...\AlmacenamientoDocumental ..\DocuArchi.Api\...\AlmacenamientoDocumental
```

## 2. Inventario de funciones (núcleo)
| Archivo | Clase | Función | Parámetros | Retorno | Invoca | Invocado por | Prompt | Estado | Observación |
|---|---|---|---|---|---|---|---|---|---|
| `.../AlmacenamientoDocumentalController.cs` | `AlmacenamientoDocumentalController` | `AlmacenarDocumento` | `AlmacenarDocumentoRequest` | `Task<ActionResult<AppResponses<...>>>` | `IClaimValidationService`, `IFeatureToggleService`, `IAlmacenarDocumentoUseCase` | API HTTP | 10/21 | Implementado | Entry point con feature flag |
| `.../AlmacenarDocumentoUseCase.cs` | `AlmacenarDocumentoUseCase` | `ExecuteAsync` | request, alias, usuario, usuarioId, ip | `Task<AppResponses<...>>` | `IDocumentStorageOrchestrator` | Controller | 3/10 | Implementado | Manejo central de excepciones |
| `.../DocumentStorageOrchestrator.cs` | `DocumentStorageOrchestrator` | `ExecuteAsync` | `StorageContext` | `Task<AlmacenarDocumentoResult>` | Metadata analyzer/path resolver (opcional) | UseCase | 3/10 | Parcial | Pipeline incremental, no CRUD simple |
| `.../StorageValidationPipeline.cs` | `StorageValidationPipeline` | `ValidateAsync` | `StorageContext` | `Task<StorageValidationResult>` | Lista `IStorageValidator` | Orchestrator | 4 | Implementado | Ordena validadores por `Order` |
| `.../StorageTransactionCoordinator.cs` | `StorageTransactionCoordinator` | `ExecuteAsync` | `StorageContext` | `Task<StorageTransactionResult>` | Identity allocator, disk repo, inventario, expediente, workflow | Orchestrator | 6 | Implementado | Núcleo ACID |
| `.../StorageTransactionCoordinator.cs` | `StorageTransactionCoordinator` | `TryInsertWorkflowLogAsync` | context, reservation, conn, tx | `Task<bool>` | naming, physical path, workflow log service | ExecuteAsync | 20 | Implementado | Aplica solo con `IdTareaWorkflow > 0` |
| `.../StorageTransactionCoordinator.cs` | `StorageTransactionCoordinator` | `TryInsertInventarioAsync` | context, reservation, conn, tx | `Task<long?>` | inventory builder/repository | ExecuteAsync | 7/17 | Implementado | Gobernado por options |
| `.../StorageTransactionCoordinator.cs` | `StorageTransactionCoordinator` | `TryExecuteExpedienteUnidadLegacyAsync` | context, conn, tx | `Task<ExpedienteUnidadLegacyResult?>` | expediente legacy service | ExecuteAsync | 8/18 | Implementado | Actualiza estado en context |
| `.../StorageIdentityAllocator.cs` | `StorageIdentityAllocator` | `ReserveAsync` | context, conn, tx | `Task<StorageIdentityReservationResult>` | `ISystemStorageRepository`, `IStorageIdentityPolicy` | TransactionCoordinator | 5 | Implementado | Reserva proxid + carpeta |
| `.../StorageIdentityPolicy.cs` | `StorageIdentityPolicy` | `Calculate` | system row, pages | `StorageIdentityReservationResult` | n/a | IdentityAllocator | 5 | Implementado | Reglas NUMPAG_CARP legacy |
| `.../StorageDiskQuotaPolicy.cs` | `StorageDiskQuotaPolicy` | `ValidateDiskAvailable` | status | `void` | n/a | Identity/Tx | 5/6 | Implementado | Estado SL/overflow |
| `.../StoragePhysicalPhaseExecutor.cs` | `StoragePhysicalPhaseExecutor` | `ExecuteAsync` | context, txResult | `Task<StoragePhysicalStatusModel>` | path service, writer, xml, compensation | Orchestrator | 9 | Implementado | Control fase post-commit |
| `.../StoragePhysicalPathService.cs` | `StoragePhysicalPathService` | `ResolveAsync` | context, identity | `Task<StoragePhysicalPathModel>` | route repo, folder policy | Physical executor / Tx | 14 | Implementado | Ruta legacy SYSTEM1RUT |
| `.../StoragePathResolver.cs` | `StoragePathResolver` | `GetStorageRoot` | gabinete | `string` | n/a | metadata/physical | 14 | Implementado | Resolver raíz segura |
| `.../StoragePathResolver.cs` | `StoragePathResolver` | `GetFinalFolder` | gabinete, disco, carpeta | `string` | n/a | physical flow | 14 | Implementado | Carpeta `{gab}{disco}\0000n` |
| `.../StorageFileWriter.cs` | `StorageFileWriter` | `CopyAsync` | plan, compensationPlan, requestId | `Task<string>` | IO file copy | Physical executor | 9 | Implementado | Copia DIG + adjuntos |
| `.../StorageXmlWriter.cs` | `StorageXmlWriter` | `WriteAsync` | path, id, model, temp | `Task<string>` | XML writer | Physical executor | 9/13 | Implementado | FXL legacy |
| `.../StorageCompensationManager.cs` | `StorageCompensationManager` | `ExecuteAsync` | plan, requestId | `Task` | IO delete best-effort | Physical executor | 9 | Implementado | Compensación post-fallo |
| `.../StoragePreindexResolver.cs` | `StoragePreindexResolver` | `Resolve` | context | `StoragePreindexFile` | fs lookup | preindex validator | 12 | Implementado | txt/xmls discovery |
| `.../StoragePreindexReader.cs` | `StoragePreindexReader` | `ReadAsync` | file | `Task<StoragePreindexResult>` | parse txt/xmls | preindex validator | 12 | Implementado | lectura robusta |
| `.../StoragePreindexIntegrator.cs` | `StoragePreindexIntegrator` | `Integrate` | context, preindex | `void` | merge campos | preindex validator | 12 | Implementado | integra valores pipeline |
| `.../StorageOptionsResolver.cs` | `StorageOptionsResolver` | `ResolveAsync` | gabinete, alias | `Task<StorageResolvedOptionsModel>` | `IStorageSystemOptionsRepository` | validadores/tx | 15/188 | Implementado | opciones reales system1 |
| `.../StorageOptionsValidator.cs` | `StorageOptionsValidator` | `ValidateAsync` | context, errors | `Task` | reglas inventario/TRD/unidad | pipeline | 15 | Implementado | gobernanza por opciones |
| `.../WorkflowStorageLogBuilder.cs` | `WorkflowStorageLogBuilder` | `Build` | context, identity, naming, path | `WorkflowStorageLogBuildResult` | n/a | workflow service | 20 | Implementado | payload logdocuarchi |
| `.../WorkflowStorageLogService.cs` | `WorkflowStorageLogService` | `ExecuteAsync` | context, identity, naming, path, conn, tx | `Task` | log repository | tx coordinator | 20 | Implementado | inserción condicional |
| `.../StorageDocumentMetadataAnalyzer.cs` | `StorageDocumentMetadataAnalyzer` | `AnalyzeAsync` | context, files | `Task<StorageDocumentPhysicalMetadata>` | page reader, size formatter | orchestrator | 16 | Implementado | tamaño+paginado |
| `.../StorageSizeFormatter.cs` | `StorageSizeFormatter` | `FormatLegacy` | bytes | `string` | n/a | metadata analyzer | 16 | Implementado | `Kb/Mb` legacy |
| `.../StoragePageCountReader.cs` | `StoragePageCountReader` | `TryReadPageCountAsync` | filePath | `Task<int?>` | pdf parser | metadata analyzer | 16 | Implementado | conteo pdf |
| `.../ExpedienteUnidadLegacyService.cs` | `ExpedienteUnidadLegacyService` | `ExecuteAsync` | context, conn, tx | `Task<ExpedienteUnidadLegacyResult>` | repos legacy expediente/unidad | tx coordinator | 18 | Implementado | reglas de expediente/unidad |
| `.../ExpedienteIndiceXmlService.cs` | `ExpedienteIndiceXmlService` | `ExecuteAsync` | context, plan, conn, tx | `Task<ExpedienteIndiceXmlUpdateResult>` | builder + writer + repo | expediente flow | 19 | Implementado | actualiza XML índice |
| `.../StorageDiskQuotaRepository.cs` | `StorageDiskQuotaRepository` | `LockDiskStatusAsync` | gabinete, disco, conn, tx | `Task<DiskQuotaStatusModel?>` | SQL lock | tx coordinator | 6 | Implementado | `FOR UPDATE` |
| `.../StorageDiskQuotaRepository.cs` | `StorageDiskQuotaRepository` | `UpdateDiskUsageAsync` | model, conn, tx | `Task<int>` | SQL update | tx coordinator | 6 | Implementado | update pag/imagenes |
| `.../SystemStorageRepository.cs` | `SystemStorageRepository` | `LockByGabineteAsync` | gabinete, conn, tx | `Task<SystemStorageRow?>` | SQL lock | identity allocator | 5 | Implementado | lock `system1` |
| `.../SystemStorageRepository.cs` | `SystemStorageRepository` | `UpdateReservationAsync` | gabinete, reservation, conn, tx | `Task<int>` | SQL update | identity allocator | 5 | Implementado | update proxid/carpeta |
| `.../GabineteStorageRepository.cs` | `GabineteStorageRepository` | `InsertAsync` | insert model, conn, tx | `Task<int>` | SQL insert dynamic | tx coordinator | 7 | Implementado | inserta en tabla gabinete |
| `.../InventarioDocumentalRepository.cs` | `InventarioDocumentalRepository` | `InsertAsync` | model, conn, tx | `Task<long>` | SQL insert | tx coordinator | 17 | Implementado | registro_producion_documental |
| `.../WorkflowStorageLogRepository.cs` | `WorkflowStorageLogRepository` | `InsertAsync` | model, conn, tx | `Task<int>` | SQL insert | workflow service | 20 | Implementado | `logdocuarchi` |
| `.../ExpedienteRepository.cs` | `ExpedienteRepository` | `LockExpedienteAsync` | id, conn, tx | `Task<ExpedienteInfoModel?>` | SQL lock | expediente services | 8/19 | Implementado | lock expediente |
| `.../ExpedienteRepository.cs` | `ExpedienteRepository` | `UpdateIndiceAsync` | model, conn, tx | `Task<int>` | SQL update | índice electrónico | 19 | Implementado | orden/pagina índice |
| `.../UnidadConservacionRepository.cs` | `UnidadConservacionRepository` | `LockAsync` | id, conn, tx | `Task<UnidadConservacionInfoModel?>` | SQL lock | expediente/unidad flow | 18 | Implementado | lock unidad |
| `.../UnidadConservacionRepository.cs` | `UnidadConservacionRepository` | `UpdateFoliosAsync` | model, conn, tx | `Task<int>` | SQL update | expediente/unidad flow | 18 | Implementado | folios electrónico/digitalizado |

## 3. Hallazgos de auditoría
1. El patrón Engine está implementado por componentes, pero `DocumentStorageOrchestrator` aún conserva integración incremental (comentario de pipeline futuro).
2. La cobertura de pruebas unitarias es amplia; integración E2E full con Docker sigue parcialmente diferida en casos `Skip`.
3. Se mantiene separación correcta entre transacción DB y fase física con compensación.
4. El mapping de `logdocuarchi` ya preserva semántica legacy crítica (`id_tran`, `RUT_DOCU`, `TIPOLOGIA_DOCUMENTAL`, `IP_TRANS`).

## 4. Recomendaciones de control
1. Consolidar orquestación completa en `DocumentStorageOrchestrator` eliminando puntos de integración opcional.
2. Ejecutar de forma obligatoria la suite de concurrencia real en pipeline protegido.
3. Mantener snapshot de inventario automático de funciones por release para detectar drift arquitectónico.
