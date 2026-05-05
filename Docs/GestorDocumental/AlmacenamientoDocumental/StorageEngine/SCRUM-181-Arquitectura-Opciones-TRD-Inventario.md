# SCRUM-181 - Arquitectura Opciones TRD/Inventario

## Objetivo
Consolidar la paridad funcional de reglas legacy para opciones de gabinete relacionadas con:

- aplicacion de inventario documental,
- validaciones TRD,
- validaciones de unidad/expediente previas a transaccion.

## Componentes
- `StorageOptionsValidator`
- `TrdRulesValidator`
- `ExpedienteUnidadRulesValidator`
- `IStorageOptionsResolver`

## Flujo
```mermaid
sequenceDiagram
    participant API as API/UseCase
    participant V as StorageValidationPipeline
    participant O as IStorageOptionsResolver
    participant VO as StorageOptionsValidator
    participant VT as TrdRulesValidator
    participant VE as ExpedienteUnidadRulesValidator

    API->>V: ValidateAsync(context)
    V->>VO: ValidateAsync
    VO->>O: ResolveAsync(gabinete, alias)
    O-->>VO: StorageOptionsModel
    VO-->>V: errores INV_* (si aplica)
    V->>VT: ValidateAsync
    VT-->>V: errores TRD_*
    V->>VE: ValidateAsync
    VE-->>V: errores EXP_*/UNI_*
    V-->>API: StorageValidationResult
```

## Reglas clave
- Si `AplicaInventarioDocumental = true`, `Inventario` es obligatorio.
- Si `AplicaTrd = true`, IDs TRD no pueden ser negativos.
- Si `AplicaUnidadConservacion = true`, `IdClaseDocumento` es obligatorio cuando hay expediente o unidad.

## Repos impactados
- `MiApp.Services` PR #122
- `MiApp.DTOs` PR #66
- `DocuArchiCore` PR #239
