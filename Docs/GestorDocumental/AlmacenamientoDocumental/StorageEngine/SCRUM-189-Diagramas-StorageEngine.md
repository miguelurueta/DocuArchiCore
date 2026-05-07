# SCRUM-189 - Diagramas StorageEngine (UML / PlantUML)

## Alcance
Documento de diagramas en formato UML compatible con PlantUML para publicacion tecnica enterprise.

## 1) Diagrama de Casos de Uso
```plantuml
@startuml
left to right direction
actor "Usuario Integrador" as Usuario
rectangle "Storage Engine" {
  usecase "Registrar documento simple" as UC1
  usecase "Registrar con preindex" as UC2
  usecase "Registrar con inventario" as UC3
  usecase "Registrar con TRD" as UC4
  usecase "Registrar con expediente/unidad" as UC5
  usecase "Registrar con workflow log" as UC6
}
Usuario --> UC1
Usuario --> UC2
Usuario --> UC3
Usuario --> UC4
Usuario --> UC5
Usuario --> UC6
@enduml
```

## 2) Diagrama de Clases (Nucleo)
```plantuml
@startuml
skinparam classAttributeIconSize 0

interface IAlmacenarDocumentoUseCase {
  +ExecuteAsync(request, defaultDbAlias, usuario, usuarioId, ipTrans) : Task<AppResponses<AlmacenarDocumentoResponse?>>
}

interface IDocumentStorageOrchestrator {
  +ExecuteAsync(context) : Task<AlmacenarDocumentoResult>
}

interface IStorageValidationPipeline {
  +ValidateAsync(context) : Task<StorageValidationResult>
}

interface IStorageTransactionCoordinator {
  +ExecuteAsync(context) : Task<StorageTransactionResult>
}

interface IStoragePhysicalPhaseExecutor {
  +ExecuteAsync(context, transactionResult) : Task<StoragePhysicalStatusModel>
}

interface IStorageCompensationManager {
  +ExecuteAsync(compensationPlan, requestId) : Task
}

class AlmacenamientoDocumentalController {
  +AlmacenarDocumento(request) : Task<IActionResult>
}

class AlmacenarDocumentoUseCase {
  -_orchestrator : IDocumentStorageOrchestrator
  -_logger : ILogger<AlmacenarDocumentoUseCase>
  +ExecuteAsync(request, defaultDbAlias, usuario, usuarioId, ipTrans) : Task<AppResponses<AlmacenarDocumentoResponse?>>
  -Validation(field, message, storageErrors) : AppResponses<AlmacenarDocumentoResponse?>
  -Error(message, details) : AppResponses<AlmacenarDocumentoResponse?>
}

class DocumentStorageOrchestrator {
  -_logger : ILogger<DocumentStorageOrchestrator>
  -_metadataAnalyzer : IStorageDocumentMetadataAnalyzer
  -_pathResolver : IStoragePathResolver
  +ExecuteAsync(context) : Task<AlmacenarDocumentoResult>
}

class StorageValidationPipeline {
  -_validators : IReadOnlyList<IStorageValidator>
  -_logger : ILogger<StorageValidationPipeline>
  +ValidateAsync(context) : Task<StorageValidationResult>
}

class StorageTransactionCoordinator {
  -_dbConnectionFactory : IDbConnectionFactory
  -_identityAllocator : IStorageIdentityAllocator
  -_storageDiskQuotaRepository : IStorageDiskQuotaRepository
  -_inventarioBuilder : IInventarioDocumentalBuilder
  -_inventarioRepository : IInventarioDocumentalRepository
  -_namingService : IStorageNamingService
  -_expedienteUnidadLegacyService : IExpedienteUnidadLegacyService
  -_workflowStorageLogService : IWorkflowStorageLogService
  -_storagePhysicalPathService : IStoragePhysicalPathService
  -_logger : ILogger<StorageTransactionCoordinator>
  +ExecuteAsync(context) : Task<StorageTransactionResult>
  -TryInsertWorkflowLogAsync(context, reservation, connection, transaction) : Task<bool>
  -TryInsertInventarioAsync(context, reservation, connection, transaction) : Task<long?>
  -TryExecuteExpedienteUnidadLegacyAsync(context, connection, transaction) : Task<ExpedienteUnidadLegacyResult?>
  -ResolveNumeroPaginasDocumento(context) : int
}

class StoragePhysicalPhaseExecutor {
  -_storagePlanBuilder : IStoragePlanBuilder
  -_storageXmlBuilder : IStorageXmlBuilder
  -_storageFileWriter : IStorageFileWriter
  -_storageXmlWriter : IStorageXmlWriter
  -_compensationManager : IStorageCompensationManager
  -_expedienteIndiceXmlService : IExpedienteIndiceXmlService
  -_logger : ILogger<StoragePhysicalPhaseExecutor>
  +ExecuteAsync(context, transactionResult) : Task<StoragePhysicalStatusModel>
  -TryUpdateExpedienteIndiceXmlAsync(context, transactionResult, filePlan, compensationPlan) : Task<ExpedienteIndiceXmlUpdateResult?>
}

class StorageCompensationManager {
  -_logger : ILogger<StorageCompensationManager>
  +ExecuteAsync(compensationPlan, requestId) : Task
  -DeleteFiles(files, requestId, phase) : void
  -DeleteDirectories(directories, requestId) : void
}

class StorageContext {
  +DefaultDbAlias : string
  +Usuario : string
  +UsuarioId : int
  +RequestId : string
  +NombreGabinete : string
  +RutaTemporalId : string
  +NombreDocumento : string
  +ArchivosTemporales : IReadOnlyList<string>
  +IpTrans : string?
  +Command : AlmacenarDocumentoCommand?
  +PreindexValues : IReadOnlyList<string>
  +EffectiveCamposIndexacion : IReadOnlyList<CampoIndexacionDto>
  +GabineteFieldsMetadata : IReadOnlyList<GabineteFieldMetadata>
  +ResolvedOptions : StorageResolvedOptionsModel?
  +PhysicalMetadata : StorageDocumentPhysicalMetadata?
  +ExpedienteUnidadResult : ExpedienteUnidadLegacyResult?
}

class AlmacenarDocumentoCommand {
  +NombreGabinete : string
  +RutaTemporalId : string
  +NombreDocumento : string
  +RequestId : string
  +Documentos : IReadOnlyList<DocumentoEntradaDto>
  +CamposIndexacion : IReadOnlyList<CampoIndexacionDto>
  +Inventario : InventarioDocumentalDto?
  +Trd : TrdStorageDto?
  +Expediente : ExpedienteStorageDto?
  +Workflow : WorkflowStorageDto?
  +FullText : string?
  +NumeroPaginasDeclaradas : int
  +TipoAlmacenamiento : TipoAlmacenamientoEnum
  +EvaluarCamposObligatorios : bool
}

class StorageResolvedOptionsModel {
  +NombreGabinete : string
  +AplicaInventarioDocumental : bool
  +AplicaTrd : bool
  +AplicaUnidadConservacion : bool
}

class StorageValidationResult {
  +IsValid : bool
  +Errors : List<StorageError>
}

class StorageError {
  +Code : string
  +Message : string
  +Phase : StoragePhase?
}

class StoragePlanModel {
  +Identity : StorageIdentityModel
  +Metadata : StorageMetadataModel
  +RutaFinal : string
}

class StorageTransactionResult {
  +IdAlmacen : long
  +IdentityReservation : StorageIdentityReservationResult
  +IdRegistroProduccionDocumental : long?
  +Success : bool
  +Estado : StorageDocumentState
  +RequestId : string
  +FechaEjecucion : DateTime
  +DuracionMs : long
  +DiskUsageUpdated : bool
  +WorkflowLogInserted : bool
  +ExpedienteUnidadResult : ExpedienteUnidadLegacyResult?
}

class AlmacenarDocumentoResult {
  +IdAlmacen : long
  +IdRegistroProduccionDocumental : long?
  +NombreArchivoFinal : string
  +RequestId : string
  +Estado : StorageDocumentState
}

IAlmacenarDocumentoUseCase <|.. AlmacenarDocumentoUseCase
IDocumentStorageOrchestrator <|.. DocumentStorageOrchestrator
IStorageValidationPipeline <|.. StorageValidationPipeline
IStorageTransactionCoordinator <|.. StorageTransactionCoordinator
IStoragePhysicalPhaseExecutor <|.. StoragePhysicalPhaseExecutor
IStorageCompensationManager <|.. StorageCompensationManager

AlmacenamientoDocumentalController --> IAlmacenarDocumentoUseCase
AlmacenarDocumentoUseCase --> IDocumentStorageOrchestrator
DocumentStorageOrchestrator --> StorageContext
DocumentStorageOrchestrator --> AlmacenarDocumentoResult
StorageValidationPipeline --> StorageContext
StorageValidationPipeline --> StorageValidationResult
StorageTransactionCoordinator --> StorageContext
StorageTransactionCoordinator --> StorageTransactionResult
StoragePhysicalPhaseExecutor --> StorageContext
StoragePhysicalPhaseExecutor --> StorageTransactionResult
StoragePhysicalPhaseExecutor --> IStorageCompensationManager
StorageContext --> AlmacenarDocumentoCommand
StorageContext --> StorageResolvedOptionsModel
@enduml
```

