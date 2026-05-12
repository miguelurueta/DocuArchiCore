# SCRUM-196 - Diagramas Validación StorageEngine (UML / PlantUML)

## Alcance
Documento de arquitectura de la validación del flujo `POST /almacenamiento` en Storage Engine, incluyendo orden de validadores, consulta de metadata de gabinete, validación de tipos/longitudes y guía de depuración.

## Estado de cierre
- Pipeline de validación activo en `DocumentStorageOrchestrator`.
- Matriz de validadores registrada por DI y ejecutada por `Order`.
- Validación de metadata de gabinete aplicada (campo requerido, tipo, longitud, compatibilidad de esquema).

## 1) Diagrama de Casos de Uso
```plantuml
@startuml
left to right direction
actor "Frontend/Integrador" as FE
actor "Operador Soporte" as OPS
rectangle "Storage Validation Engine" {
  usecase "Enviar solicitud de almacenamiento" as UC1
  usecase "Validar estructura del request" as UC2
  usecase "Validar reglas de documento y negocio" as UC3
  usecase "Validar metadata de gabinete" as UC4
  usecase "Rechazar con errores normalizados" as UC5
  usecase "Depurar por requestId y etapa" as UC6
}
FE --> UC1
UC1 --> UC2
UC1 --> UC3
UC1 --> UC4
UC2 --> UC5
UC3 --> UC5
UC4 --> UC5
OPS --> UC6
@enduml
```

## 2) Diagrama de Clases (Núcleo Validación)
```plantuml
@startuml
skinparam classAttributeIconSize 0

interface IStorageValidationPipeline {
  +ValidateAsync(context) : Task<StorageValidationResult>
}

interface IStorageValidator {
  +Order : int
  +ValidateAsync(context, errors) : Task
}

interface IStorageGabineteMetadataProvider {
  +GetFieldsAsync(nombreGabinete, defaultDbAlias) : Task<IReadOnlyList<GabineteFieldMetadata>>
}

interface IStorageGabineteMetadataRepository {
  +GetFieldsAsync(nombreGabinete, alias) : Task<IReadOnlyList<GabineteFieldMetadata>>
  +GetPhysicalColumnsAsync(nombreTabla, alias) : Task<IReadOnlyList<StoragePhysicalColumnMetadata>>
}

class StorageValidationPipeline
class RequestStructureValidator
class DocumentoValidator
class CamposValidator
class TipoAlmacenamientoValidator
class ReglasBasicasValidator
class PreindexValidator
class GabineteRequiredFieldsValidator {
  +ValidateAsync(context, errors)
  -ValidateTypeAndLength(metadata, value, errors)
  -IsCompatibleValue(normalizedType, value) : bool
}
class StorageOptionsValidator
class TrdRulesValidator
class ExpedienteUnidadRulesValidator
class StorageGabineteMetadataProvider
class StorageGabineteMetadataRepository
class DocumentStorageOrchestrator

IStorageValidationPipeline <|.. StorageValidationPipeline
IStorageValidator <|.. RequestStructureValidator
IStorageValidator <|.. DocumentoValidator
IStorageValidator <|.. CamposValidator
IStorageValidator <|.. TipoAlmacenamientoValidator
IStorageValidator <|.. ReglasBasicasValidator
IStorageValidator <|.. PreindexValidator
IStorageValidator <|.. GabineteRequiredFieldsValidator
IStorageValidator <|.. StorageOptionsValidator
IStorageValidator <|.. TrdRulesValidator
IStorageValidator <|.. ExpedienteUnidadRulesValidator
IStorageGabineteMetadataProvider <|.. StorageGabineteMetadataProvider
IStorageGabineteMetadataRepository <|.. StorageGabineteMetadataRepository

DocumentStorageOrchestrator --> IStorageValidationPipeline
StorageValidationPipeline --> IStorageValidator : Order ASC
GabineteRequiredFieldsValidator --> IStorageGabineteMetadataProvider
StorageGabineteMetadataProvider --> IStorageGabineteMetadataRepository
@enduml
```

