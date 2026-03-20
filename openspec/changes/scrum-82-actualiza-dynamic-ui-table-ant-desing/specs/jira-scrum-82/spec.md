## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-82.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-82

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: DynamicUiTable exposes explicit Ant Design column aliases
The system MUST expose explicit column aliases that simplify direct consumption from Ant Design Table without breaking current MUI consumers.

#### Scenario: Builder derives Ant Design aliases from current payload
- **WHEN** a `UiColumnDto` is built without explicit Ant Design aliases
- **THEN** backend derives `DataIndex` from `ColumnName`
- **AND** derives `Title` from `HeaderName`
- **AND** preserves `Key`, `ColumnName`, and `HeaderName` for backward compatibility

#### Scenario: Action column remains explicit for Ant Design render
- **WHEN** the builder auto-adds the actions column
- **THEN** the resulting column exposes `DataIndex = "actions"`
- **AND** exposes `Title = "Opciones"`
- **AND** keeps `IsActionColumn = true`

### Requirement: DynamicUiTable exposes filter metadata for Ant Design
The system MUST expose minimal filter metadata so frontend adapters can configure Ant Design filter UX consistently.

#### Scenario: Builder infers semantic filter types
- **WHEN** a column is normalized by the builder
- **THEN** text-like columns expose `FilterType = "text"`
- **AND** date columns expose `FilterType = "date"` or `datetime`
- **AND** numeric/chip-like columns expose `FilterType = "select"`

#### Scenario: Action columns never expose filters
- **WHEN** a column is marked as action column
- **THEN** backend sets `Filterable = false`
- **AND** sets `FilterType = "none"`

### Requirement: Automated evidence covers Ant Design adaptation
The system MUST include automated evidence for the new Ant Design aliases and filter metadata.

#### Scenario: Builder tests verify aliases and filter metadata
- **WHEN** automated tests run for the dynamic table builder
- **THEN** they verify `DataIndex`, `Title`, `Filterable`, and `FilterType`
- **AND** verify backward compatibility of the existing payload fields