## 2.1) Diagrama de Clases de Modelos y DTOs (Detalle Completo de Atributos)
```plantuml
@startuml
skinparam classAttributeIconSize 0

class AlmacenarDocumentoRequest {
  +NombreGabinete : string
  +RutaTemporalId : string
  +NombreDocumento : string
  +RequestId : string
  +Documentos : IReadOnlyList<DocumentoEntradaDto>
  +CamposIndexacion : IReadOnlyList<CampoIndexacionDto>
  +Inventario : InventarioDocumentalDto?
  +Trd : TrdStorageDto?
  +Expediente : ExpedienteStorageDto?
  +Workflow : WorkflowStorageDto?
  +FullText : string?
  +NumeroPaginasDeclaradas : int
}

class AlmacenarDocumentoResponse {
  +IdAlmacen : long
  +IdRegistroProduccionDocumental : long?
  +NombreArchivoFinal : string
  +RequestId : string
}

class DocumentoEntradaDto {
  +IdDocumento : string
  +ArchivoTemporalId : string
  +NombreOriginal : string?
  +Extension : string?
  +NumeroPaginas : int
}

class CampoIndexacionDto {
  +NombreCampo : string
  +Valor : string?
  +EsObligatorio : bool
}

class InventarioDocumentalDto {
  +IdUsuarioGestion : int
  +IdEmpresa : int
  +Radicado : string?
  +FechaElaboracion : string?
  +SegundoNombreDocumento : string?
}

class TrdStorageDto {
  +IdArea : int?
  +IdSerie : int?
  +IdSubSerie : int?
  +IdTipoDocumento : int?
  +NombreArea : string?
  +NombreSerie : string?
  +NombreSubSerie : string?
  +NombreTipoDocumento : string?
}

class ExpedienteStorageDto {
  +IdExpediente : int?
  +IdTipoExpediente : int?
  +IdTipoUnidadConservacion : int?
  +IdUnidadConservacion : int?
  +IdClaseDocumento : int?
  +NombreExpediente : string?
  +ClaseDocumento : string?
  +NombreUnidadConservacion : string?
}

class WorkflowStorageDto {
  +IdTareaWorkflow : long?
  +IdRutaWorkflow : int?
}

class AlmacenarDocumentoCommand {
  +NombreGabinete : string
  +RutaTemporalId : string
  +NombreDocumento : string
  +RequestId : string
  +Documentos : IReadOnlyList<DocumentoEntradaDto>
  +CamposIndexacion : IReadOnlyList<CampoIndexacionDto>
  +Inventario : InventarioDocumentalDto?
  +Trd : TrdStorageDto?
  +Expediente : ExpedienteStorageDto?
  +Workflow : WorkflowStorageDto?
  +FullText : string?
  +NumeroPaginasDeclaradas : int
  +TipoAlmacenamiento : TipoAlmacenamientoEnum
  +EvaluarCamposObligatorios : bool
}

class StorageContext {
  +DefaultDbAlias : string
  +Usuario : string
  +UsuarioId : int
  +RequestId : string
  +NombreGabinete : string
  +RutaTemporalId : string
  +NombreDocumento : string
  +ArchivosTemporales : IReadOnlyList<string>
  +IpTrans : string?
  +Command : AlmacenarDocumentoCommand?
  +PreindexValues : IReadOnlyList<string>
  +EffectiveCamposIndexacion : IReadOnlyList<CampoIndexacionDto>
  +GabineteFieldsMetadata : IReadOnlyList<GabineteFieldMetadata>
  +ResolvedOptions : StorageResolvedOptionsModel?
  +PhysicalMetadata : StorageDocumentPhysicalMetadata?
  +ExpedienteUnidadResult : ExpedienteUnidadLegacyResult?
}

class StorageResolvedOptionsModel {
  +NombreGabinete : string
  +AplicaInventarioDocumental : bool
  +AplicaTrd : bool
  +AplicaUnidadConservacion : bool
}

class StorageDocumentPhysicalMetadata {
  +TotalBytes : long
  +TamanoLegacy : string
  +Formato : string
  +NumeroPaginas : int
  +PaginasCalculadasDesdeArchivo : bool
}

class GabineteFieldMetadata {
  +FieldName : string
  +IsRequired : bool
  +Orden : int
}

class StorageIdentityModel {
  +IdAlmacen : long
  +Disco : int
  +Carpeta : int
  +NumeroPaginasCarpeta : int
}

class StorageMetadataModel {
  +Formato : string
  +Tamano : string
  +NumeroPaginas : int
  +NombreFinalDocumento : string
}

class StorageIdentityReservationResult {
  +Identity : StorageIdentityModel
  +PreviousProxId : long
  +NewProxId : long
  +PreviousFolder : int
  +NewFolder : int
  +PreviousFolderPages : int
  +NewFolderPages : int
  +TamDisc : long
}

class StoragePlanModel {
  +Identity : StorageIdentityModel
  +Metadata : StorageMetadataModel
  +RutaFinal : string
}

class StorageFilePlanModel {
  +StorageRoot : string
  +RutaFinal : string
  +NombreArchivoPrincipal : string
  +NombreXml : string
  +SegundoNombreDocumental : string
  +ArchivosOrigen : List<string>
}

class StorageNamingResult {
  +NombreArchivoPrincipal : string
  +NombreXml : string
  +SegundoNombre : string
}

class StorageCompensationPlan {
  +ArchivosTemporales : List<string>
  +ArchivosFinales : List<string>
  +XmlTemporales : List<string>
  +XmlFinales : List<string>
  +XmlIndiceExpedienteTemporales : List<string>
  +XmlIndiceExpedienteFinales : List<string>
  +DirectoriosCreados : List<string>
}

class ExpedienteIndiceXmlUpdateResult {
  +Updated : bool
  +RutaArchivoXml : string
  +Estado : string
}

class ExpedienteUnidadLegacyResult {
  +Ejecutado : bool
  +TieneExpediente : bool
  +TieneUnidadConservacion : bool
  +IdTipoUnidadDocumental : int?
  +EstadoExpedienteElectronico : int?
  +NumeroFolios : int
  +UnidadConservaTipo : UnidadConservaTipoEnum
}

class StorageTransactionResult {
  +IdAlmacen : long
  +IdentityReservation : StorageIdentityReservationResult
  +IdRegistroProduccionDocumental : long?
  +Success : bool
  +Estado : StorageDocumentState
  +RequestId : string
  +FechaEjecucion : DateTime
  +DuracionMs : long
  +DiskUsageUpdated : bool
  +WorkflowLogInserted : bool
  +ExpedienteUnidadResult : ExpedienteUnidadLegacyResult?
}

class StoragePhysicalStatusModel {
  +RequestId : string
  +IdAlmacen : long
  +NombreArchivoFinal : string
  +Estado : StorageDocumentState
  +ErrorMessage : string?
  +FechaActualizacion : DateTime
  +ExpedienteIndiceXmlResult : ExpedienteIndiceXmlUpdateResult?
  +InconsistenciaPostCommit : string?
}

class StorageError {
  +Code : string
  +Message : string
  +Phase : StoragePhase?
}

class StorageValidationResult {
  +IsValid : bool
  +Errors : List<StorageError>
}

class AlmacenarDocumentoResult {
  +IdAlmacen : long
  +IdRegistroProduccionDocumental : long?
  +NombreArchivoFinal : string
  +RequestId : string
  +Estado : StorageDocumentState
}

enum StoragePhase {
  RequestReceived
  Validation
  Transaction
  IdentityReservation
  GabineteInsert
  InventarioInsert
  ExpedienteIndex
  FileSystem
  Xml
  Compensation
  Completed
  Failed
}

enum StorageDocumentState {
  Pending
  Reserved
  Completed
  PhysicalFailed
  Failed
}

enum TipoAlmacenamientoEnum {
  BatchPreindex
  Manual
  Digitalizacion
  Workflow
}

enum UnidadConservaTipoEnum {
  Ninguna
  Digitalizado
  Electronico
}

AlmacenarDocumentoRequest "1" *-- "1..*" DocumentoEntradaDto
AlmacenarDocumentoRequest "1" *-- "0..*" CampoIndexacionDto
AlmacenarDocumentoRequest "1" o-- "0..1" InventarioDocumentalDto
AlmacenarDocumentoRequest "1" o-- "0..1" TrdStorageDto
AlmacenarDocumentoRequest "1" o-- "0..1" ExpedienteStorageDto
AlmacenarDocumentoRequest "1" o-- "0..1" WorkflowStorageDto

AlmacenarDocumentoCommand "1" *-- "1..*" DocumentoEntradaDto
AlmacenarDocumentoCommand "1" *-- "0..*" CampoIndexacionDto
AlmacenarDocumentoCommand "1" o-- "0..1" InventarioDocumentalDto
AlmacenarDocumentoCommand "1" o-- "0..1" TrdStorageDto
AlmacenarDocumentoCommand "1" o-- "0..1" ExpedienteStorageDto
AlmacenarDocumentoCommand "1" o-- "0..1" WorkflowStorageDto
AlmacenarDocumentoCommand --> TipoAlmacenamientoEnum

StorageContext "1" o-- "0..1" AlmacenarDocumentoCommand
StorageContext "1" *-- "0..*" CampoIndexacionDto
StorageContext "1" *-- "0..*" GabineteFieldMetadata
StorageContext "1" o-- "0..1" StorageResolvedOptionsModel
StorageContext "1" o-- "0..1" StorageDocumentPhysicalMetadata
StorageContext "1" o-- "0..1" ExpedienteUnidadLegacyResult

StorageIdentityReservationResult "1" *-- "1" StorageIdentityModel
StoragePlanModel "1" *-- "1" StorageIdentityModel
StoragePlanModel "1" *-- "1" StorageMetadataModel
StorageTransactionResult "1" *-- "1" StorageIdentityReservationResult
StorageTransactionResult "1" o-- "0..1" ExpedienteUnidadLegacyResult
StorageTransactionResult --> StorageDocumentState

StoragePhysicalStatusModel "1" o-- "0..1" ExpedienteIndiceXmlUpdateResult
StoragePhysicalStatusModel --> StorageDocumentState
ExpedienteUnidadLegacyResult --> UnidadConservaTipoEnum

StorageValidationResult "1" *-- "0..*" StorageError
StorageError --> StoragePhase
AlmacenarDocumentoResult --> StorageDocumentState
@enduml
```

