# Design - SCRUM-143 CREAR-API-ORCHES-ATOMICO-GUARDAR-DOCUMENTO

## Context Reference
- `openspec/context/multi-repo-context.md`
- `openspec/context/OPSXJ_BACKEND_RULES.md`

## Problem Statement
The current implementation for saving documents and synchronizing images in the Tiptap editor is split into separate APIs. This lack of atomicity leads to data inconsistency if the document is saved but the image synchronization fails. We need a single orchestrated endpoint that guarantees consistency through a database transaction.

## Proposed Solution
Implement a new orchestrated API `full-save` that:
1. Receives the document HTML and the list of associated image UIDs.
2. Starts a database transaction.
3. Calls the existing document saving logic.
4. Calls the existing image synchronization logic using the same transaction.
5. Commits if both succeed, otherwise rolls back.

## Architectural Pattern
`Controller -> Service (Orchestrator) -> Repositories (Existing)`

### Affected Repositories
- `DocuArchi.Api`: New Controller
- `MiApp.Services`: New Service and Interface
- `MiApp.DTOs`: New DTOs
- `MiApp.Repository`: Update existing repositories to support transactions (if needed)
- `MiApp.Models`: Reference existing models

## Implementation Details

### 1. DTOs (MiApp.DTOs)
**File:** `MiApp.DTOs/DTOs/GestorDocumental/Editor/FullSaveEditorDocumentRequestDto.cs`
```csharp
public class FullSaveEditorDocumentRequestDto
{
    public long? DocumentId { get; set; }
    public long? TemplateId { get; set; }
    public int? TemplateVersion { get; set; }
    public string? DocumentTitle { get; set; }
    public string DocumentHtml { get; set; } = null!;
    public string? StatusCode { get; set; }
    public List<string> ImageUids { get; set; } = new();
}
```

### 2. Service (MiApp.Services)
**File:** `MiApp.Services/Service/GestorDocumental/Editor/ServiceFullSaveEditorDocument.cs`
**Interface:**
```csharp
public interface IServiceFullSaveEditorDocument
{
    Task<AppResponses<RaEditorDocument>> FullSaveAsync(FullSaveEditorDocumentRequestDto request, string defaultDbAlias);
}
```
**Logic:**
- Validate `defaultDbAlias` and `request.DocumentHtml`.
- Initialize `IDapperCrudEngine` transaction.
- Call `IGuardaEditorDocumentRepository.GuardaEditorDocumentAsync` (passing transaction).
- Call `ISincronizaEditorDocumentImagesRepository.SincronizaEditorDocumentImagesAsync` (passing transaction).
- Commit/Rollback.

### 3. Controller (DocuArchi.Api)
**File:** `DocuArchi.Api/Controllers/GestorDocumental/Editor/FullSaveEditorDocumentController.cs`
**Route:** `POST /api/gestor-documental/editor/document/full-save`
- Validate claim `defaulalias`.
- Call Service.
- Return `AppResponses`.

### 4. Repository Updates (MiApp.Repository)
Existing repositories `GuardaEditorDocumentRepository` and `SincronizaEditorDocumentImagesRepository` must be updated to accept an optional `IDbTransaction` in their methods.

## Testing Strategy
### Unit Tests (Service)
- Success creation/update.
- Failure in save -> Rollback.
- Failure in sync -> Rollback.
- Claim validation failure (Controller).

### Integration Tests
- Real transaction test with MySQL.
- Verify that if sync fails, the document is NOT saved.

## Constraints (OPSXJ_BACKEND_RULES)
- Register in `Program.cs`.
- Use `AppResponses`.
- Wrap in `try/catch`.
- Mock interfaces, not Dapper.
