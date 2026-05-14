# SCRUM-199 - Implementacion DA_EXTENSION por ESTENSION

## Objetivo
- Corregir la clasificación técnica del archivo para Storage Engine.
- Eliminar la dependencia incorrecta de `TRD.IdTipoDocumento` para resolver extensión técnica.
- Validar temprano (antes de transacción) cuando no existe mapeo en `DA_EXTENSION`.

## Cambios implementados

### 1. Resolución técnica por extensión real
- Archivo: `MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Extension/IStorageExtensionRepository.cs`
- Cambio:
  - Antes: búsqueda por `ESTADO_NORMAL/ESTADO_ADJUNTO/ESTADO_LINK = TRD.IdTipoDocumento`.
  - Ahora: búsqueda por `ESTENSION = <extension-normalizada>`.
- Salida técnica propagada:
  - `ESTADO_NORMAL`
  - `ESTADO_ADJUNTO`
  - `ESTADO_LINK`
  - extensión normalizada para nombre final (`.pdf`, `.tif`, etc.).
- Compatibilidad aplicada:
  - acepta valores en `DA_EXTENSION.ESTENSION` con y sin punto (`PDF` / `.PDF`),
  - comparación case-insensitive sin hardcode por tipo específico.

### 2. Nuevo contrato de clasificación técnica
- Archivo nuevo: `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageExtensionClassificationModel.cs`
- Campos:
  - `ExtensionNormalizada`
  - `EstadoNormal`
  - `EstadoAdjunto`
  - `EstadoLink`

### 3. Propagación al contexto del flujo
- Archivo: `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageContext.cs`
- Campo agregado:
  - `ExtensionClassification`

### 4. Validación funcional temprana (pre-transacción)
- Archivo nuevo: `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Validation/TechnicalExtensionValidator.cs`
- Orden de ejecución: `35`.
- Reglas:
  - Si no hay extensión en documento principal: `DOC_EXTENSION_REQUIRED`.
  - Si no existe fila en `DA_EXTENSION`: `EXTENSION_MAPPING_NOT_FOUND`.
- Resultado:
  - El flujo falla en validación y no avanza a commit por esta causa.
  - Se evita avance a fase física cuando no existe mapeo técnico de extensión.

### 5. Naming físico
- Archivo: `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Builders/StoragePlanBuilder.cs`
- Cambio:
  - Antes: resolvía naming por `TRD.IdTipoDocumento`.
  - Ahora: usa `context.ExtensionClassification.ExtensionNormalizada` (con fallback controlado al resolver por extensión si aún no está en contexto).

### 6. Registro DI del nuevo validator
- Archivo: `DocuArchi.Api/Program.cs`
- Registro agregado:
  - `TechnicalExtensionValidator` como `IStorageValidator`.

## Relación de campos base obligatorios de gabinete (contabil)

| Campo `contabil` | Origen actual en modelo/repositorio | Regla |
|---|---|---|
| `ID` | `GabineteInsertModel.IdAlmacen` | Obligatorio |
| `DISC` | `GabineteInsertModel.Disco` | Obligatorio |
| `PAG` | `GabineteInsertModel.Paginas` | Obligatorio |
| `DBT` | `GabineteInsertModel.TipoDocumento` | Debe venir de `DA_EXTENSION.ESTADO_NORMAL` |
| `IDEX` | `GabineteInsertModel.Carpeta` | Obligatorio |
| `USER` | `GabineteInsertModel.Usuario` | Obligatorio |
| `DATE1` | `GabineteInsertModel.Fecha` | Obligatorio |
| `TIME1` | `GabineteInsertModel.Hora` | Obligatorio (`NOT NULL`) |

## Ajuste adicional aplicado para `TIME1`
- Archivo: `MiApp.Models/.../GabineteInsertModel.cs`
  - Nuevo campo requerido: `Hora`.
- Archivo: `MiApp.Repository/.../Gabinete/IGabineteStorageRepository.cs`
  - Validación de `Hora`.
  - Inserción explícita de columna `TIME1`.

## Pruebas ejecutadas
- `StorageNamingServiceTests` (actualizado a resolución por extensión).
- `StorageValidationPipelineTests` (nuevas pruebas de `TechnicalExtensionValidator`).
- `GabineteStorageRepositoryTests` (incluye validación de `TIME1`).

## Nota arquitectónica importante
- En el estado actual del flujo, `StorageTransactionCoordinator` no invoca aún la inserción de gabinete en esta rama; por eso el ajuste de `DBT <- ESTADO_NORMAL` queda preparado a nivel de clasificación técnica y contrato de datos para el momento de integración/invocación de inserción de gabinete.
