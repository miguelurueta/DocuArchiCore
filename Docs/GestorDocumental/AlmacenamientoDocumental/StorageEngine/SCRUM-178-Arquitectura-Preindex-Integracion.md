# SCRUM-178 — Arquitectura Preindex Integración

## Objetivo
Cerrar paridad legacy VB de preindex para `BatchPreindex` con tres responsabilidades separadas:
- Resolver archivo preindex real.
- Leer/parsear valores de preindex.
- Integrar valores en campos de indexación antes de validación/persistencia.

## Componentes
- `IStoragePreindexResolver` / `StoragePreindexResolver`
- `IStoragePreindexReader` / `StoragePreindexReader`
- `IStoragePreindexIntegrator` / `StoragePreindexIntegrator`
- `PreindexValidator` (coordinador de flujo)

## Principios
- SRP por componente.
- Validación temprana en pipeline.
- Integración sin sobrescribir valores manuales.
- Reutilización de `StorageContext` para propagar campos efectivos.

## Secuencia
```mermaid
sequenceDiagram
    participant O as Orchestrator
    participant V as PreindexValidator
    participant R as StoragePreindexResolver
    participant L as StoragePreindexReader
    participant I as StoragePreindexIntegrator
    participant G as GabineteRequiredFieldsValidator
    participant T as TransactionCoordinator

    O->>V: ValidateAsync(context)
    V->>R: Resolve(context)
    R-->>V: StoragePreindexFile
    alt no encontrado
        V-->>O: PREINDEX_NOT_FOUND
    else encontrado
        V->>L: ReadAsync(file)
        L-->>V: StoragePreindexResult(values)
        V->>I: Integrate(context, result)
        I-->>V: EffectiveCamposIndexacion
        V->>G: siguiente validator
        G-->>T: campos validados
    end
```

## Decisiones
- `StorageContext.EffectiveCamposIndexacion` actúa como fuente prioritaria para validación y persistencia.
- Resolver usa candidatos múltiples: `ArchivoTemporalId`, nombre base de documento y candidato legacy normalizado.
- Reader no resuelve rutas: solo lee un archivo ya resuelto.
