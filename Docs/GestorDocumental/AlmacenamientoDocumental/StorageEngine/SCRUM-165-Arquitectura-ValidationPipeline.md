# SCRUM-165 Arquitectura ValidationPipeline

## Objetivo
Extender StorageEngine con un ValidationPipeline desacoplado para cubrir reglas legacy de preindex, metadata y opciones funcionales sin abrir transacciones ni operar filesystem libre.

## Patron
- Pipeline / Chain of Responsibility por `IStorageValidator` ordenados.
- Orchestrator ejecuta `IStorageValidationPipeline` antes de fases transaccionales.
- Errores funcionales se acumulan en `StorageValidationResult`.

## Relacion con SCRUM-163 y SCRUM-164
- Reusa contratos/modelos del Prompt 2 (SCRUM-163).
- Se integra sobre UseCase + Orchestrator base del Prompt 3 (SCRUM-164).

## Componentes
- `IStorageValidationPipeline` / `StorageValidationPipeline`
- `IStorageValidator` / `BaseStorageValidator`
- Validadores: RequestStructure, Documento, Campos, TipoAlmacenamiento, ReglasBasicas, Preindex, GabineteRequiredFields, StorageOptions, TrdRules, ExpedienteUnidadRules
- Providers: `IStoragePreindexReader`, `IStorageGabineteMetadataProvider`, `IStorageOptionsResolver`

## Flujo
1. `AlmacenarDocumentoUseCase` mapea request a command/context.
2. `DocumentStorageOrchestrator` ejecuta pipeline.
3. Si hay errores -> `StorageValidationException`.
4. Si no hay errores -> resultado `Pending` para fases siguientes.

## SOLID
- SRP por validator.
- DIP por interfaces de lectura controlada.
- OCP agregando validadores nuevos por DI.

## Restricciones cumplidas
- Sin escritura DB.
- Sin transacciones.
- Sin XML.
- Sin rutas físicas directas desde request.
