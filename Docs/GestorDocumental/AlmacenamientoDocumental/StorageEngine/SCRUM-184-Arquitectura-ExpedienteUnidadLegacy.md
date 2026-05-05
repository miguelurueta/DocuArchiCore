# SCRUM-184 — Arquitectura Expediente/Unidad Legacy

## Objetivo
Recuperar paridad VB para expediente/unidad en almacenamiento documental, separando casos expediente vs unidad y actualizando folios en campos correctos.

## Componentes objetivo
- `IExpedienteUnidadLegacyBuilder` / `ExpedienteUnidadLegacyBuilder`
- `IExpedienteUnidadLegacyService` / `ExpedienteUnidadLegacyService`
- `IExpedienteLegacyRepository`
- `IUnidadConservacionLegacyRepository`
- `IClaseDocumentoLegacyRepository`
- Integración en `StorageTransactionCoordinator`

## Decisiones arquitectónicas
- Resolver plan de ejecución desde `StorageContext` + `ResolvedOptions` + `PhysicalMetadata`.
- Ejecutar en transacción existente (sin abrir conexión nueva).
- Usar `SELECT ... FOR UPDATE` para lock de expediente/unidad.
- Bloquear ambigüedad (`IdExpediente` y `IdUnidadConservacion` simultáneos).
- Exigir `IdClaseDocumento` cuando exista expediente o unidad.
- Priorizar `IdTipoUnidadDocumental` resuelto por fase legacy para inventario.

## Secuencia de alto nivel
1. `StorageTransactionCoordinator` invoca `IExpedienteUnidadLegacyService.ExecuteAsync`.
2. Builder construye `ExpedienteUnidadLegacyPlan`.
3. Service bloquea registro objetivo (`expediente_archivo` o `unidad_conservacion`).
4. Service consulta tipo de conservación por clase documental.
5. Service actualiza contador de folios correcto.
6. Service retorna `ExpedienteUnidadLegacyResult` al `StorageContext` y al resultado transaccional.

## Reglas de negocio clave
- `AplicaUnidadConservacion = false` => no ejecutar fase.
- Solo expediente => `IdTipoUnidadDocumental = 2`.
- Solo unidad => `IdTipoUnidadDocumental = 1`.
- Expediente cerrado/inactivo => error bloqueante.
- Unidad tipo `DIGITALIZADO` => incrementar `NUMERO_DIGITALIZADO_CONTENIDO`.
- Unidad tipo `ELECTRONICO` => incrementar `NUMERO_ELECTRONICO_CONTENIDO`.
- No usar `NUMERO_FOLIO_UNIDAD_CONSERVACION`.

## Riesgos controlados
- Carrera concurrente en folios: mitigado con `FOR UPDATE`.
- Desalineación legacy: mitigada con matriz de regresión VB vs C#.
- Doble asignación expediente/unidad: mitigada con validación de ambigüedad.
