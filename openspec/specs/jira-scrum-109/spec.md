# jira-scrum-109 Specification

## Purpose
TBD - created by archiving change scrum-109-crea-builder-campos-bandeja-gestion-corr. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-109.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-109

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow dynamic UI column builder
The system MUST provide a dedicated builder that transforms workflow dynamic column metadata into `UiColumnDto` entries without consulting repositories or hardcoding dynamic business fields.

#### Scenario: Build dynamic and static UI columns
- **GIVEN** a list of `WorkflowDynamicColumnDefinitionDto`
- **WHEN** `IWorkflowDynamicUiColumnBuilder.Build` is executed
- **THEN** dynamic columns are mapped generically and ordered by `Order`
- **AND** static columns `id_tarea`, `fecha_inicio`, `ESTADO` and `acciones` are appended in that exact order
- **AND** the `acciones` column is marked as `IsActionColumn = true`

#### Scenario: Normalize UI metadata by data type
- **GIVEN** dynamic columns with types `text`, `date`, `datetime` or `number`
- **WHEN** the builder maps each column to `UiColumnDto`
- **THEN** it derives `RenderType`, `FilterType`, `AgGridFilterType`, `Align` and `Width` from the normalized type
- **AND** it fills `Key`, `ColumnName`, `DataIndex`, `Field`, `HeaderName` and `Title` consistently

#### Scenario: No repository access in builder phase
- **WHEN** the workflow dynamic UI column builder executes
- **THEN** it performs only in-memory transformation
- **AND** it does not call repositories, Dapper, QueryOptions or database access services

### Requirement: Dependency injection and automated coverage
The system MUST register the workflow dynamic UI column builder in backend DI and cover it with automated tests.

#### Scenario: Resolve builder from DI
- **WHEN** the backend starts
- **THEN** `IWorkflowDynamicUiColumnBuilder` resolves to `WorkflowDynamicUiColumnBuilder`

#### Scenario: Validate builder behavior with tests
- **WHEN** automated tests run for workflow dynamic UI column composition
- **THEN** they verify empty input, ordering, type normalization, static columns and duplicate key avoidance

