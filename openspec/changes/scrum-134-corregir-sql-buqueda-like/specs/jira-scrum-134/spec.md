## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-134.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-134

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

## MODIFIED Requirements

### Requirement: Workflow Inbox LIKE escape must be multi-engine safe
Workflow Inbox MUST generate global LIKE search conditions without using `ESCAPE '\\'`, and MUST use a stable escape character compatible with MySQL, MariaDB, Oracle and SQL Server.

#### Scenario: SearchType 2 escapes LIKE wildcards with a standard escape clause
- **WHEN** `SearchType = 2` receives search text containing `%`, `_` or `!`
- **THEN** the generated LIKE pattern escapes `%` as `!%`, `_` as `!_` and `!` as `!!`
- **AND** the generated condition uses `ESCAPE '!'`
- **AND** the generated condition does not contain `ESCAPE '\\'`

#### Scenario: SearchType 2 preserves SQL literal safety
- **WHEN** `SearchType = 2` receives search text containing a single quote
- **THEN** the generated LIKE pattern escapes the quote as a SQL literal quote
- **AND** the generated condition keeps using `ESCAPE '!'`

#### Scenario: SearchType 2 skips empty search
- **WHEN** search text is `null`, empty, whitespace or shorter than the configured minimum
- **THEN** the builder does not generate a global LIKE condition
- **AND** it does not generate a pattern equivalent to `LIKE '%%'`

#### Scenario: Rows count and export remain consistent
- **WHEN** the builder creates row, count and export query options for the same `SearchType = 2` request
- **THEN** all three query options use the same LIKE condition
- **AND** all three use `ESCAPE '!'`

#### Scenario: Autocomplete uses the same LIKE escape policy
- **WHEN** autocomplete builds prefix LIKE conditions for search text containing `%`, `_` or `!`
- **THEN** the generated prefix LIKE pattern uses the same escape policy
- **AND** it uses `ESCAPE '!'`
- **AND** it does not contain `ESCAPE '\\'`

#### Scenario: Other search types are unchanged
- **WHEN** `SearchType = 1` is used
- **THEN** the builder does not add global LIKE conditions
- **WHEN** `SearchType = 3` is used
- **THEN** the existing advanced-search sanitization behavior remains unchanged

#### Scenario: Engine-specific bracket rules are not introduced
- **WHEN** search text contains `[` or `]`
- **THEN** the builder does not introduce a SQL Server-only bracket escaping rule
- **AND** the generated syntax remains a common LIKE pattern using `ESCAPE '!'`
