# SCRUM-164 Arquitectura UseCase/Orchestrator Storage Engine

## Objetivo
Definir la capa base de orquestación para almacenamiento documental, usando contratos de `SCRUM-163` y separando responsabilidades de Application Layer.

## Flujo
`AlmacenarDocumentoRequest -> AlmacenarDocumentoUseCase -> StorageContext -> DocumentStorageOrchestrator -> AlmacenarDocumentoResult -> AlmacenarDocumentoResponse`

## Componentes
- `IAlmacenarDocumentoUseCase` / `AlmacenarDocumentoUseCase`
- `IDocumentStorageOrchestrator` / `DocumentStorageOrchestrator`

## Reglas aplicadas
- Validación de entrada defensiva.
- Manejo de excepciones tipadas (`StorageValidationException`, `StorageTransactionException`, `StoragePhysicalException`).
- Sin acceso a DB/FS/XML en esta fase.
- Preparado para pipeline futuro: validación, transacción, escritura física, XML y compensación.

## Dependencias
- Contratos de `MiApp.DTOs` y `MiApp.Models` creados en `SCRUM-163`.
- Registro DI en `DocuArchi.Api/Program.cs`.