## 3) Diagrama de Secuencia (Flujo Principal)
```plantuml
@startuml
autonumber
actor Cliente
participant "Controller" as C
participant "UseCase" as U
participant "Orchestrator" as O
participant "ValidationPipeline" as V
participant "PlanBuilder" as P
participant "TxCoordinator" as T
database "MySQL" as DB
participant "PhysicalExecutor" as F
participant "FileSystem/XML" as FS

Cliente -> C : POST almacenar documento
C -> U : ExecuteAsync(command)
U -> O : ExecuteAsync(context)

O -> V : ValidateAsync(context)
V --> O : OK

O -> P : BuildPlanAsync(context)
P --> O : StoragePlan

O -> T : ExecuteAsync(context, plan)
T -> DB : BEGIN / SELECT FOR UPDATE
T -> DB : UPDATE system1 + disco_detalle
T -> DB : INSERT gabinete + opcionales
T -> DB : COMMIT
T --> O : TxResult

O -> F : ExecuteAsync(context, plan, txResult)
F -> FS : Copy files + write xml
FS --> F : OK
F --> O : PhysicalResult OK

O --> U : StorageResult YES
U --> C : AppResponse 200
@enduml
```

## 3.0) Fuentes PlantUML por Clase (Archivos .puml)
- `PlantUML/SCRUM-189/Sequence-AlmacenarDocumentoUseCase.puml`
- `PlantUML/SCRUM-189/Sequence-DocumentStorageOrchestrator.puml`
- `PlantUML/SCRUM-189/Sequence-StorageValidationPipeline.puml`
- `PlantUML/SCRUM-189/Sequence-StorageTransactionCoordinator.puml`
- `PlantUML/SCRUM-189/Sequence-StoragePhysicalPhaseExecutor.puml`
- `PlantUML/SCRUM-189/Sequence-StorageCompensationManager.puml`

