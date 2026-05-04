# SCRUM-179 — Cobertura Legacy Naming

## Mapeo VB -> .NET

### 1) Ceros_Imagen_Almacenada(...)
- Legacy VB:
  - Genera ceros para completar identificador.
- Nuevo:
  - `StorageNamingService.Generate(...)` usa `idAlmacen.ToString("D8")`.
- Paridad: `cumple`.

### 2) RetornaExtensionTipoDocumento(...)
- Legacy VB:
  - Consulta `DA_EXTENSION` por `ESTADO_NORMAL / ESTADO_ADJUNTO / ESTADO_LINK`.
- Nuevo:
  - `StorageExtensionRepository.GetExtensionAsync(...)` con filtros OR equivalentes.
  - `StorageExtensionResolver.ResolveAsync(...)` normaliza y valida.
- Paridad: `cumple`.

### 3) Generando_Archivo_Dat_Xml(...)
- Legacy VB:
  - XML físico con patrón `FXL + ceros + id + .xml`.
- Nuevo:
  - `StorageXmlWriter` consume `plan.NombreXml` generado por naming service.
- Paridad: `cumple`.

### 4) Nombre archivo principal
- Legacy VB:
  - `DIG + ceros + id + extensión`.
- Nuevo:
  - `StoragePlanBuilder` asigna `NombreArchivoPrincipal` desde `StorageNamingService`.
- Paridad: `cumple`.

## Diferencias controladas
- El builder de índice electrónico aún no usa explícitamente `SegundoNombreDocumental`.
- Impacto: bajo para nombre físico; pendiente para trazabilidad total de `segundo_nombre`.

