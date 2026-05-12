## Context

- Jira issue key: `SCRUM-196`
- Jira summary: `IMPLEMENTACION-VALIDACION-METADA-DATA-GABINETE`
- Change: `scrum-196-implementacion-validacion-metada-data-ga`

Referencias:

- `openspec/context/multi-repo-context.md`
- `openspec/context/OPSXJ_BACKEND_RULES.md`
- evidencia SQL: `C:\cert\resultado-detalle.txt`

## Problem

El pipeline de almacenamiento documental depende de metadata por gabinete para validar campos
de indexacion. Hoy esa metadata no se carga desde base de datos:

- `StorageGabineteMetadataProvider` retorna lista vacia (placeholder)
- `GabineteRequiredFieldsValidator` corta flujo con `GAB_FIELDS_NOT_FOUND`
- no existe validacion estructural completa de tipo y longitud por metadata

Resultado: el endpoint de almacenamiento falla incluso con payload funcional para gabinetes
como `CONTABIL`.

## Goals

1. Resolver metadata real por gabinete desde `detalle_gabienete`.
2. Validar estructura de campos antes de persistencia final.
3. Mantener separacion estricta entre reglas backend y estado UI.
4. Reducir latencia con cache por `alias + gabinete`.
5. Mantener compatibilidad con `StorageValidationPipeline` y contratos actuales.

## Non-Goals

- Redisenar contrato publico de `POST /api/gestor-documental/almacenamiento`.
- Migrar toda la matriz legacy de gabinetes en esta iteracion.
- Introducir dependencia obligatoria a servicios externos de cache.

## Data Source and Mapping

Fuente principal:

- tabla legacy `detalle_gabienete`

Consulta base:

```sql
SELECT
    GABINETE,
    CAMPO,
    TIPO,
    ESTADO,
    CAMPO_ENABLE_DISABLE,
    INDETI_CAMPO_VALOR
FROM detalle_gabienete
WHERE UPPER(GABINETE) = UPPER(@gabinete)
ORDER BY IDENTI ASC;
```

Reglas de mapeo:

- `CAMPO` -> nombre de campo documental.
- `TIPO` -> tipo legacy a parsear.
- obligatoriedad backend:
  - priorizar `IS_REQUIRED_BACKEND` si la columna existe en ambiente.
  - fallback legacy: `ESTADO = 0` obligatorio, `ESTADO = 1` opcional.
- `CAMPO_ENABLE_DISABLE` no altera validacion backend (solo UI legacy).

## Validation Model

Validaciones backend requeridas:

1. Metadata de gabinete debe existir (`GAB_FIELDS_NOT_FOUND`).
2. Campo del request debe existir en metadata (`GAB_FIELD_UNKNOWN`).
3. Campo obligatorio backend no puede estar vacio (`GAB_REQUIRED_EMPTY`).
4. Tipo debe ser compatible (`GAB_FIELD_TYPE_INVALID` o `GAB_TYPE_UNSUPPORTED`).
5. Longitud maxima debe cumplirse (`GAB_FIELD_LENGTH_INVALID`).

Tipos legacy soportados inicialmente:

- `VARCHAR(n)`
- `CHAR(n)`
- `INT` / `INTEGER`
- `DATE`
- `DATETIME`
- `TEXT`
- `LONGTEXT`

## Physical Schema Consistency

Se habilita una validacion opcional para detectar drift metadata vs tabla fisica:

- fuente: `INFORMATION_SCHEMA.COLUMNS`
- objetivo: detectar columna no existente, tipo incompatible o longitud incompatible
- codigo de error: `GAB_SCHEMA_MISMATCH`

Esta validacion debe ser configurable por ambiente para no bloquear despliegues iniciales.

## Caching Strategy

Cache de metadata por llave:

- `alias + gabinete(normalizado)`

Reglas:

- TTL configurable (`StorageMetadata:CacheTtlMinutes`)
- invalidacion manual opcional
- logging de `cache_hit` / `cache_miss`

## Architecture and Responsibilities

`MiApp.Repository`

- consulta metadata `detalle_gabienete`
- consulta opcional `INFORMATION_SCHEMA.COLUMNS`

`MiApp.Services`

- provider de metadata con cache
- parser de tipo legacy
- validadores estructurales integrados al pipeline

`MiApp.Models`

- ampliar metadata de campo: nombre, tipo declarado, longitud, obligatoriedad, orden

`DocuArchi.Api`

- registrar DI de servicios/repositorios nuevos

## Validation Pipeline Integration

Mantener el flujo actual con validadores ordenados, agregando etapas sin romper contrato:

1. resolver metadata de gabinete
2. validar existencia/obligatoriedad
3. validar tipo y longitud
4. continuar flujo actual si no hay errores

Todos los errores siguen saliendo en `StorageValidationResult.Errors`.

## Error Catalog

- `GAB_FIELDS_NOT_FOUND`
- `GAB_FIELD_UNKNOWN`
- `GAB_REQUIRED_EMPTY`
- `GAB_FIELD_TYPE_INVALID`
- `GAB_FIELD_LENGTH_INVALID`
- `GAB_TYPE_UNSUPPORTED`
- `GAB_SCHEMA_MISMATCH`

## Observability

Logs minimos:

- `requestId`
- `alias`
- `gabinete`
- `campo`
- `tipoDeclarado`
- `resultadoValidacion`
- `cacheHit`
- `duracionMs`

No loguear valores documentales sensibles.

## Risks and Mitigations

- Drift entre metadata y esquema fisico.
  - mitigacion: validacion opcional de `INFORMATION_SCHEMA` + alertas.
- Tipos legacy no estandarizados (espacios/casing/formato).
  - mitigacion: parser tolerante y catalogo de errores explicito.
- Degradacion de performance por consultas repetidas.
  - mitigacion: cache con TTL e invalidacion.