## 3.1) Secuencia por Clase - AlmacenarDocumentoUseCase
```plantuml
@startuml
autonumber
participant "Controller" as C
participant "AlmacenarDocumentoUseCase" as UC
participant "DocumentStorageOrchestrator" as OR

C -> UC : ExecuteAsync(request, alias, usuario, usuarioId, ip)
UC -> UC : validar request/alias/usuario/documentos

alt request invalido
  UC --> C : AppResponses{success=false, Validation}
else request valido
  UC -> UC : construir AlmacenarDocumentoCommand
  UC -> UC : construir StorageContext
  UC -> OR : ExecuteAsync(context)

  alt StorageValidationException
    OR --> UC : throw StorageValidationException
    UC --> C : AppResponses{success=false, Validation + errores}
  else StorageTransactionException
    OR --> UC : throw StorageTransactionException
    UC --> C : AppResponses{success=false, Error transaccional}
  else StoragePhysicalException
    OR --> UC : throw StoragePhysicalException
    UC --> C : AppResponses{success=false, Error FS/XML}
  else OK
    OR --> UC : AlmacenarDocumentoResult
    UC --> C : AppResponses{success=true, AlmacenarDocumentoResponse}
  end
end
@enduml
```

## 3.2) Secuencia por Clase - DocumentStorageOrchestrator
```plantuml
@startuml
autonumber
participant "UseCase" as UC
participant "DocumentStorageOrchestrator" as OR
participant "IStoragePathResolver" as PR
participant "IStorageDocumentMetadataAnalyzer" as MA

UC -> OR : ExecuteAsync(context)
OR -> OR : validar context y requestId

opt metadataAnalyzer y pathResolver disponibles
  loop por cada documento temporal
    OR -> PR : GetTemporaryFilePath(rutaTemporalId, archivoTemporalId)
    PR --> OR : rutaAbsolutaTemporal
  end
  OR -> MA : AnalyzeAsync(context, archivosOrigen)
  MA --> OR : StorageDocumentPhysicalMetadata
  OR -> OR : context.PhysicalMetadata = metadata
end

OR --> UC : AlmacenarDocumentoResult (estado actual/target)
@enduml
```

