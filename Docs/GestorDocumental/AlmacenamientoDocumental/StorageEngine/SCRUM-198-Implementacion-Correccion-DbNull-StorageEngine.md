# SCRUM-198 - Implementacion Correccion DbNull StorageEngine

## Resumen

Se corrige el manejo de nulos en el flujo transaccional de almacenamiento para evitar errores de Dapper al recibir `System.DBNull` como valor de parámetro.

Error objetivo corregido:

`InsertAsync error: The member SERIE_DOCUMENTO of type System.DBNull cannot be used as a parameter value`

## Cambios implementados

1. Inventario transaccional:
- Archivo: `MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Inventario/IInventarioDocumentalRepository.cs`
- Se reemplazó `DBNull.Value` por `null` en campos opcionales del insert (`SERIE_DOCUMENTO`, `ID_SERIE_DOCUMENTO`, `SUBSERIE_DOCUMENTO`, `ID_SUBSERIE_DOCUMENTO` y equivalentes).
- Se añadieron helpers internos para normalizar opcionales:
  - `NullableText(string?)`
  - `NullableValue<T>(T?)`

2. Repositorios Storage con helper `AddColumn`:
- Archivos:
  - `Gabinete/IGabineteStorageRepository.cs`
  - `IndiceElectronico/IIndiceElectronicoRepository.cs`
  - `Workflow/WorkflowStorageLogRepository.cs`
  - `Compensation/IStorageDbCompensationRepository.cs`
- Se cambió asignación `valor ?? DBNull.Value` por `valor ?? null`.

3. Trazabilidad de error transaccional:
- Archivo: `MiApp.Repository/Repositorio/DataAccess/DapperCrudEngine.cs`
- En `InsertBeginTrandAsync`, el mensaje de error ahora identifica contexto transaccional y tabla:
  - `InsertBeginTrandAsync error (table=...): ...`

## Decisión arquitectónica aplicada

- Capas de dominio/servicio/repositorio usan `null` semántico de C#.
- No se usa `DBNull.Value` en payloads de parámetros para Dapper en Storage Engine.
- Se mantiene separación de responsabilidades: no se agregaron reglas nuevas de negocio.

## Impacto funcional

- No cambia contratos HTTP.
- No cambia reglas de validación funcional.
- Corrige falla transaccional por mapeo de parámetros opcionales.
