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

## Funciones revisadas y/o modificadas

1. `InventarioDocumentalRepository.InsertAsync(...)`
2. `InventarioDocumentalRepository.NullableText(...)`
3. `InventarioDocumentalRepository.NullableValue<T>(...)`
4. `GabineteStorageRepository.AddColumn(...)`
5. `IndiceElectronicoRepository.AddColumn(...)`
6. `WorkflowStorageLogRepository.AddColumn(...)`
7. `StorageDbCompensationRepository.AddColumn(...)`
8. `DapperCrudEngine.InsertBeginTrandAsync(...)`

## Revisión global de `DBNull` (ticket)

Se ejecutó búsqueda global en:

1. `MiApp.Repository` (`*.cs`)
2. `MiApp.Services` (`*.cs`)
3. `MiApp.Models` (`*.cs`)

Resultado:

1. `MiApp.Repository`: sin coincidencias activas de `DBNull` en Storage Engine tras la corrección.
2. `MiApp.Models`: sin coincidencias de `DBNull`.
3. `MiApp.Services`: existen 4 coincidencias, pero fuera del módulo Storage Engine:
   - `Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`
   - `Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs`

Decisión:

Esas coincidencias de `Workflow Inbox` se registran como hallazgo fuera de alcance de `SCRUM-198` (no tocado por este cambio).

## Decisión arquitectónica aplicada

- Capas de dominio/servicio/repositorio usan `null` semántico de C#.
- No se usa `DBNull.Value` en payloads de parámetros para Dapper en Storage Engine.
- Se mantiene separación de responsabilidades: no se agregaron reglas nuevas de negocio.

## Impacto funcional

- No cambia contratos HTTP.
- No cambia reglas de validación funcional.
- Corrige falla transaccional por mapeo de parámetros opcionales.