## 3.3) Secuencia por Clase - StorageValidationPipeline
```plantuml
@startuml
autonumber
participant "Orchestrator" as OR
participant "StorageValidationPipeline" as VP
participant "IStorageValidator[*]" as V

OR -> VP : ValidateAsync(context)
VP -> VP : iniciar stopwatch + lista errors

alt context nulo
  VP -> VP : agregar error CTX_REQUIRED
  VP --> OR : StorageValidationResult{IsValid=false}
else context valido
  loop validators ordenados por Order
    VP -> V : ValidateAsync(context, errors)
    V --> VP : errors actualizado
  end
  VP --> OR : StorageValidationResult{IsValid=(errors==0)}
end
@enduml
```

## 3.4) Secuencia por Clase - StorageTransactionCoordinator
```plantuml
@startuml
autonumber
participant "Orchestrator" as OR
participant "StorageTransactionCoordinator" as TC
participant "IDbConnectionFactory" as CF
participant "IStorageIdentityAllocator" as IA
participant "IStorageDiskQuotaRepository" as DQ
participant "IInventarioDocumentalBuilder" as IB
participant "IInventarioDocumentalRepository" as IR
participant "IExpedienteUnidadLegacyService" as EU
participant "IWorkflowStorageLogService" as WL
database "MySQL" as DB

OR -> TC : ExecuteAsync(context)
TC -> CF : GetOpenConnectionAsync(alias)
CF --> TC : connection
TC -> DB : BEGIN TRANSACTION (Serializable)
TC -> IA : ReserveAsync(context, connection, tx)
IA --> TC : StorageIdentityReservationResult
TC -> DQ : LockDiskStatusAsync(gabinete, disco)
DQ --> TC : diskStatus
TC -> DQ : UpdateDiskUsageAsync(...)
DQ --> TC : rows=1

opt context.ResolvedOptions.AplicaInventarioDocumental == true
  TC -> IB : Build(context, identity, naming)
  IB --> TC : buildResult
  TC -> IR : InsertAsync(model, connection, tx)
  IR --> TC : idRegistroProduccion
end

opt context.ResolvedOptions.AplicaUnidadConservacion == true
  TC -> EU : ExecuteAsync(context, connection, tx)
  EU --> TC : ExpedienteUnidadLegacyResult
end

opt workflow con IdTareaWorkflow > 0
  TC -> WL : ExecuteAsync(context, identity, naming, physicalPath, connection, tx)
  WL --> TC : OK
end

TC -> DB : COMMIT
TC --> OR : StorageTransactionResult

alt excepcion
  TC -> DB : ROLLBACK
  TC --> OR : throw StorageTransactionException
end
@enduml
```

