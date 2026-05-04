# SCRUM-171 Arquitectura Final StorageEngine

## Objetivo
Exponer el endpoint final de almacenamiento documental como puerta de entrada del Storage Engine ya construido en capas previas (validación, transacción, fase física y compensación).

## Contexto de arquitectura
`API Controller -> UseCase -> Orchestrator -> ValidationPipeline -> TransactionCoordinator -> PhysicalPhaseExecutor -> Response`

## Componentes clave
- `AlmacenamientoDocumentalController` (DocuArchi.Api): validación claims + feature flag + traducción HTTP.
- `IAlmacenarDocumentoUseCase` (MiApp.Services): crea contexto y ejecuta orquestación.
- `DocumentStorageOrchestrator` (MiApp.Services): coordina validación, transacción DB y fase física.

## Estados del flujo
- `Pending`: contexto inicial.
- `Reserved`: transacción DB completada.
- `Completed`: DB + FS + XML exitosos.
- `PhysicalFailed`: fallo post-commit y compensación ejecutada.
- `Failed`: error previo o no recuperable.

## Consistencia y límites
- La fase DB se confirma antes de FS/XML.
- FS/XML fallidos se reportan como `PhysicalFailed` y ejecutan compensación.
- La idempotencia persistente queda como deuda técnica documentada.

