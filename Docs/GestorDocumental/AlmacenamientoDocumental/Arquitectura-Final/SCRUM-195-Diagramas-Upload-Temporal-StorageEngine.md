# SCRUM-195 - Diagramas Upload Temporal StorageEngine (UML / PlantUML)

## Alcance
Documento de arquitectura de la capacidad de upload temporal por streaming/chunks y su integración con `POST /almacenamiento` del Storage Engine.

## Estado de cierre
- Contrato API de upload temporal implementado.
- Validación `COMPLETED` integrada en `AlmacenarDocumentoUseCase`.
- Política de validación aislada en `IStorageUploadPolicy`.

## 1) Diagrama de Casos de Uso
```plantuml
@startuml
left to right direction
actor "Frontend/Integrador" as FE
rectangle "Storage Upload Temporal" {
  usecase "Iniciar upload temporal" as UC1
  usecase "Subir chunk" as UC2
  usecase "Consultar estado upload" as UC3
  usecase "Completar upload" as UC4
  usecase "Cancelar upload" as UC5
  usecase "Almacenar documento con referencia temporal" as UC6
}
FE --> UC1
FE --> UC2
FE --> UC3
FE --> UC4
FE --> UC5
FE --> UC6
@enduml
```

## 2) Diagrama de Clases (Núcleo Upload Temporal)
```plantuml
@startuml
skinparam classAttributeIconSize 0

interface IStorageLargeUploadService {
  +InitAsync(request, usuarioId) : Task<StorageUploadInitResult>
  +UploadChunkAsync(rutaTemporalId, archivoTemporalId, chunkIndex, totalChunks, contentLength, content, usuarioId) : Task
  +GetStatusAsync(rutaTemporalId, archivoTemporalId, usuarioId) : Task<StorageUploadStatusResult>
  +CompleteAsync(rutaTemporalId, archivoTemporalId, usuarioId) : Task
  +CancelAsync(rutaTemporalId, archivoTemporalId, usuarioId) : Task
  +EnsureCompletedAsync(rutaTemporalId, archivoTemporalIds, usuarioId) : Task
}

interface IStorageUploadPolicy {
  +ValidateInitAndNormalizeExtension(request, usuarioId, options) : string
  +NormalizeExpectedHash(hash) : string?
  +ValidateInProgress(metadata) : void
  +ValidateChunk(metadata, chunkIndex, totalChunks, contentLength, options) : void
  +ValidateCompletedFileSize(length, expectedLength) : void
  +ValidateCompletedHash(expectedHash, actualHash) : void
}

interface IStorageUploadPathResolver {
  +GetUploadRoot() : string
  +GetSessionRoot(rutaTemporalId) : string
  +GetChunksFolder(rutaTemporalId) : string
  +GetFinalFolder(rutaTemporalId) : string
  +GetChunkPath(rutaTemporalId, chunkIndex) : string
  +GetFinalFilePath(rutaTemporalId, archivoTemporalId) : string
}

interface IStorageUploadSessionStore {
  +CreateAsync(metadata) : Task
  +GetAsync(rutaTemporalId, archivoTemporalId) : Task<StorageTemporaryUploadMetadata?>
  +SaveAsync(metadata) : Task
  +DeleteAsync(rutaTemporalId) : Task
}

class StorageLargeUploadService {
  -_sessionStore : IStorageUploadSessionStore
  -_pathResolver : IStorageUploadPathResolver
  -_policy : IStorageUploadPolicy
  -_options : StorageUploadOptions
}

class StorageUploadPolicy
class StorageUploadPathResolver
class StorageUploadSessionStore

class StorageTemporaryUploadMetadata {
  +RutaTemporalId : string
  +ArchivoTemporalId : string
  +UsuarioId : int
  +NombreOriginal : string
  +Extension : string
  +TamanoBytesEsperado : long
  +TamanoBytesRecibido : long
  +HashSha256Esperado : string?
  +HashSha256Calculado : string?
  +NumeroChunks : int
  +ChunksRecibidos : List<int>
  +Estado : string
}

class AlmacenarDocumentoUseCase {
  -_largeUploadService : IStorageLargeUploadService
  +ExecuteAsync(request, alias, usuario, usuarioId, ipTrans) : Task<AppResponses<AlmacenarDocumentoResponse?>>
}

IStorageLargeUploadService <|.. StorageLargeUploadService
IStorageUploadPolicy <|.. StorageUploadPolicy
IStorageUploadPathResolver <|.. StorageUploadPathResolver
IStorageUploadSessionStore <|.. StorageUploadSessionStore
StorageLargeUploadService --> StorageTemporaryUploadMetadata
AlmacenarDocumentoUseCase --> IStorageLargeUploadService
@enduml
```