## 3.5) Secuencia por Clase - StoragePhysicalPhaseExecutor
```plantuml
@startuml
autonumber
participant "Orchestrator" as OR
participant "StoragePhysicalPhaseExecutor" as PE
participant "IStoragePlanBuilder" as PB
participant "IStorageFileWriter" as FW
participant "IStorageXmlBuilder" as XB
participant "IStorageXmlWriter" as XW
participant "IExpedienteIndiceXmlService" as EX
participant "IStorageCompensationManager" as CM

OR -> PE : ExecuteAsync(context, transactionResult)
PE -> PB : BuildFilePlanAsync(context, txResult)
PB --> PE : StorageFilePlanModel
PE -> FW : CopyAsync(filePlan, compensationPlan, requestId)
FW --> PE : primaryFile
PE -> XB : BuildXmlModel(context, txResult)
XB --> PE : xmlModel
PE -> XW : WriteAsync(filePlan, xmlModel, compensationPlan, requestId)
XW --> PE : OK

opt servicio indice expediente configurado
  PE -> EX : ExecuteAsync(context, txResult, naming, physicalPath)
  EX --> PE : ExpedienteIndiceXmlUpdateResult
end

PE --> OR : StoragePhysicalStatusModel{Completed}

alt exception en FS/XML
  PE -> CM : ExecuteAsync(compensationPlan, requestId)
  CM --> PE : OK
  PE --> OR : throw StoragePhysicalException
end
@enduml
```

