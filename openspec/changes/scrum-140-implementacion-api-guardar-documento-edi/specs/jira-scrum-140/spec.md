## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-140.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-140

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Save editor HTML document
The system MUST allow creating and updating an HTML editor document in `ra_editor_documents` via API using the Controller → Service → Repository pattern.

#### Scenario: Claim required
- **WHEN** the API is called
- **THEN** it validates claim `defaulalias` via `IClaimValidationService`
- **AND** returns `BadRequest(validation.Response)` when missing/invalid

#### Scenario: HTML required
- **WHEN** request `documentHtml` is null/empty/whitespace
- **THEN** returns `success = false` with validation error and does not persist data

#### Scenario: Create
- **WHEN** `documentId` is null/<=0
- **THEN** it inserts a new row with `FormatCode = "html"` and `StatusCode = "saved"` by default
- **AND** it returns `success = true` with the created document

#### Scenario: Update
- **WHEN** `documentId > 0`
- **THEN** it validates the document exists and updates the row
- **AND** it returns `success = true` with the updated document
