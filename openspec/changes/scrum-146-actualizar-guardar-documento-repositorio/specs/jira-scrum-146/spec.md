## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-146.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-146

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Repository stays pure (no orchestration)
`GuardaEditorDocumentRepository` MUST remain a pure persistence component for `ra_editor_documents` and MUST NOT orchestrate full-save flows or image synchronization.

#### Scenario: No orchestration logic
- **WHEN** the repository is reviewed
- **THEN** it only performs insert/update/select for `ra_editor_documents`
- **AND** it does not call other repositories or implement business workflows

### Requirement: Correct timestamps for insert/update
The repository MUST set timestamps consistently.

#### Scenario: Insert sets CreatedAt and UpdatedAt
- **WHEN** a document is inserted (`DocumentId <= 0`)
- **THEN** `CreatedAt = now` and `UpdatedAt = now`

#### Scenario: Update only sets UpdatedAt
- **WHEN** a document is updated (`DocumentId > 0`)
- **THEN** `CreatedAt` MUST NOT be modified
- **AND** `UpdatedAt = now`

### Requirement: Update validates existence
When updating (`DocumentId > 0`), the repository MUST fail with a controlled error if the document does not exist.

#### Scenario: Update missing document returns controlled error
- **GIVEN** `DocumentId > 0` does not exist in `ra_editor_documents`
- **WHEN** the repository executes update
- **THEN** it returns `success=false` with `data=null` (no unhandled exception)

### Requirement: Column naming consistency
SQL and model mapping MUST be consistent for `ra_editor_documents` columns (e.g., `DocumentId`, `TemplateId`, `CreatedAt`, `UpdatedAt`).

#### Scenario: Repository selects explicit columns
- **WHEN** the repository fetches the document after insert/update
- **THEN** it selects explicit columns mapped to the model properties (no ambiguous `SELECT *`)
