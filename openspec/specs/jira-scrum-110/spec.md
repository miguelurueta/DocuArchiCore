# jira-scrum-110 Specification

## Purpose
TBD - created by archiving change scrum-110-crea-querybuilder-respository-bandeja-wo. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-110.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-110

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox query builder
The system MUST provide a repository-side query builder that transforms workflow inbox request metadata into `QueryOptions` without executing SQL.

#### Scenario: Build workflow inbox query options
- **GIVEN** a valid `WorkflowInboxDynamicTableRequestDto`
- **AND** a resolved list of `WorkflowDynamicColumnDefinitionDto`
- **WHEN** `IWorkflowInboxQueryBuilder.Build` is executed
- **THEN** it returns `QueryOptions`
- **AND** it sets the workflow inbox table alias, fixed joins, base filters, selected columns, ordering and paging

#### Scenario: Apply safe search modes
- **WHEN** the request uses type 1 search
- **THEN** no extra search raw conditions are added
- **WHEN** the request uses type 2 search
- **THEN** it only builds `LIKE` conditions for visible, filterable text columns
- **WHEN** the request uses type 3 search
- **THEN** it only applies the expression when it passes a whitelist-based sanitization

#### Scenario: Unsafe advanced expression is blocked
- **GIVEN** a type 3 search expression with non-whitelisted columns or dangerous SQL tokens
- **WHEN** the query builder evaluates it
- **THEN** it does not add the raw condition to `QueryOptions`

### Requirement: Workflow inbox request contract
The system MUST expose the request contract required by the workflow inbox query builder.

#### Scenario: Repository consumes workflow inbox request contract
- **WHEN** `IWorkflowInboxQueryBuilder.Build` is compiled and tested
- **THEN** `WorkflowInboxDynamicTableRequestDto` is available from the DTO layer with the fields required by the builder

