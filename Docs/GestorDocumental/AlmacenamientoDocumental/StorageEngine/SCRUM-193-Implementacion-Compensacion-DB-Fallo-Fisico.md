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

## Flujo Aplicado
1. `DocumentStorageOrchestrator` ejecuta validación, metadata, transacción y fase física.
2. Si ocurre `StoragePhysicalException` post-commit:
- Construye `StorageCompensationDbPlan`.
- Ejecuta `IStorageDbCompensationService.ExecuteAsync(plan)`.
- Registra resultado `OK | PARTIAL | FAILED`.
- Relanza la excepción física original (no se oculta error raíz).

## Acciones de Compensación DB
- Reversión de cuota en `disco_detalle` con control de concurrencia.
- Reversión de folios en `expediente_archivo`/`unidad_conservacion` cuando aplica.
- Anulación lógica de `registro_producion_documental`.
- Eliminación de log `logdocuarchi` cuando fue insertado.
- Eliminación del registro en gabinete dinámico por `ID`.
- Intento de auditoría en `ra_log_sotorage_compensacion` (best-effort).

## Idempotencia y Seguridad
- Guardas por estado (`NUMPAG_CARP`) para no repetir reversión de disco cuando ya fue compensado.
- Operaciones tolerantes a no-op (registro ya eliminado/anulado).
- Validación defensiva de plan antes de ejecutar compensación.
