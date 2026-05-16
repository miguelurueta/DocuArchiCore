# SCRUM-202 - Diagramas Reemplazo PDF (UML / PlantUML)

## Alcance
Documento de arquitectura de la capacidad de reemplazo físico de PDF en gabinete (`Documentos/ReemplazoPdf`) y su integración con upload temporal existente.

## Estado de cierre
- Endpoint dedicado implementado en módulo `Documentos`.
- Reutilización de upload temporal (`EnsureCompletedAsync`) implementada.
- Auditoría transversal en `logdocuarchi` desacoplada mediante `ILogDocuarchiRepository`.

## 1) Diagrama de Casos de Uso
```plantuml
@startuml
left to right direction
actor "Frontend/Integrador" as FE
rectangle "Reemplazo PDF" {
  usecase "Iniciar upload temporal" as UC1
  usecase "Subir chunk" as UC2
  usecase "Completar upload" as UC3
  usecase "Reemplazar PDF almacenado" as UC4
  usecase "Registrar auditoría técnica" as UC5
}
FE --> UC1
FE --> UC2
FE --> UC3
FE --> UC4
UC4 --> UC5
@enduml
```

## 2) Diagrama de Clases (Núcleo Reemplazo)
```plantuml
@startuml
skinparam classAttributeIconSize 0

interface IReemplazoPdfService {
  +ExecuteAsync(request, defaultDbAlias, usuario, usuarioId, ipTrans) : Task<AppResponses<ReemplazarDocumentoPdfResponse?>>
}

interface IReemplazoPdfDocumentLocationRepository {
  +GetLocationByIdAsync(nombreGabinete, idDocumento, defaultDbAlias) : Task<StorageDocumentLocationModel?>
}

interface ILogDocuarchiRepository {
  +InsertAsync(model, defaultDbAlias) : Task<int>
  +InsertBeginTransAsync(model, connection, transaction, usuario) : Task<int>
}

interface IStorageLargeUploadService {
  +EnsureCompletedAsync(rutaTemporalId, archivoTemporalIds, usuarioId) : Task
}

interface IStorageUploadPathResolver {
  +GetFinalFilePath(rutaTemporalId, archivoTemporalId) : string
  +GetTempRoot() : string
}

interface IStorageRouteRepository {
  +GetRouteAsync(nombreGabinete, defaultDbAlias) : Task<StorageRouteModel?>
}

interface IStorageFolderLegacyPolicy {
  +ResolveFolder(rutaBase, nombreGabinete, disco, carpeta) : StorageFolderResolution
}

interface IStoragePathResolver {
  +ResolveSafePath(rootPath, relativePath) : string
}

class ReemplazoPdfService {
  -_largeUploadService : IStorageLargeUploadService
  -_uploadPathResolver : IStorageUploadPathResolver
  -_storagePathResolver : IStoragePathResolver
  -_routeRepository : IStorageRouteRepository
  -_folderPolicy : IStorageFolderLegacyPolicy
  -_locationRepository : IReemplazoPdfDocumentLocationRepository
  -_replacementLogRepository : ILogDocuarchiRepository
}

class ReemplazoPdfController
class ReemplazoPdfDocumentLocationRepository
class LogDocuarchiRepository

IReemplazoPdfService <|.. ReemplazoPdfService
IReemplazoPdfDocumentLocationRepository <|.. ReemplazoPdfDocumentLocationRepository
ILogDocuarchiRepository <|.. LogDocuarchiRepository
ReemplazoPdfController --> IReemplazoPdfService
ReemplazoPdfService --> IStorageLargeUploadService
ReemplazoPdfService --> IStorageUploadPathResolver
ReemplazoPdfService --> IStorageRouteRepository
ReemplazoPdfService --> IStorageFolderLegacyPolicy
ReemplazoPdfService --> IStoragePathResolver
ReemplazoPdfService --> IReemplazoPdfDocumentLocationRepository
ReemplazoPdfService --> ILogDocuarchiRepository
@enduml
```

## 3) Diagrama de Secuencia Integral
```plantuml
@startuml
autonumber
actor "Frontend" as FE
participant "ReemplazoPdfController" as C
participant "IReemplazoPdfService" as S
participant "IStorageLargeUploadService" as U
participant "IStorageUploadPathResolver" as P
participant "IReemplazoPdfDocumentLocationRepository" as L
participant "IStorageRouteRepository" as R
participant "IStorageFolderLegacyPolicy" as F
participant "IStoragePathResolver" as SP
participant "ILogDocuarchiRepository" as A
database "MySQL" as DB
collections "FS" as FS

FE -> C : POST /documentos/reemplazopdf
C -> S : ExecuteAsync(request, alias, usuario, usuarioId, ip)
S -> U : EnsureCompletedAsync(rutaTemporalId, [archivoTemporalId], usuarioId)
S -> P : GetFinalFilePath(rutaTemporalId, archivoTemporalId)
S -> FS : exists(temporal.pdf)
S -> L : GetLocationByIdAsync(gabinete, idDocumento, alias)
L -> DB : SELECT gabinete(ID,DISC,IDEX,PAG,DBT,TIPODOCUMENTO)
S -> R : GetRouteAsync(gabinete, alias)
S -> F : ResolveFolder(ruta, gabinete, disco, carpeta)
S -> SP : ResolveSafePath(root, relative)
S -> FS : locate DIG########.*
S -> FS : backup + overwrite + hash verify
S -> A : InsertAsync(logdocuarchi)
A -> DB : INSERT logdocuarchi
S --> C : AppResponses success/error
C --> FE : 200 / 400 / 500
@enduml
```

