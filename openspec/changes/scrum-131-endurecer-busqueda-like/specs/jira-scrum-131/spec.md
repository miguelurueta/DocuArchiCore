## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-131.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-131

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Harden workflow inbox LIKE search
Workflow inbox LIKE search MUST be built only from safe backend metadata and MUST normalize user search text before adding RawConditions.

#### Scenario: LIKE search uses only eligible metadata columns
- **WHEN** SearchType/TipoConsulta is normalized to `2`
- **THEN** the LIKE RawCondition uses only dynamic columns where `IsVisible = true`, `IsFilterable = true`, `DataType = "text"`, and the column passes `WorkflowInboxQueryPolicy.IsSelectableColumn(...)`
- **AND** columns supplied only by the client, hidden columns, non-filterable columns, and non-text columns are excluded

#### Scenario: LIKE search text is normalized and escaped
- **WHEN** the search text contains leading/trailing whitespace, single quotes, `%`, `_`, `[` or `]`
- **THEN** the builder trims the value, escapes SQL literal quotes, escapes LIKE wildcard characters, and builds an internal contains pattern
- **AND** the client cannot provide arbitrary LIKE wildcards

#### Scenario: LIKE search is omitted for unsafe or overly broad input
- **WHEN** search is null, empty, whitespace, shorter than `WorkflowInboxQueryPolicy.LikeSearchMinLength`, or there are no eligible columns
- **THEN** no LIKE RawCondition is generated
- **AND** the query remains valid

#### Scenario: LIKE search length is bounded
- **WHEN** search exceeds `WorkflowInboxQueryPolicy.LikeSearchMaxLength`
- **THEN** the term is truncated to the configured maximum before the LIKE pattern is built

#### Scenario: rows, count and export share the same LIKE condition
- **WHEN** `Build(...)`, `BuildCount(...)` and `BuildExport(...)` receive the same request/context/metadata
- **THEN** the same grouped LIKE RawCondition is applied to rows, count and export
- **AND** StructuredFilters remain combined as additional RawConditions using AND semantics

#### Scenario: non-LIKE search types are preserved
- **WHEN** SearchType/TipoConsulta is `1`, `3` or invalid
- **THEN** SearchType `1` and invalid fallback do not add LIKE RawConditions
- **AND** SearchType `3` preserves the existing advanced expression behavior