## 3.6) Secuencia por Clase - StorageCompensationManager
```plantuml
@startuml
autonumber
participant "PhysicalExecutor" as PE
participant "StorageCompensationManager" as CM
participant "FileSystem" as FS

PE -> CM : ExecuteAsync(compensationPlan, requestId)

loop por cada lista de archivos (xml/final/tmp)
  CM -> FS : Exists(path)?
  alt existe
    CM -> FS : Delete(path)
    FS --> CM : OK
  else no existe
    FS --> CM : skip
  end
end

loop directorios creados (descendente)
  CM -> FS : Directory.Exists(dir) && Empty?
  alt vacio
    CM -> FS : Directory.Delete(dir, false)
    FS --> CM : OK
  else no vacio/error
    FS --> CM : warning
  end
end

CM --> PE : Task.CompletedTask
@enduml
```

## 4) Diagrama de Estados
```plantuml
@startuml
[*] --> Recibido
Recibido --> Validando
Validando --> Rechazado : errores validacion
Validando --> ReservadoDB : validacion OK
ReservadoDB --> PersistiendoFisico
PersistiendoFisico --> Completado : copy/xml OK
PersistiendoFisico --> FalloFisico : error IO
FalloFisico --> Compensando
Compensando --> Compensado
Rechazado --> [*]
Completado --> [*]
Compensado --> [*]
@enduml
```

