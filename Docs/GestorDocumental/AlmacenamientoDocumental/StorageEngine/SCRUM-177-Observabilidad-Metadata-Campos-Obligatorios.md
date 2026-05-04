# SCRUM-177 Observabilidad Metadata Campos Obligatorios

## Logs agregados/ajustados

### StorageGabineteMetadataProvider
- Nivel: `Information`
- Mensaje:
  - `Metadata gabinete consultada alias={Alias} nombreGabinete={NombreGabinete} cantidadCampos={CantidadCampos}`
- Objetivo:
  - evidenciar consulta real (no placeholder),
  - medir cardinalidad de metadata por gabinete.

### GabineteRequiredFieldsValidator
- Nivel: `Warning` (en validaciones fallidas funcionales)
- Mensaje:
  - `Validacion de metadata/campos fallida requestId={RequestId} nombreGabinete={NombreGabinete}`
- Objetivo:
  - rastrear causa funcional sin exponer datos sensibles.

## Campos de trazabilidad relevantes
- `requestId`
- `nombreGabinete`
- `alias` (defaultDbAlias)
- `cantidadCampos`

## Datos que NO se registran
- valores de `CamposIndexacion`
- `FullText`
- rutas fisicas de archivos
- contenido de preindex

## Troubleshooting rapido

| Sintoma | Codigo | Diagnostico probable |
|---|---|---|
| No encuentra metadata | `GAB_FIELDS_NOT_FOUND` | gabinete sin configuracion en `DETALLE_GABIENETE` o filtro `VISIBLE` |
| Cantidad no coincide | `GAB_FIELDS_MISMATCH` | request incompleto o metadata de otro gabinete |
| Campo desconocido/desalineado | `GAB_FIELD_UNKNOWN` | orden o nombres de `CamposIndexacion` no coincide con metadata real |
| Campo obligatorio vacio | `GAB_REQUIRED_EMPTY` | request envio valor vacio para campo `SISTEMA=1` |

## Recomendacion operativa
- Correlacionar logs de ValidationPipeline y Orchestrator por `requestId`.
- Ante `GAB_FIELDS_*`, validar primero configuracion de `DETALLE_GABIENETE` antes de revisar fase transaccional.
