## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-124.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-124

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: workflowInboxgestion MUST publish filtered total in Pagination.Total
The system MUST expose the real filtered total for `workflowInboxgestion` through `DynamicUiTableDto.Pagination.Total`.

#### Scenario: Query with filters and pagination
- **WHEN** the inbox query applies workflow base filters, search, structured filters and pagination
- **THEN** `Pagination.Total` represents the total number of filtered records before pagination
- **AND** it does not use the number of rows returned in the current page

### Requirement: Count query MUST reuse the same workflow query rules
The system MUST build the count query from the same workflow query rules used for the data query.

#### Scenario: Count query generation
- **WHEN** backend prepares the count query for `workflowInboxgestion`
- **THEN** it reuses the same joins, workflow base filters, `Search`, `SearchType` and `StructuredFilters`
- **AND** it omits `ORDER BY`
- **AND** it omits `LIMIT` and `OFFSET`
- **AND** it selects only a `COUNT(1)`-style expression

### Requirement: Out-of-range page MUST keep total metadata
The system MUST keep the correct filtered total even when the requested page has no rows.

#### Scenario: Page greater than total pages
- **WHEN** the requested page is out of range after filters are applied
- **THEN** the response is successful
- **AND** `Rows` is an empty array
- **AND** `Pagination.Total` preserves the filtered total
- **AND** the full `DynamicUiTableDto` structure remains intact