## 3) Diagrama de Secuencia Integral (Frontend + Upload + Almacenamiento)
```plantuml
@startuml
autonumber
actor "Frontend" as FE
participant "AlmacenamientoDocumentalController" as C
participant "IStorageLargeUploadService" as U
participant "IStorageUploadPolicy" as P
participant "IStorageUploadSessionStore" as S
participant "IStorageUploadPathResolver" as R
participant "AlmacenarDocumentoUseCase" as UC
participant "IDocumentStorageOrchestrator" as O
database "MySQL" as DB
collections "FS Temp" as FS

FE -> C : POST /upload-temporal/init(request)
C -> U : InitAsync(request, usuarioId)
U -> P : ValidateInitAndNormalizeExtension(...)
U -> S : CreateAsync(metadata)
U --> C : StorageUploadInitResult
C --> FE : rutaTemporalId + archivoTemporalId + chunkSize

loop por cada chunk
  FE -> C : PUT /chunk/{chunkIndex} (bytes)
  C -> U : UploadChunkAsync(...)
  U -> S : GetAsync(rutaTemporalId, archivoTemporalId)
  U -> P : ValidateInProgress(metadata)
  U -> P : ValidateChunk(metadata, chunkIndex,...)
  U -> R : GetChunkPath(...)
  U -> FS : write chunk
  U -> S : SaveAsync(metadata)
  U --> C : OK
  C --> FE : 200
end

FE -> C : GET /status
C -> U : GetStatusAsync(...)
U -> S : GetAsync(...)
U --> C : StorageUploadStatusResult
C --> FE : chunksRecibidos/chunksPendientes

FE -> C : POST /complete
C -> U : CompleteAsync(...)
U -> S : GetAsync(...)
U -> P : ValidateInProgress(metadata)
U -> R : GetFinalFilePath(...)
U -> FS : ensamblar chunks + hash
U -> P : ValidateCompletedFileSize(...)
U -> P : ValidateCompletedHash(...)
U -> S : SaveAsync(estado=COMPLETED)
U --> C : OK
C --> FE : 200

FE -> C : POST /almacenamiento(request referencias)
C -> UC : ExecuteAsync(request, alias, usuario, usuarioId, ip)
UC -> U : EnsureCompletedAsync(rutaTemporalId, archivoTemporalIds, usuarioId)
U -> S : GetAsync(...) por cada archivo
U -> R : GetFinalFilePath(...)
U -> FS : exists(file)
U --> UC : OK
UC -> O : ExecuteAsync(context)
O -> DB : transaccion storage engine
O --> UC : AlmacenarDocumentoResult
UC --> C : AppResponses success
C --> FE : 200 OK
@enduml
```

## 3.1 Tabla de interacciones principales
| Paso | Origen | Destino | Función | Parámetros clave | Retorno |
|---|---|---|---|---|---|
| 1 | Frontend | Controller | `init` | `nombreOriginal,tamanoBytes,extension,numeroChunks` | `rutaTemporalId,archivoTemporalId,chunkSize` |
| 2 | Frontend | Controller | `chunk` | `rutaTemporalId,archivoTemporalId,chunkIndex,bytes` | `OK` |
| 3 | Frontend | Controller | `status` | `rutaTemporalId,archivoTemporalId` | `estado,chunks` |
| 4 | Frontend | Controller | `complete` | `rutaTemporalId,archivoTemporalId` | `OK` |
| 5 | Frontend | Controller | `almacenamiento` | `rutaTemporalId,documentos[].archivoTemporalId` | `idAlmacen,requestId` |
| 6 | UseCase | UploadService | `EnsureCompletedAsync` | `rutaTemporalId,archivoTemporalIds,usuarioId` | `OK/Error` |
| 7 | UseCase | Orchestrator | `ExecuteAsync` | `StorageContext` | `AlmacenarDocumentoResult` |

## 4) Diagrama de Estados (Sesión temporal)
```plantuml
@startuml
[*] --> IN_PROGRESS
IN_PROGRESS --> COMPLETED : complete ok
IN_PROGRESS --> FAILED : hash/size/chunk error
IN_PROGRESS --> CANCELLED : cancel
IN_PROGRESS --> EXPIRED : ttl cleanup
FAILED --> [*]
COMPLETED --> [*]
CANCELLED --> [*]
EXPIRED --> [*]
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
  [StorageLargeUploadService]
  [StorageUploadPolicy]
  [StorageUploadPathResolver]
  [StorageUploadSessionStore]
  [AlmacenarDocumentoUseCase]
  [DocumentStorageOrchestrator]
}

database "MySQL StorageEngine" as DB
folder "FS Temp" as TEMP

[AlmacenamientoDocumentalController] --> [StorageLargeUploadService]
[StorageLargeUploadService] --> [StorageUploadPolicy]
[StorageLargeUploadService] --> [StorageUploadPathResolver]
[StorageLargeUploadService] --> [StorageUploadSessionStore]
[StorageUploadSessionStore] --> TEMP
[AlmacenarDocumentoUseCase] --> [StorageLargeUploadService]
[AlmacenarDocumentoUseCase] --> [DocumentStorageOrchestrator]
[DocumentStorageOrchestrator] --> DB
@enduml
```

## 6) Diagrama de Despliegue
```plantuml
@startuml
node "Cliente Web" as FE
node "IIS / ASP.NET Core Host" as API {
  artifact "DocuArchi.Api.dll"
}
node "Servicio Storage" as SVC {
  artifact "MiApp.Services.dll"
}
database "MySQL" as MYSQL
node "Servidor de archivos" as FILES {
  folder "StoragePaths:Temp"
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
- Documento guía para integración frontend: usar siempre flujo `init -> chunk -> status -> complete -> almacenamiento`.
