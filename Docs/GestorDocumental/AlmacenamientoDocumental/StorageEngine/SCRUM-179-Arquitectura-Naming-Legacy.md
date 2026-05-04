# SCRUM-179 — Arquitectura Naming Legacy

## Objetivo
Restaurar la convención de nombres legacy en almacenamiento documental:

- Binario principal: `DIG########.{ext}`
- XML de metadata física: `FXL########.xml`
- Extensión resuelta por tipo documental desde `DA_EXTENSION`.

## Diseño por capas

### Capa Repository
- `IStorageExtensionRepository`
- Responsabilidad: consultar extensión por `IdTipoDocumento`.
- Fuente: tabla `DA_EXTENSION`.

### Capa Services
- `IStorageExtensionResolver`
  - Normaliza extensión (`.pdf`, `.tif`, etc.).
  - Lanza error funcional cuando no hay mapeo.
- `IStorageNamingService`
  - Construye nombres determinísticos legacy.
  - Aplica padding de 8 dígitos.

### Capa Orquestación física
- `StoragePlanBuilder`:
  - Obtiene tipo documental.
  - Resuelve extensión desde DB.
  - Genera `NombreArchivoPrincipal`, `NombreXml`, `SegundoNombreDocumental`.
- `StorageXmlWriter`:
  - Persiste XML con nombre `FXL########.xml`.

## Contratos actualizados
- `IStoragePlanBuilder` migra a `BuildFilePlanAsync(...)`.
- `StorageFilePlanModel` agrega:
  - `NombreXml`
  - `SegundoNombreDocumental`

## Inyección de dependencias
Registrado en `DocuArchi.Api/Program.cs`:
- `IStorageExtensionRepository -> StorageExtensionRepository`
- `IStorageExtensionResolver -> StorageExtensionResolver`
- `IStorageNamingService -> StorageNamingService`

## Decisiones clave
- Se elimina naming `alm_{id}` por incompatibilidad legacy.
- Se evita extensión fija o inferida desde frontend.
- Naming queda centralizado en un servicio reusable y testeable.

