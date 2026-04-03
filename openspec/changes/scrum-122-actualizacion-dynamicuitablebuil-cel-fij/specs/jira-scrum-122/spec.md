## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-122.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-122

### Requirement: UiColumnDto supports optional pinned metadata
The backend dynamic table contract MUST allow each `UiColumnDto` to declare optional pinning metadata compatible with AppTable adapters.

#### Scenario: Contract exposes pinned values
- **WHEN** a backend producer builds `DynamicUiTableDto.Columns`
- **THEN** each `UiColumnDto` may include `Pinned` with values `"left"` or `"right"`
- **AND** `LockPinned` remains optional

### Requirement: Dynamic table normalization preserves valid pinning values
The shared backend dynamic table builder MUST preserve valid pinning metadata and ignore invalid values without breaking current payloads.

#### Scenario: Valid pinned value is preserved
- **WHEN** `DynamicUiTableBuilder` receives a `UiColumnDto` with `Pinned = "left"` or `Pinned = "right"`
- **THEN** the normalized payload keeps the same value
- **AND** `LockPinned` keeps its original boolean value when provided

#### Scenario: Invalid pinned value is ignored
- **WHEN** `DynamicUiTableBuilder` receives a `UiColumnDto` with any other `Pinned` value
- **THEN** the normalized payload clears that property instead of emitting an arbitrary value

### Requirement: Workflow inbox emits fixed action column metadata
The workflow inbox column builder MUST mark its action column as pinned to the right so the frontend can render it as a fixed column.

#### Scenario: Workflow action column is fixed
- **WHEN** `WorkflowDynamicUiColumnBuilder` builds the static `acciones` column for `workflowInboxgestion`
- **THEN** that column includes `Pinned = "right"`
- **AND** `LockPinned = true`
