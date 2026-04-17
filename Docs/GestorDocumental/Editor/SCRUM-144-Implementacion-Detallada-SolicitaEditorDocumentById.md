# SCRUM-144 — Implementación detallada: `SolicitaEditorDocumentById`

## Archivos creados

- `DocuArchi.Api/Controllers/GestorDocumental/Editor/SolicitaEditorDocumentByIdController.cs`
- `MiApp.Services/Service/GestorDocumental/Editor/ServiceSolicitaEditorDocumentById.cs`
- `MiApp.Repository/Repositorio/GestorDocumental/Editor/SolicitaEditorDocumentByIdRepository.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Editor/EditorDocumentDetailResponseDto.cs`
- `MiApp.DTOs/DTOs/GestorDocumental/Editor/EditorDocumentImageResponseDto.cs`

## DI

Registrar:

- `ISolicitaEditorDocumentByIdRepository` → `SolicitaEditorDocumentByIdRepository`
- `IServiceSolicitaEditorDocumentById` → `ServiceSolicitaEditorDocumentById`

## Errores

- Validaciones (`documentId`, `defaultDbAlias`) retornan `AppResponses<T>` con error tipo `Validation`.
- Documento no encontrado retorna `AppResponses<T>` con error tipo `NotFound`.

## Verificación local

- Se intentó `dotnet build` del proyecto `DocuArchi.Api` en este entorno; el comando está devolviendo código de salida `1` sin errores MSBuild/NuGet explícitos en la salida (posible condición del entorno/shell). La verificación funcional debe repetirse en tu estación/CI.
