# jira-scrum-133 Specification

## Purpose
TBD - created by archiving change scrum-133-pruebas-regresion-backend-auto-complete. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-133.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-133

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox search regression suite
The backend test suite MUST cover Workflow Inbox search semantics for legacy search, global LIKE search, advanced search, structured filters, and unsafe text handling without relying on production data.

#### Scenario: SearchType semantics are covered
- **WHEN** backend tests run for Workflow Inbox query building and service orchestration
- **THEN** they verify `SearchType = 1` legacy behavior, `SearchType = 2` global LIKE behavior, `SearchType = 3` advanced behavior, and unknown SearchType fallback behavior

#### Scenario: Safe searchable columns and special characters are covered
- **WHEN** global LIKE search or autocomplete tests run
- **THEN** they verify only visible, filterable, text columns participate and special characters `%`, `_`, brackets, and quotes remain safe

### Requirement: Workflow inbox count pagination and export regression suite
The backend test suite MUST cover consistency between rows, count, pagination, structured filters, and export modes.

#### Scenario: Rows count and export reuse filters
- **WHEN** tests build rows, count, and export queries for the same request
- **THEN** they verify shared filter conditions, stable pagination rules, `allMatching`, `currentPage`, and structured filters are preserved

#### Scenario: Service and repository contracts are covered
- **WHEN** service and repository tests execute
- **THEN** they verify `AppResponses` error handling, count/export request propagation, and controlled failures without requiring productive data

### Requirement: Workflow inbox autocomplete regression suite
The backend test suite MUST cover the autocomplete endpoint, service, repository, and query builder contracts introduced for Workflow Inbox.

#### Scenario: Invalid autocomplete search does not query
- **WHEN** autocomplete search is null, empty, whitespace, or shorter than the minimum
- **THEN** tests verify successful empty `Items` and no effective repository query

#### Scenario: Autocomplete authorization and response contract
- **WHEN** autocomplete controller and service tests execute
- **THEN** they verify required claims, invalid `usuarioid`, `defaulaliaswf`, capped limits, de-duplicated suggestions, non-empty `Value`, logical `Field`, and no internal request fields

#### Scenario: Autocomplete prefix search
- **WHEN** autocomplete query builder tests execute
- **THEN** they verify escaped prefix `LIKE 'valor%'` and do not allow global contains `LIKE '%valor%'` as the autocomplete default
