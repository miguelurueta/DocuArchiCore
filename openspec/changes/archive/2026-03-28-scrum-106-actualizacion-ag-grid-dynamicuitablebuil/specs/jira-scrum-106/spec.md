## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-106.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-106

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: DynamicUiTable payload exposes explicit AG Grid aliases
The system MUST expose AG Grid-friendly metadata in `DynamicUiTableDto` without breaking the existing payload consumed by MUI DataGrid.

#### Scenario: Builder normalizes columns for AG Grid and existing adapters
- **WHEN** `DynamicUiTableBuilder` builds a table payload
- **THEN** every column includes explicit aliases for `field`, `dataIndex` and `title`
- **AND** existing fields such as `key`, `columnName` and `headerName` remain available
- **AND** the generated action column follows the same alias normalization

#### Scenario: Builder derives explicit AG Grid filter metadata
- **WHEN** a column is filterable
- **THEN** the payload includes an AG Grid filter hint derived from the semantic `FilterType`
- **AND** action columns or non-filterable columns expose `AgGridFilterType = "none"`

### Requirement: DynamicUiTable compatibility remains backward compatible
The system MUST keep the current response contract consumable by existing MUI DataGrid integrations while documenting the AG Grid mapping.

#### Scenario: Existing MUI integrations consume the updated payload
- **WHEN** a client continues reading `key`, `headerName`, `rows`, `pagination` and `sorting`
- **THEN** the payload structure remains valid without requiring breaking changes

#### Scenario: Frontend team reviews integration guidance
- **WHEN** documentation is updated under `Docs/UI/MuiTable`
- **THEN** it explains the mapping for AG Grid columns, filters, actions, pagination and sorting
