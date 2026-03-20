# jira-scrum-81 Specification

## Purpose
TBD - created by archiving change scrum-81-actualiza-dynamic-ui-table-design. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-81.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-81

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: DynamicUiTable contract stays compatible with MUI and Ant Design
The system MUST keep `DynamicUiTableDto` and related metadata consumable from both MUI DataGrid and Ant Design Table without introducing a second backend payload.

#### Scenario: Column metadata maps to both frameworks
- **WHEN** frontend receives `UiColumnDto`
- **THEN** `Key` can be used as stable column identifier
- **AND** `ColumnName` can be mapped to data field/dataIndex
- **AND** `HeaderName` can be mapped to visible title/header

#### Scenario: Current render hints remain backward compatible
- **WHEN** backend emits current `RenderType` values such as `grid_text`, `grid_datetime`, `grid_chip`, or `custom`
- **THEN** they are treated as semantic rendering hints
- **AND** current MUI consumers remain compatible
- **AND** Ant Design adapters can translate them without payload changes

### Requirement: Action metadata remains framework-agnostic
The system MUST define table action metadata as logical UI intent rather than concrete framework components.

#### Scenario: Placement and presentation express intent
- **WHEN** backend emits `Placement` and `Presentation`
- **THEN** `toolbar`, `bulk`, and `row` identify logical placement
- **AND** `button`, `menu_item`, and `icon` identify suggested presentation
- **AND** frontend adapters choose the concrete MUI or Ant Design components

### Requirement: Frontend integration docs cover Ant Design mapping
The system MUST document how `DynamicUiTableDto` maps to both current MUI DataGrid consumption and Ant Design Table consumption.

#### Scenario: Integration guide includes Ant Design example
- **WHEN** frontend or backend teams review `Docs/UI/MuiTable/SCRUM-34-Integracion-Frontend.md`
- **THEN** they find an explicit mapping from `DynamicUiTableDto` to Ant Design `Table`
- **AND** the current MUI example remains available

#### Scenario: Technical builder doc explains framework neutrality
- **WHEN** developers review `Docs/UI/MuiTable/SCRUM-35-DynamicUiTableBuilder-Tecnico.md`
- **THEN** the document explains that the builder emits framework-agnostic metadata
- **AND** shows how columns, rows, pagination, sorting, and actions map to Ant Design

