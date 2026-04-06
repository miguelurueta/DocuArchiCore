## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-129.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-129

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: AppTable export accepts current page for executive formats
`POST /api/AppTable/export` MUST accept `ExportMode = "currentPage"` in addition to `ExportMode = "allMatching"`.

#### Scenario: Export mode is current page
- **WHEN** the client requests `ExportMode = "currentPage"` with a valid format
- **THEN** the backend accepts the request without creating a new endpoint

#### Scenario: Export mode is invalid
- **WHEN** the client sends an unsupported `ExportMode`
- **THEN** the backend returns a controlled validation error for `exportMode`

### Requirement: Current page export preserves the visible slice
The backend MUST export exactly the current visible page when `ExportMode = "currentPage"`.

#### Scenario: Current page xlsx export uses visible pagination
- **WHEN** the client requests `Format = "xlsx"` and `ExportMode = "currentPage"` with `Page` and `PageSize`
- **THEN** the export query respects that pagination slice and generates the file from those rows only

#### Scenario: Current page pdf export uses visible pagination
- **WHEN** the client requests `Format = "pdf"` and `ExportMode = "currentPage"` with `Page` and `PageSize`
- **THEN** the export query respects that pagination slice and generates the file from those rows only

### Requirement: All matching export remains unchanged
The backend MUST preserve the existing semantics of `ExportMode = "allMatching"`.

#### Scenario: All matching ignores visible page
- **WHEN** the client requests `ExportMode = "allMatching"`
- **THEN** the export query ignores the visible page slice and keeps exporting the filtered universe

#### Scenario: All matching still enforces export limit
- **WHEN** the filtered total exceeds the configured export cap
- **THEN** the backend returns the existing controlled limit error and does not generate the file