## 5) Diagrama de Componentes
```plantuml
@startuml
skinparam componentStyle rectangle

package "DocuArchi.Api" {
  [AlmacenamientoDocumentalController]
}

package "MiApp.Services" {
  [AlmacenarDocumentoUseCase]
  [DocumentStorageOrchestrator]
  [StorageValidationPipeline]
  [StorageTransactionCoordinator]
  [StoragePhysicalPhaseExecutor]
  [StorageCompensationManager]
}

package "MiApp.Repository" {
  [Repositorios Dapper]
}

database "MySQL" as MYSQL
folder "Storage Filesystem" as FS
file "XML Indice" as XML

[AlmacenamientoDocumentalController] --> [AlmacenarDocumentoUseCase]
[AlmacenarDocumentoUseCase] --> [DocumentStorageOrchestrator]
[DocumentStorageOrchestrator] --> [StorageValidationPipeline]
[DocumentStorageOrchestrator] --> [StorageTransactionCoordinator]
[DocumentStorageOrchestrator] --> [StoragePhysicalPhaseExecutor]
[DocumentStorageOrchestrator] --> [StorageCompensationManager]

[StorageValidationPipeline] --> [Repositorios Dapper]
[StorageTransactionCoordinator] --> [Repositorios Dapper]
[Repositorios Dapper] --> MYSQL

[StoragePhysicalPhaseExecutor] --> FS
[StoragePhysicalPhaseExecutor] --> XML
@enduml
```

## 6) Diagrama de Despliegue
```plantuml
@startuml
node "Client / Integrador" as Client
node "API Host" as ApiHost {
  artifact "DocuArchi.Api"
}
node "App Host" as AppHost {
  artifact "MiApp.Services"
  artifact "MiApp.Repository"
}
database "MySQL" as DB
node "Storage Server" as Storage {
  folder "Documentos"
  file "Indices XML"
}

Client --> ApiHost
ApiHost --> AppHost
AppHost --> DB
AppHost --> Storage
@enduml
```

## 7) Secuencia de Error y Compensacion
```plantuml
@startuml
autonumber
participant "Orchestrator" as O
participant "PhysicalExecutor" as F
participant "FileSystem/XML" as FS
participant "CompensationManager" as C

O -> F : ExecuteAsync(...)
F -> FS : Copy/Write
FS --> F : Exception IO
F --> O : PhysicalFailed
O -> C : ExecuteCompensationAsync(context)
C -> FS : Delete created artifacts
C --> O : CompensationResult
O --> O : Return controlled error
@enduml
```

## Nota de compatibilidad
- Formato: `PlantUML`.
- Compatible con: PlantUML Server, IntelliJ PlantUML, VSCode PlantUML, Azure DevOps (ext), GitLab (plugin).
- Si tu visor no procesa PlantUML embebido, exporta a PNG/SVG desde PlantUML y adjunta los artefactos.
