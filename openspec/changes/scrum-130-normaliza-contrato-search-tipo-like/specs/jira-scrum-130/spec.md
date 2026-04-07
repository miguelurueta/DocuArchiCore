## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-130.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-130

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox SearchType contract normalization
The Workflow Inbox backend MUST normalize the `SearchType` contract for `POST /api/workflowInboxgestion/inboxgestion` without changing the endpoint or public DTO shape.

#### Scenario: Legacy search remains unchanged
- **WHEN** a request uses `SearchType = 1` with any `Search` value
- **THEN** the backend MUST keep the legacy behavior and not add the global LIKE condition

#### Scenario: Global LIKE search is explicit
- **WHEN** a request uses `SearchType = 2` with a valid `Search` value
- **THEN** the backend MUST add one grouped global LIKE raw condition over eligible dynamic columns

#### Scenario: Advanced search remains unchanged
- **WHEN** a request uses `SearchType = 3`
- **THEN** the backend MUST keep the existing advanced-search sanitization and whitelist behavior

#### Scenario: Invalid search type falls back safely
- **WHEN** a request uses a `SearchType` different from `1`, `2`, or `3`
- **THEN** the backend MUST normalize it to legacy behavior equivalent to `SearchType = 1`

### Requirement: Global LIKE search safety and eligibility
The global LIKE search for Workflow Inbox MUST be limited to safe dynamic metadata and sanitized user input.

#### Scenario: Eligible columns only
- **WHEN** global LIKE search is applied
- **THEN** only columns from `List<WorkflowDynamicColumnDefinitionDto>` with `IsVisible = true`, `IsFilterable = true`, and text `DataType` according to `WorkflowInboxQueryPolicy.IsTextDataType(...)` MAY participate

#### Scenario: LIKE term is normalized
- **WHEN** global LIKE search receives `Search`
- **THEN** the backend MUST trim it, reject empty/whitespace values, reject values shorter than the configured minimum, apply a configured maximum length, escape single quotes, and escape LIKE wildcard characters `%` and `_`

#### Scenario: No arbitrary client wildcard
- **WHEN** the client sends `%` or `_` inside `Search`
- **THEN** those characters MUST be treated as literal input rather than SQL LIKE wildcards

#### Scenario: No eligible columns
- **WHEN** there are no eligible dynamic columns
- **THEN** the backend MUST omit the global LIKE block without generating invalid SQL

### Requirement: Query consistency across rows, count and export
The rows, count and export query builders MUST reuse the same normalized search behavior.

#### Scenario: Count uses same LIKE filter
- **WHEN** `BuildCount` is called for a request with `SearchType = 2`
- **THEN** it MUST include the same global LIKE raw condition shape used by the rows query

#### Scenario: Export uses same LIKE filter
- **WHEN** `BuildExport` is called for a request with `SearchType = 2`
- **THEN** it MUST include the same global LIKE raw condition shape used by the rows query while preserving export pagination mode behavior

### Requirement: Tests for SearchType and LIKE behavior
The backend test suite MUST cover SearchType normalization, LIKE eligibility, input edge cases and query consistency.

#### Scenario: QueryBuilder test coverage
- **WHEN** `WorkflowInboxQueryBuilderTests` run
- **THEN** they MUST cover `SearchType` 1, 2, 3, invalid values, empty/short/long search values, escaped quotes, escaped `%` and `_`, eligible/ineligible columns, structured filter coexistence, rows/count/export consistency

#### Scenario: Service test coverage
- **WHEN** `WorkflowInboxServiceTests` run
- **THEN** they MUST verify that the service propagates valid `SearchType = 2` and normalizes invalid values to the legacy `SearchType = 1` behavior when building the internal request
