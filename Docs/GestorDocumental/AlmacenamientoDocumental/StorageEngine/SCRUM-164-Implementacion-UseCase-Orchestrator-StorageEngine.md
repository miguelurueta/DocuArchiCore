# SCRUM-164 Implementación UseCase/Orchestrator Storage Engine

## Archivos creados en `MiApp.Services`
- `Service/GestorDocumental/AlmacenamientoDocumental/IAlmacenarDocumentoUseCase.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/AlmacenarDocumentoUseCase.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/IDocumentStorageOrchestrator.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/DocumentStorageOrchestrator.cs`

## Cambios en `DocuArchi.Api`
- Registro DI:
  - `IAlmacenarDocumentoUseCase -> AlmacenarDocumentoUseCase`
  - `IDocumentStorageOrchestrator -> DocumentStorageOrchestrator`
- Ajuste incluido en `TramiteController` por solicitud del flujo actual de trabajo.

## Contratos usados
- `AlmacenarDocumentoRequest`
- `AlmacenarDocumentoResponse`
- `StorageContext`
- `AlmacenarDocumentoResult`
- `StorageDocumentState`

## Decisiones técnicas
- `requestId` preservado/normalizado en el caso de uso.
- `StorageContext` armado con datos mínimos para orquestación.
- Orchestrator retorna estado `Pending` como base para fases siguientes.
