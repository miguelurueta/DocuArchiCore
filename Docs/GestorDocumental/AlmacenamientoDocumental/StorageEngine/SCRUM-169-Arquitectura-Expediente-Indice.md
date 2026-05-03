# SCRUM-169 Arquitectura Expediente Indice

## Objetivo arquitectonico
Extender `StorageTransactionCoordinator` con la fase archivistica transaccional para:
- lock de `expediente_archivo`,
- lock de `unidad_conservacion`,
- calculo de indice electronico,
- actualizacion de folios/indice en expediente y unidad,
- insercion en `ra_cert_indice_expediente`.

## Principios aplicados
- SRP: repositorios separados por agregado (`Expediente`, `UnidadConservacion`, `IndiceElectronico`).
- Orquestacion centralizada en coordinator.
- Calculo y hash separados en `IIndiceElectronicoCalculator` e `IIndiceElectronicoBuilder`.
- Acceso a datos solo por `DapperCrudEngine + QueryOptions`.

## Flujo
1. `StorageTransactionCoordinator` abre transaccion.
2. Ejecuta flujo existente (identidad, gabinete, inventario).
3. Si hay `IdExpediente` + `IdUnidadConservacion`:
   - lock expediente (`FOR UPDATE`),
   - lock unidad (`FOR UPDATE`),
   - calcula orden/paginas,
   - actualiza expediente (orden, ultima pagina, folios),
   - actualiza unidad (folios),
   - inserta indice electronico.
4. Commit unico o rollback total.

## Riesgos
- Conflictos de concurrencia en updates optimistas.
- Deadlocks por orden inconsistente de locks.
- Inconsistencia si se salta alguna validacion de estado.

## Mitigaciones
- Orden fijo de locks: expediente -> unidad.
- Filtros optimistas en updates (`valor previo`).
- Errores transaccionales controlados con `StorageTransactionException`.