## 3.1 Tabla de interacciones principales
| Paso | Origen | Destino | Función | Parámetros clave | Retorno |
|---|---|---|---|---|---|
| 1 | Frontend | Controller | `reemplazopdf` | `NombreGabinete,IdDocumento,RutaTemporalId,ArchivoTemporalId,DescOp,ModuloRegistro,Radicado,IdTareaWorkflow,IdRutaWorkflow,TipologiaDocumental` | `AppResponses` |
| 2 | Service | UploadService | `EnsureCompletedAsync` | `rutaTemporalId,archivoTemporalIds,usuarioId` | `OK/Error` |
| 3 | Service | PathResolver | `GetFinalFilePath` | `rutaTemporalId,archivoTemporalId` | `ruta temporal final` |
| 4 | Service | LocationRepo | `GetLocationByIdAsync` | `gabinete,idDocumento,alias` | `DISC,IDEX,PAG,DBT,TIPODOCUMENTO` |
| 5 | Service | RouteRepo | `GetRouteAsync` | `gabinete,alias` | `ruta almacenamiento` |
| 6 | Service | FolderPolicy | `ResolveFolder` | `ruta,gabinete,disco,carpeta` | `ruta final carpeta` |
| 7 | Service | AuditRepo | `InsertAsync` | `LogDocuarchiEntryModel` | `rows=1` |

## 4) Diagrama de Estados (Reemplazo)
```plantuml
@startuml
[*] --> RECEIVED
RECEIVED --> VALIDATED : request + claims ok
VALIDATED --> TEMPORAL_CONFIRMED : EnsureCompleted
TEMPORAL_CONFIRMED --> TARGET_RESOLVED : gabinete/ruta/archivo DIG
TARGET_RESOLVED --> BACKUP_DONE : backup creado
BACKUP_DONE --> REPLACED : overwrite ok
REPLACED --> AUDITED : insert logdocuarchi ok
AUDITED --> COMPLETED

RECEIVED --> FAILED : validacion
VALIDATED --> FAILED : temporal invalido
TEMPORAL_CONFIRMED --> FAILED : ruta/archivo no existe
REPLACED --> FAILED : hash mismatch
AUDITED --> FAILED : log error

FAILED --> [*]
COMPLETED --> [*]
@enduml
```

## 5) Diagrama de Componentes
```plantuml
@startuml
skinparam componentStyle rectangle

package "DocuArchi.Api" {
  [ReemplazoPdfController]
}

package "MiApp.Services" {
  [ReemplazoPdfService]
  [StorageLargeUploadService]
  [StorageUploadPathResolver]
  [StorageFolderLegacyPolicy]
  [StoragePathResolver]
}

package "MiApp.Repository" {
  [ReemplazoPdfDocumentLocationRepository]
  [LogDocuarchiRepository]
  [StorageRouteRepository]
}

database "MySQL" as DB
folder "FS Temp / FS Final" as FS

[ReemplazoPdfController] --> [ReemplazoPdfService]
[ReemplazoPdfService] --> [StorageLargeUploadService]
[ReemplazoPdfService] --> [StorageUploadPathResolver]
[ReemplazoPdfService] --> [StorageFolderLegacyPolicy]
[ReemplazoPdfService] --> [StoragePathResolver]
[ReemplazoPdfService] --> [ReemplazoPdfDocumentLocationRepository]
[ReemplazoPdfService] --> [StorageRouteRepository]
[ReemplazoPdfService] --> [LogDocuarchiRepository]

[ReemplazoPdfDocumentLocationRepository] --> DB
[StorageRouteRepository] --> DB
[LogDocuarchiRepository] --> DB
[ReemplazoPdfService] --> FS
@enduml
```

## 6) Diagrama de Despliegue
```plantuml
@startuml
node "Cliente Web" as FE
node "IIS / ASP.NET Core Host" as API {
  artifact "DocuArchi.Api.dll"
}
node "Servicio Reemplazo PDF" as SVC {
  artifact "MiApp.Services.dll"
}
database "MySQL" as MYSQL
node "Servidor de archivos" as FILES {
  folder "StoragePaths:Temp"
  folder "Rutas gabinete"
}

FE --> API
API --> SVC
SVC --> MYSQL
SVC --> FILES
@enduml
```

## 7) Nota de compatibilidad
- Formato: `PlantUML`.
- Compatible con VSCode/IntelliJ/PlantUML Server.
- Flujo recomendado frontend: `init -> chunk -> complete -> reemplazopdf`.
