# SCRUM-204 - Diagramas Visualizacion Documento (UML / PlantUML)

## Alcance
Arquitectura de visualización documental (`Documentos/VisualizacionDocumento`) con token temporal y conversión TIF->PDF interna.

## 1) Diagrama de Casos de Uso
```plantuml
@startuml
left to right direction
actor "Frontend/Integrador" as FE
rectangle "Visualizacion Documento" {
  usecase "Resolver documento visualizable" as UC1
  usecase "Descargar archivo temporal" as UC2
  usecase "Convertir TIF a PDF (interno)" as UC3
}
FE --> UC1
FE --> UC2
UC1 --> UC3
@enduml
```

## 2) Diagrama de Clases
```plantuml
@startuml
skinparam classAttributeIconSize 0

interface IVisualizacionDocumentoService {
  +ResolveAsync(request, alias, usuarioId)
  +TryResolveDownload(token, usuarioId)
}

interface ITifToPdfConverterService {
  +BuildTemporaryPdfFromTiffAsync(gabinete, idDocumento, sourceFiles)
}

interface IReemplazoPdfDocumentLocationRepository {
  +GetLocationByIdAsync(nombreGabinete, idDocumento, alias)
}

interface IStorageRouteRepository {
  +GetRouteAsync(nombreGabinete, alias)
}

class VisualizacionDocumentoController
class VisualizacionDocumentoService
class TifToPdfConverterService

IVisualizacionDocumentoService <|.. VisualizacionDocumentoService
ITifToPdfConverterService <|.. TifToPdfConverterService
VisualizacionDocumentoController --> IVisualizacionDocumentoService
VisualizacionDocumentoService --> IReemplazoPdfDocumentLocationRepository
VisualizacionDocumentoService --> IStorageRouteRepository
VisualizacionDocumentoService --> ITifToPdfConverterService
@enduml
```

## 3) Diagrama de Secuencia
```plantuml
@startuml
autonumber
actor Front as Frontend
participant C as VisualizacionDocumentoController
participant S as VisualizacionDocumentoService
participant L as IReemplazoPdfDocumentLocationRepository
participant R as IStorageRouteRepository
participant CV as ITifToPdfConverterService
participant FS as FileSystem
participant MC as MemoryCache

Front -> C : POST /visualizacion/resolve
C -> S : ResolveAsync(request, alias, usuarioId)
S -> L : GetLocationByIdAsync(gabinete,id,alias)
S -> R : GetRouteAsync(gabinete,alias)
S -> FS : localizar DIG########.*
alt origen TIF/TIFF
  S -> CV : BuildTemporaryPdfFromTiffAsync(...)
  CV -> FS : genera PDF temporal
end
S -> MC : guardar token temporal (TTL)
S --> C : AppResponses(data.UrlTemporal)
C --> Front : 200

Front -> C : GET /visualizacion/download/{token}
C -> S : TryResolveDownload(token, usuarioId)
S -> MC : resolver token
S --> C : filePath/contentType
C --> Front : archivo binario
@enduml
```

## 4) Diagrama de Estados
```plantuml
@startuml
[*] --> RECEIVED
RECEIVED --> VALIDATED : claims + request ok
VALIDATED --> LOCATED : gabinete/ruta/archivo
LOCATED --> CONVERTED : origen TIF
LOCATED --> TOKENIZED : origen ORIGINAL
CONVERTED --> TOKENIZED
TOKENIZED --> COMPLETED

VALIDATED --> FAILED : validacion
LOCATED --> FAILED : documento/ruta no existe
CONVERTED --> FAILED : conversion TIF->PDF
FAILED --> [*]
COMPLETED --> [*]
@enduml
```