## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-120.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-120

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: DynamicUiTableDto exposes contextual menu catalog
The system MUST extend `DynamicUiTableDto` so dynamic tables can expose a reusable catalog of contextual menu actions through `MenuActions`.

#### Scenario: Workflow inbox returns menu action catalog
- **WHEN** `WorkflowInboxService` builds the `DynamicUiTableDto`
- **THEN** `CellActions` keeps `BehaviorConfig.menuItems` as `List<string>` or `string[]` of `ActionId`
- **AND** the payload includes `MenuActions` with the full action definitions referenced by those ids

#### Scenario: Menu action catalog remains backward compatible
- **WHEN** a dynamic table does not provide `MenuActions`
- **THEN** the payload still renders existing `ToolbarActions`, `BulkActions`, `RowActions` and `CellActions` without breaking current consumers

### Requirement: UiActionDto supports hierarchical menu items and dividers
The system MUST extend `UiActionDto` to support submenu hierarchies through `Children` and visual separators through `IsDivider`.

#### Scenario: Backend returns submenu hierarchy
- **WHEN** a menu action contains nested options
- **THEN** the parent action uses `Children` with fully resolved `UiActionDto` items
- **AND** children do not depend on additional `menuItems` indirection

#### Scenario: Backend returns divider item
- **WHEN** the backend emits a divider inside `MenuActions`
- **THEN** the action sets `IsDivider = true`
- **AND** the divider does not include `ActionId`, `Request`, executable `Behavior` or nested `Children`

### Requirement: Workflow inbox menu contract is structurally valid
The system MUST validate the workflow inbox contextual menu contract before returning the response.

#### Scenario: Duplicate action ids are rejected
- **WHEN** `MenuActions` contains duplicated executable `ActionId` values across root or child actions
- **THEN** the service returns a controlled failure instead of returning an ambiguous payload

#### Scenario: Cell action references only existing menu items
- **WHEN** `BehaviorConfig.menuItems` is emitted for the workflow inbox action trigger
- **THEN** every referenced id exists in `MenuActions`
- **AND** non-existing references are not returned in the backend payload
