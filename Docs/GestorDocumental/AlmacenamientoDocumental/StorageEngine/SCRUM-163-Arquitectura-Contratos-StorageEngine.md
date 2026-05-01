# SCRUM-163 Arquitectura de Contratos Storage Engine

## Objetivo
Definir contratos base y tipado fuerte para el flujo de almacenamiento documental sin implementar logica de negocio.

## Flujo
`AlmacenarDocumentoRequest -> AlmacenarDocumentoCommand -> StorageContext -> AlmacenarDocumentoResult -> AlmacenarDocumentoResponse`

## Estados
- `StorageDocumentState`: `Pending`, `Reserved`, `Completed`, `PhysicalFailed`, `Failed`.
- `StoragePhase`: fases de procesamiento desde `RequestReceived` hasta `Completed`/`Failed`.

## Idempotencia
- `RequestId` se mantiene en request, command, context, result y response.
- `StorageIdempotencyModel` y `StoragePhysicalStatusModel` preparan control de doble ejecucion y estado fisico.

## Relacion con siguientes prompts
- SCRUM-164+: implementacion de orquestador/use case y pipeline de validacion/transaccion.
