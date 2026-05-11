# SCRUM-196 - Pruebas Validación Metadata de Gabinete

## 1. Objetivo de pruebas
Verificar que la validación de metadata de gabinete en Storage Engine:
- use metadata real,
- aplique reglas de tipo/longitud/obligatoriedad,
- y mantenga compatibilidad con el `StorageValidationPipeline`.

## 2. Suite ejecutada
Proyecto:
- `tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj`

Filtro ejecutado:
- `StorageValidationPipelineTests`

Resultado:
- **9/9 pruebas exitosas**
- **0 fallos**
- **0 omitidas**

## 3. Escenarios cubiertos
### 3.1 Metadata y reglas básicas
- metadata no disponible -> `GAB_FIELDS_NOT_FOUND`
- campo desconocido en request -> `GAB_FIELD_UNKNOWN`
- campo obligatorio vacío -> `GAB_REQUIRED_EMPTY`

### 3.2 Validación de tipo
- tipo `INT` con valor no numérico -> `GAB_FIELD_TYPE_INVALID`
- tipo no soportado (`DECIMAL(10,2)` en prueba) -> `GAB_TYPE_UNSUPPORTED`

### 3.3 Validación de longitud
- `VARCHAR(20)` con valor > 20 -> `GAB_FIELD_LENGTH_INVALID`

### 3.4 Consistencia física
- campo marcado como no presente físicamente -> `GAB_SCHEMA_MISMATCH`

## 4. Validación de compilación
Comando:
- `dotnet build DocuArchiCore.sln --no-restore -v minimal`

Estado:
- compilación exitosa de solución
- sin errores de compilación en cambios de `SCRUM-196`

## 5. Riesgos pendientes para pruebas de integración
Pruebas aún recomendadas en ambiente con DB real:
- validación de fallback `IS_REQUIRED_BACKEND` vs `ESTADO`.
- comportamiento con `ValidatePhysicalSchema=true` contra esquemas reales.
- coherencia de collation/case-insensitive por ambiente MySQL.
