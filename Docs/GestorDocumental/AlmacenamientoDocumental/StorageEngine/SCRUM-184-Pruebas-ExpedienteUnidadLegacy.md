# SCRUM-184 — Pruebas Expediente/Unidad Legacy

## Unitarias Builder
- Opción apagada => no ejecuta.
- `Expediente` null con opción activa => error.
- Sin expediente/unidad => error.
- Expediente + unidad simultáneo => error.
- Sin `IdClaseDocumento` con expediente/unidad => error.
- Sin `PhysicalMetadata` o folios inválidos => error.
- Solo expediente => plan con `IdTipoUnidadDocumental=2`.
- Solo unidad => plan con `IdTipoUnidadDocumental=1`.

## Unitarias Service
- Expediente inexistente => error.
- Expediente cerrado => error.
- Expediente válido => update `NUMERO_ELECTRONICO_CONTENIDO`.
- Unidad inexistente => error.
- Unidad tipo digitalizado => update `NUMERO_DIGITALIZADO_CONTENIDO`.
- Unidad tipo electrónico => update `NUMERO_ELECTRONICO_CONTENIDO`.
- Tipo unidad indeterminado => error.
- Overflow en acumulación => error.

## Integración
- `FOR UPDATE` efectivo sobre expediente.
- `FOR UPDATE` efectivo sobre unidad.
- Commit persiste folios.
- Rollback revierte folios.
- Resultado llega a `StorageContext` y `StorageTransactionResult`.

## Evidencia requerida
- Salidas de `dotnet test` por proyecto impactado.
- Casos de regresión VB->C# documentados con expected/actual.
- Validación de no actualización de `NUMERO_FOLIO_UNIDAD_CONSERVACION`.
