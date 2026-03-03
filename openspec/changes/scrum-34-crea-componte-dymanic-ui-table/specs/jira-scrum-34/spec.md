## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-34.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-34

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Generic dynamic UI table query endpoint
The system MUST provide a generic endpoint to query dynamic table data and configuration by `TableId`.

#### Scenario: Query with configuration included
- **WHEN** frontend calls `POST /api/ui/dynamic-table/query` with `IncludeConfig=true`
- **THEN** backend returns `AppResponses<DynamicUiTableDto>` with columns, rows, actions, pagination and sorting metadata

#### Scenario: Query rows-only
- **WHEN** frontend calls `POST /api/ui/dynamic-table/query` with `IncludeConfig=false`
- **THEN** backend returns rows and pagination without full configuration payload

### Requirement: Generic dynamic UI table action endpoint
The system MUST expose an action execution endpoint with backend authorization revalidation.

#### Scenario: Execute row action
- **WHEN** frontend calls `POST /api/ui/dynamic-table/action` with `ActionId` and `RowId`
- **THEN** backend validates required claims and action payload before execution

#### Scenario: Execute bulk action
- **WHEN** frontend sends `SelectedRowIds` for a bulk action
- **THEN** backend validates ids and claims and returns controlled `AppResponses<object>`

### Requirement: Reusable builder and handler strategy
The system MUST implement a reusable `DynamicUiTableBuilder` and `TableId` handler strategy in Services.

#### Scenario: Builder composes dynamic DTO
- **WHEN** service requests table response build
- **THEN** builder orders columns, injects action column if needed, maps rows and separates actions by placement

#### Scenario: Service resolves by TableId without switch explosion
- **WHEN** service receives a `TableId`
- **THEN** it resolves an `IDynamicUiTableHandler` from DI/lookup and delegates query/action behavior

### Requirement: Repository configuration and query safety
The system MUST load UI column configuration from repository using parameterized queries and safe sorting rules.

#### Scenario: Repository returns table column configuration
- **WHEN** service requests columns for `TableId`
- **THEN** repository returns mapped `UiColumnDto` list from `ui_table_columns`

#### Scenario: Sorting whitelist blocks unsafe fields
- **WHEN** request sends `SortField` not present in whitelist
- **THEN** service returns controlled validation response and does not execute unsafe ordering

### Requirement: Claims-based visibility and execution controls
The system MUST apply claims for both action visibility and execution authorization.

#### Scenario: Visible but not executable action is blocked
- **WHEN** user can see an action by `RequiredClaimsAny` but misses `RequiredClaimsAll`
- **THEN** execution is rejected with controlled authorization/validation response

### Requirement: Integration docs and test evidence
The system MUST include docs and tests for frontend integration and backend reliability.

#### Scenario: Documentation delivered for frontend and services
- **WHEN** SCRUM-34 implementation is reviewed
- **THEN** docs include endpoints, DTO contract, query/action examples, and service consumption guide under `Docs/UI/MuiTable`

#### Scenario: Automated tests executed
- **WHEN** validation is executed for SCRUM-34
- **THEN** unit tests and integration tests (or explicit skipped reason) are recorded in evidence
