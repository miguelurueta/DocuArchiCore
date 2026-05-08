# SCRUM-193 - Arquitectura Compensación DB Post-Fallo Físico

## Contexto
El flujo actual confirma transacción DB en `StorageTransactionCoordinator` y luego ejecuta fase física (`StoragePhysicalPhaseExecutor`).  
Si FS/XML falla después del `commit`, quedan efectos persistidos sin representación física.

## Decisión Arquitectónica
Se introduce compensación lógica DB como fase de recuperación en `DocumentStorageOrchestrator` dentro de `catch (StoragePhysicalException)`.

## Componentes
- `DocumentStorageOrchestrator`
- `IStorageDbCompensationService` / `StorageDbCompensationService`
- `IStorageDbCompensationRepository` / `StorageDbCompensationRepository`
- Modelos: `StorageCompensationDbPlan`, `StorageCompensationDbResult`, `StorageCompensationDbStatus`

## Secuencia de Alto Nivel
1. `ExecuteAsync(context)` completa validación + transacción.
2. Fase física falla con `StoragePhysicalException`.
3. Se construye plan de compensación (`BuildDbCompensationPlan`).
4. Se ejecuta compensación DB.
5. Se registra resultado `OK|PARTIAL|FAILED`.
6. Se relanza excepción física original.

## Invariantes
- No revertir reserva de identidad en `system1`.
- No ocultar la excepción física original.
- Mantener compensación idempotente y auditable.

## Riesgos Controlados
- `PARTIAL`: cuando una o más acciones no se completan.
- Esquemas heterogéneos: auditoría best-effort en `ra_log_sotorage_compensacion`.
- Concurrencia: pasos críticos con control transaccional en repositorio.