## 3) Diagrama de Secuencia (Orquestador + Pipeline + Metadata)
```plantuml
@startuml
autonumber
actor "Frontend" as FE
participant "AlmacenamientoDocumentalController" as C
participant "AlmacenarDocumentoUseCase" as UC
participant "DocumentStorageOrchestrator" as O
participant "IStorageValidationPipeline" as VP
participant "RequestStructureValidator (10)" as V10
participant "DocumentoValidator (20)" as V20
participant "CamposValidator (30)" as V30
participant "TipoAlmacenamientoValidator (40)" as V40
participant "ReglasBasicasValidator (50)" as V50
participant "PreindexValidator (60)" as V60
participant "GabineteRequiredFieldsValidator" as GV
participant "StorageOptionsValidator (80)" as V80
participant "TrdRulesValidator (90)" as V90
participant "ExpedienteUnidadRulesValidator (100)" as V100
participant "IStorageGabineteMetadataProvider" as MP
participant "IStorageGabineteMetadataRepository" as MR
database "MySQL" as DB

FE -> C : POST /almacenamiento
C -> UC : ExecuteAsync(...)
UC -> O : ExecuteAsync(context)
O -> VP : ValidateAsync(context)

group Order 10
  VP -> V10 : ValidateAsync(context, errors)
  V10 --> VP : errores estructura (si aplica)
end

group Order 20
  VP -> V20 : ValidateAsync(context, errors)
  V20 --> VP : errores documento (si aplica)
end

group Order 30
  VP -> V30 : ValidateAsync(context, errors)
  V30 --> VP : errores campos (si aplica)
end

group Order 40
  VP -> V40 : ValidateAsync(context, errors)
  V40 --> VP : errores tipo almacenamiento (si aplica)
end

group Order 50
  VP -> V50 : ValidateAsync(context, errors)
  V50 --> VP : errores reglas basicas (si aplica)
end

group Order 60
  VP -> V60 : ValidateAsync(context, errors)
  V60 --> VP : errores preindex (si aplica)
end

group Order 70 (Metadata gabinete)
VP -> GV : ValidateAsync(context, errors)
GV -> MP : GetFieldsAsync(nombreGabinete, alias)
MP -> MR : GetFieldsAsync(...)
MR -> DB : SELECT DETALLE_GABIENETE...
MR --> MP : metadata campos
MP -> MR : GetPhysicalColumnsAsync(...) (opcional por config)
MR -> DB : SELECT INFORMATION_SCHEMA.COLUMNS...
MR --> MP : columnas fisicas
MP --> GV : metadata normalizada/cacheada
GV -> GV : ValidateTypeAndLength(...)
GV -> GV : IsCompatibleValue(...)
GV --> VP : agrega errores (si aplica)
end

group Order 80
  VP -> V80 : ValidateAsync(context, errors)
  V80 --> VP : errores opciones storage (si aplica)
end

group Order 90
  VP -> V90 : ValidateAsync(context, errors)
  V90 --> VP : errores TRD (si aplica)
end

group Order 100
  VP -> V100 : ValidateAsync(context, errors)
  V100 --> VP : errores expediente/unidad (si aplica)
end

note over VP
El pipeline ejecuta todos los validadores.
No hay short-circuit por primer error.
end note

alt errors.Count final > 0
  VP --> O : StorageValidationResult(IsValid=false)
  O --> UC : throw StorageValidationException
  UC --> C : AppResponses validation error
  C --> FE : 400/422
else valido
  VP --> O : StorageValidationResult(IsValid=true)
  O --> UC : continuar fases transaccional/fisica
end
@enduml
```

