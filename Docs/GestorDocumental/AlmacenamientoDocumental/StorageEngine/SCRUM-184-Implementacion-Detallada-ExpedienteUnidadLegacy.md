# SCRUM-184 — Implementación Detallada Expediente/Unidad Legacy

## Alcance técnico
Implementación de fase transaccional legacy-compatible para expediente/unidad con builder + service + repositorios dedicados.

## Flujo detallado
1. `StorageTransactionCoordinator` llama servicio legacy dentro de la transacción activa.
2. `ExpedienteUnidadLegacyBuilder` valida:
   - opción activa,
   - presencia expediente/unidad,
   - no ambigüedad,
   - `IdClaseDocumento`,
   - `PhysicalMetadata.NumeroPaginas > 0`.
3. `ExpedienteUnidadLegacyService`:
   - consulta tipo de conservación por clase,
   - bloquea entidad (`FOR UPDATE`),
   - valida estado de expediente,
   - calcula nuevo total con protección de overflow,
   - actualiza campo legacy correcto,
   - arma `ExpedienteUnidadLegacyResult`.
4. Coordinator persiste resultado en `StorageContext.ExpedienteUnidadResult` y en `StorageTransactionResult.ExpedienteUnidadResult`.

## Contratos esperados
- `IExpedienteLegacyRepository.LockByIdAsync`.
- `IExpedienteLegacyRepository.UpdateFoliosElectronicosAsync`.
- `IExpedienteLegacyRepository.UpdateFoliosDigitalizadosAsync`.
- `IUnidadConservacionLegacyRepository.LockByIdAsync`.
- `IUnidadConservacionLegacyRepository.UpdateFoliosElectronicosAsync`.
- `IUnidadConservacionLegacyRepository.UpdateFoliosDigitalizadosAsync`.
- `IClaseDocumentoLegacyRepository.GetUnidadConservaTipoAsync`.

## SQL esperado
- Lock expediente:
```sql
SELECT ... FROM expediente_archivo WHERE ID_EXPEDIENTE=@IdExpediente FOR UPDATE;
```
- Lock unidad:
```sql
SELECT ... FROM unidad_conservacion WHERE ID_UNIDAD_CONSERVACION=@IdUnidadConservacion FOR UPDATE;
```
- Updates:
```sql
UPDATE expediente_archivo SET NUMERO_ELECTRONICO_CONTENIDO=@NuevoTotal WHERE ID_EXPEDIENTE=@IdExpediente;
UPDATE expediente_archivo SET NUMERO_DIGITALIZADO_CONTENIDO=@NuevoTotal WHERE ID_EXPEDIENTE=@IdExpediente;
UPDATE unidad_conservacion SET NUMERO_ELECTRONICO_CONTENIDO=@NuevoTotal WHERE ID_UNIDAD_CONSERVACION=@IdUnidadConservacion;
UPDATE unidad_conservacion SET NUMERO_DIGITALIZADO_CONTENIDO=@NuevoTotal WHERE ID_UNIDAD_CONSERVACION=@IdUnidadConservacion;
```

## Integración DI
```csharp
services.AddScoped<IExpedienteUnidadLegacyBuilder, ExpedienteUnidadLegacyBuilder>();
services.AddScoped<IExpedienteUnidadLegacyService, ExpedienteUnidadLegacyService>();
services.AddScoped<IExpedienteLegacyRepository, ExpedienteLegacyRepository>();
services.AddScoped<IUnidadConservacionLegacyRepository, UnidadConservacionLegacyRepository>();
services.AddScoped<IClaseDocumentoLegacyRepository, ClaseDocumentoLegacyRepository>();
```

## Dependencias con otros prompts
- Consumido por inventario (prioridad de `IdTipoUnidadDocumental`).
- Habilita condiciones para índice electrónico (prompt siguiente).
