# SCRUM-193 - Implementación Compensación DB Post-Fallo Físico

## Objetivo
Agregar compensación de base de datos cuando la fase física (`FS/XML`) falla después de `commit` transaccional.

## Componentes Implementados
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageCompensationDbPlan.cs`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StorageCompensationDbResult.cs`
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/Enums/StorageCompensationDbStatus.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/Compensation/IStorageDbCompensationRepository.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Compensation/IStorageDbCompensationService.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/DocumentStorageOrchestrator.cs`
- `DocuArchi.Api/Program.cs` (registro DI de servicio y repositorio)

## Flujo Aplicado
1. `DocumentStorageOrchestrator` ejecuta validación, metadata, transacción y fase física.
2. Si ocurre `StoragePhysicalException` post-commit:
   - Construye `StorageCompensationDbPlan`.
   - Ejecuta `IStorageDbCompensationService.ExecuteAsync(plan)`.
   - Registra resultado `OK | PARTIAL | FAILED`.
   - Relanza la excepción física original (no se oculta error raíz).

## Registro de Dependencias (DI)
- `IStorageDbCompensationService -> StorageDbCompensationService`
- `IStorageDbCompensationRepository -> StorageDbCompensationRepository`

Sin este registro, el `HostBuilder` falla al resolver `DocumentStorageOrchestrator`.

## Acciones de Compensación DB
- Reversión de cuota en `disco_detalle` con control de concurrencia.
- Reversión de folios en `expediente_archivo`/`unidad_conservacion` cuando aplica.
- Marcación de error de almacenamiento en `registro_producion_documental`.
- Eliminación de log `logdocuarchi` cuando fue insertado.
- Eliminación del registro en gabinete dinámico por `ID`.
- Intento de auditoría en `ra_log_sotorage_compensacion` (best-effort).

## Cambio de Marcador de Inventario
- Campo anterior en compensación: `ESTADO_DOCUMENTO_ARCHIVO`.
- Campo vigente para compensación por error físico: `ESTADO_ELIMINA_ERROR_ALMACENAMIENTO = 1`.
- DDL de soporte:
  - `ALTER TABLE registro_producion_documental ADD ESTADO_ELIMINA_ERROR_ALMACENAMIENTO int(10) unsigned NOT NULL DEFAULT '0';`

## Idempotencia y Seguridad
- Guardas por estado (`NUMPAG_CARP`) para no repetir reversión de disco cuando ya fue compensado.
- Operaciones tolerantes a no-op (registro ya eliminado/anulado).
- Validación defensiva de plan antes de ejecutar compensación.
- Se preserva invariante legacy: no revertir `system1.proxid`, `system1.numcarp`, `system1.NUMPAG_CARP`.

## Resultado Esperado
- Si la fase física falla post-commit, el sistema deja trazabilidad de error físico y ejecuta reversión lógica DB con resultado auditable.
- Si la compensación falla parcialmente, el estado queda explícito (`PARTIAL`) para reconciliación operativa.