## 3.1 Matriz de validadores (orden real)
| Order | Clase | Objetivo |
|---|---|---|
| 10 | `RequestStructureValidator` | Integridad estructural inicial del request |
| 20 | `DocumentoValidator` | Reglas de documentos enviados |
| 30 | `CamposValidator` | Reglas base de campos de indexación |
| 40 | `TipoAlmacenamientoValidator` | Compatibilidad tipo de almacenamiento |
| 50 | `ReglasBasicasValidator` | Reglas funcionales generales |
| 60 | `PreindexValidator` | Reglas de preindexación |
| 70 | `GabineteRequiredFieldsValidator` | Metadata de gabinete: requerido/tipo/longitud/esquema |
| 80 | `StorageOptionsValidator` | Opciones/configuración de storage |
| 90 | `TrdRulesValidator` | Reglas TRD |
| 100 | `ExpedienteUnidadRulesValidator` | Reglas de expediente/unidad |

## 3.2 Validaciones y alcance funcional
| Order | Validador | Qué valida | Errores típicos |
|---|---|---|---|
| 10 | `RequestStructureValidator` | Existencia de `StorageContext` y `Command`. | `CTX_REQUIRED`, `COMMAND_REQUIRED` |
| 20 | `DocumentoValidator` | Que exista al menos un documento y que cada documento tenga `ArchivoTemporalId`. | `DOC_REQUIRED`, `DOC_TEMP_ID_REQUIRED` |
| 30 | `CamposValidator` | Consistencia mínima de campos de indexación (nombre de campo obligatorio). | `CAMPO_NOMBRE_REQUIRED` |
| 40 | `TipoAlmacenamientoValidator` | Que `TipoAlmacenamiento` esté dentro de los valores soportados del dominio. | `TIPO_ALMACENAMIENTO_INVALID` |
| 50 | `ReglasBasicasValidator` | Reglas base de entrada: `NombreGabinete`, `RutaTemporalId`, `NombreDocumento`. | `NOMBRE_GABINETE_REQUIRED`, `RUTA_TEMP_REQUIRED`, `NOMBRE_DOCUMENTO_REQUIRED` |
| 60 | `PreindexValidator` | Integración con preindex: existencia/lectura de archivo, formato, cantidad de valores y mapeo contra campos de indexación. | `PREINDEX_NOT_FOUND`, `PREINDEX_PATH_INVALID`, `PREINDEX_FIELDS_EMPTY`, `PREINDEX_FIELDS_MISMATCH`, `PREINDEX_INVALID_FORMAT`, `PREINDEX_READ_ERROR` |
| 70 | `GabineteRequiredFieldsValidator` | Consulta metadata de gabinete, valida campos desconocidos, campos obligatorios, compatibilidad de esquema físico, tipo de dato y longitud máxima por campo. | `GAB_FIELDS_NOT_FOUND`, `GAB_FIELD_UNKNOWN`, `GAB_SCHEMA_MISMATCH`, `GAB_REQUIRED_EMPTY`, `GAB_TYPE_UNSUPPORTED`, `GAB_FIELD_TYPE_INVALID`, `GAB_FIELD_LENGTH_INVALID` |
| 80 | `StorageOptionsValidator` | Reglas activadas por opciones legacy del gabinete (por ejemplo requerir inventario y datos obligatorios asociados). | `INV_REQUIRED`, `INV_USER_REQUIRED`, `INV_EMPRESA_REQUIRED`, `STORAGE_OPTIONS` |
| 90 | `TrdRulesValidator` | Si TRD aplica según configuración, valida presencia y rango de `IdArea`, `IdSerie`, `IdSubSerie`, `IdTipoDocumento`. | `TRD_REQUIRED`, `TRD_AREA_REQUIRED`, `TRD_SERIE_REQUIRED`, `TRD_TIPO_DOCUMENTO_REQUIRED`, `TRD_INVALID_AREA`, `TRD_INVALID_SERIE`, `TRD_INVALID_SUBSERIE`, `TRD_INVALID_TIPO_DOCUMENTO` |
| 100 | `ExpedienteUnidadRulesValidator` | Reglas de expediente/unidad: obligatoriedad por configuración, exclusión mutua, ids válidos y requerimiento de clase documental según caso. | `EXP_UNI_REQUIRED`, `EXP_UNI_AMBIGUO`, `EXP_UNI_INVALID`, `EXP_CLASE_REQUIRED`, `UNI_CLASE_REQUIRED` |

