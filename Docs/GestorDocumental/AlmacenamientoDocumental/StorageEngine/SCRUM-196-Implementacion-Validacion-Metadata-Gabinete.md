# SCRUM-196 - Implementación Validación Metadata de Gabinete

## 1. Objetivo técnico
Implementar resolución real de metadata documental por gabinete y aplicar validación estructural backend en el pipeline de almacenamiento antes de persistir.

Resultado esperado:
- El backend deja de depender de metadata placeholder.
- Se valida existencia de campo, obligatoriedad, tipo y longitud.
- Se soporta validación opcional de consistencia metadata vs esquema físico.

## 2. Problema corregido
Situación previa:
- `StorageGabineteMetadataProvider` retornaba lista vacía.
- `GabineteRequiredFieldsValidator` terminaba en `GAB_FIELDS_NOT_FOUND`.
- No existía validación robusta de tipo/longitud por metadata.

## 3. Alcance implementado
- Resolución real desde `DETALLE_GABIENETE`.
- Normalización de tipo legacy (`VARCHAR`, `CHAR`, `INT`, `DATE`, `DATETIME`, `TEXT`, `LONGTEXT`).
- Cache por `alias + gabinete` con TTL configurable.
- Validación opcional contra `INFORMATION_SCHEMA.COLUMNS`.
- Nuevos códigos de validación:
  - `GAB_FIELD_UNKNOWN`
  - `GAB_REQUIRED_EMPTY`
  - `GAB_FIELD_TYPE_INVALID`
  - `GAB_FIELD_LENGTH_INVALID`
  - `GAB_TYPE_UNSUPPORTED`
  - `GAB_SCHEMA_MISMATCH`

Fuera de alcance:
- Cambio de contrato público de `POST /api/gestor-documental/almacenamiento`.
- Reescritura del orquestador principal de persistencia.

## 4. Componentes actualizados
### 4.1 Models (`MiApp.Models`)
- `GabineteFieldMetadata`
  - nuevos campos: `DeclaredType`, `NormalizedType`, `MaxLength`, `IsUiEnabled`, `IsPhysicalColumnPresent`, `PhysicalDataType`, `PhysicalMaxLength`.
- `StorageMetadataOptions` (nuevo)
  - `CacheTtlMinutes`
  - `ValidatePhysicalSchema`
- `StoragePhysicalColumnMetadata` (nuevo)

### 4.2 Repository (`MiApp.Repository`)
- `IStorageGabineteMetadataRepository` / `StorageGabineteMetadataRepository`
  - lectura de metadata de `DETALLE_GABIENETE`.
  - detección de columna `IS_REQUIRED_BACKEND` vía `INFORMATION_SCHEMA`.
  - fallback de obligatoriedad con `ESTADO` cuando `IS_REQUIRED_BACKEND` no existe.
  - lectura opcional de columnas físicas desde `INFORMATION_SCHEMA.COLUMNS`.

Reglas de obligatoriedad backend:
- si existe `IS_REQUIRED_BACKEND`:
  - `1` obligatorio
  - `0` opcional
- fallback legacy:
  - `ESTADO = 0` obligatorio
  - `ESTADO = 1` opcional

### 4.3 Services (`MiApp.Services`)
- `StorageGabineteMetadataProvider`
  - reemplazo del placeholder.
  - normalización de tipos y longitud.
  - cache en memoria (`IMemoryCache`) por `alias+gabinete`.
  - validación opcional de columna física.
- `GabineteRequiredFieldsValidator`
  - validación de existencia por nombre de campo.
  - validación de obligatoriedad backend.
  - validación de compatibilidad de tipo.
  - validación de longitud para `VARCHAR/CHAR`.
  - reporte de mismatch físico cuando aplica.

Nota funcional:
- `CAMPO_ENABLE_DISABLE` se registra como estado UI (`IsUiEnabled`) pero no desactiva validación backend.

### 4.4 API (`DocuArchi.Api`)
- `Program.cs`
  - bind de `StorageMetadata`.
  - `AddMemoryCache()` para cache de metadata.
- `appsettings.json` y `appsettings.Development.json`
  - sección nueva:
```json
{
  "StorageMetadata": {
    "CacheTtlMinutes": 30,
    "ValidatePhysicalSchema": false
  }
}
```

## 5. Reglas de validación aplicadas
Por cada campo recibido:
1. Debe existir en metadata (`GAB_FIELD_UNKNOWN`).
2. Si tipo no soportado (`GAB_TYPE_UNSUPPORTED`).
3. Si valor no compatible con tipo (`GAB_FIELD_TYPE_INVALID`).
4. Si excede longitud declarada en `VARCHAR/CHAR` (`GAB_FIELD_LENGTH_INVALID`).
5. Si no existe físicamente y validación física está activa (`GAB_SCHEMA_MISMATCH`).

Para campos obligatorios:
- si `EvaluarCamposObligatorios = true` y valor vacío: `GAB_REQUIRED_EMPTY`.

## 6. Observabilidad
Eventos registrados:
- cache metadata: `cache hit` / `cache miss`.
- resolución de metadata por alias y gabinete.
- campo obligatorio faltante.
- error de consulta de metadata con `requestId`.

## 7. Archivos principales tocados
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/GabineteFieldMetadata.cs`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageMetadataOptions.cs`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StoragePhysicalColumnMetadata.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/GabineteMetadata/IStorageGabineteMetadataRepository.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Metadata/StorageGabineteMetadataProvider.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Validation/GabineteRequiredFieldsValidator.cs`
- `DocuArchi.Api/Program.cs`
- `DocuArchi.Api/appsettings.json`
- `DocuArchi.Api/appsettings.Development.json`

## 8. Contrato API relacionado
Documento de contrato HTTP actualizado:
- `SCRUM-171-Integracion-API-Frontend.md`
