## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-132.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-132

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox autocomplete endpoint
The API MUST expose `POST /api/workflowInboxgestion/inboxgestion/autocomplete` for authenticated workflow inbox autocomplete requests.

#### Scenario: Valid authenticated autocomplete request
- **WHEN** the request includes a valid `defaulalias` claim, integer `usuarioid` claim, `Search` with at least 3 trimmed characters, and optional `Limit`
- **THEN** the controller delegates to the workflow inbox service and returns `AppResponses<WorkflowInboxAutocompleteResponseDto>`

#### Scenario: Invalid claims
- **WHEN** `defaulalias` is missing or `usuarioid` is missing/non-integer
- **THEN** the endpoint rejects the request consistently with the existing workflow inbox endpoints

### Requirement: Autocomplete suggestion scope
Autocomplete suggestions MUST be generated only from backend-resolved workflow context and route metadata.

#### Scenario: Search term below minimum length
- **WHEN** `Search` is null, whitespace, or shorter than 3 trimmed characters
- **THEN** the service returns a successful empty `Items` list without executing repository queries

#### Scenario: Eligible dynamic columns
- **WHEN** route metadata is resolved
- **THEN** the service searches only dynamic columns that are selectable, visible, filterable, and text typed

#### Scenario: Limit enforcement
- **WHEN** `Limit` is absent, non-positive, or greater than the backend maximum
- **THEN** the service uses the backend default/max limit of 10 suggestions

### Requirement: Prefix-only safe query
The repository/query builder MUST query autocomplete suggestions without returning full workflow rows.

#### Scenario: Prefix LIKE search
- **WHEN** the query builder receives a valid search term
- **THEN** it builds per-column distinct value queries using escaped prefix `LIKE 'term%'` and never the global contains pattern `LIKE '%term%'`

#### Scenario: Suggestion response shape
- **WHEN** repository queries return duplicate values
- **THEN** the repository returns stable, capped, de-duplicated items with `Value`, `Label`, and logical `Field`
