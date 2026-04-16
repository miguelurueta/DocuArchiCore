## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-141.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-141

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Save editor image
The system MUST expose an API to upload images for the Tiptap editor and persist them in `ra_editor_document_images`.

#### Scenario: API contract
- **WHEN** the API is implemented
- **THEN** it exposes `POST /api/gestor-documental/editor/guardar-imagen`
- **AND** it accepts `multipart/form-data` with form field `file`
- **AND** it returns `AppResponses<GuardaEditorImageResponseDto?>`

#### Scenario: Claim required
- **WHEN** the API is called
- **THEN** it validates claim `defaulalias` via `IClaimValidationService`
- **AND** returns `BadRequest(validation.Response)` when missing/invalid

#### Scenario: File validation
- **WHEN** `file` is missing or empty
- **THEN** it returns `success = false` with validation error

#### Scenario: Content type validation
- **WHEN** `contentType` is not in the allowed set (`image/png`, `image/jpeg`, `image/jpg`, `image/webp`)
- **THEN** it returns `success = false` with validation error and does not persist data

#### Scenario: Persistence
- **WHEN** the file is valid
- **THEN** it persists an image row with `storage_type_code = "db"` and `image_bytes` populated
- **AND** it returns `success = true` with image metadata (`imageId`, `imageUid`, `publicUrl`)