## 4) Diagrama de Estados (Resultado de validación)
```plantuml
@startuml
[*] --> IN_PROGRESS
IN_PROGRESS --> INVALID : errors.Count > 0
IN_PROGRESS --> VALID : errors.Count == 0
INVALID --> [*]
VALID --> [*]
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
  [RequestStructureValidator]
  [DocumentoValidator]
  [CamposValidator]
  [TipoAlmacenamientoValidator]
  [ReglasBasicasValidator]
  [PreindexValidator]
  [GabineteRequiredFieldsValidator]
  [StorageOptionsValidator]
  [TrdRulesValidator]
  [ExpedienteUnidadRulesValidator]
  [StorageGabineteMetadataProvider]
}

package "MiApp.Repository" {
  [StorageGabineteMetadataRepository]
}

database "MySQL StorageEngine" as DB

[AlmacenamientoDocumentalController] --> [AlmacenarDocumentoUseCase]
[AlmacenarDocumentoUseCase] --> [DocumentStorageOrchestrator]
[DocumentStorageOrchestrator] --> [StorageValidationPipeline]
[StorageValidationPipeline] --> [GabineteRequiredFieldsValidator]
[GabineteRequiredFieldsValidator] --> [StorageGabineteMetadataProvider]
[StorageGabineteMetadataProvider] --> [StorageGabineteMetadataRepository]
[StorageGabineteMetadataRepository] --> DB
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
node "Repositorio" as REP {
  artifact "MiApp.Repository.dll"
}
database "MySQL" as MYSQL

FE --> API
API --> SVC
SVC --> REP
REP --> MYSQL
@enduml
```

## 7) Guía de depuración operativa
1. Generar/ubicar `requestId` de la solicitud y filtrar todo el log por ese valor.
2. Ver inicio/fin del pipeline para validar duración total y conteo final de errores.
3. Trazar por `Order` (10 a 100) e identificar en qué validador aumenta `errors.Count`.
4. Si falla en `Order=70`, validar:
   - `nombreGabinete` y `alias` en `StorageContext`.
   - retorno de `GetFieldsAsync` (vacío dispara `GAB_FIELDS_NOT_FOUND`).
   - discrepancias de campo (`GAB_FIELD_UNKNOWN`).
   - incompatibilidad de esquema físico (`GAB_SCHEMA_MISMATCH`).
   - tipo inválido (`GAB_FIELD_TYPE_INVALID`) o longitud inválida (`GAB_FIELD_LENGTH_INVALID`).
5. Confirmar si `StorageMetadata:ValidatePhysicalSchema` está activo y si existe cache previa que deba expirar.
6. Reproducir con payload mínimo e ir agregando campos de indexación uno a uno.

## 8) Funciones críticas de seguimiento
- `DocumentStorageOrchestrator.ExecuteAsync`: punto de entrada de validación.
- `StorageValidationPipeline.ValidateAsync`: ejecuta secuencia completa.
- `GabineteRequiredFieldsValidator.ValidateAsync`: valida campos contra metadata.
- `GabineteRequiredFieldsValidator.ValidateTypeAndLength`: valida tipo y tamaño.
- `GabineteRequiredFieldsValidator.IsCompatibleValue`: regla de parseo por tipo.
- `StorageGabineteMetadataProvider.GetFieldsAsync`: cache/normalización y schema check opcional.
- `StorageGabineteMetadataRepository.GetFieldsAsync`: consulta `DETALLE_GABIENETE`.
- `StorageGabineteMetadataRepository.GetPhysicalColumnsAsync`: consulta `INFORMATION_SCHEMA.COLUMNS`.

## 9) Nota de compatibilidad
- Formato: `PlantUML`.
- Compatible con VSCode/IntelliJ/PlantUML Server.
- Documento guía para soporte y desarrollo al depurar validaciones de `POST /almacenamiento`.
