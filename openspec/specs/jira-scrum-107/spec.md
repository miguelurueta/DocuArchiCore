# jira-scrum-107 Specification

## Purpose
TBD - created by archiving change scrum-107-crea-dto-bandeja-correspondencia. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-107.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-107

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox DTO contracts
The system MUST define the base DTO and enum contracts for the dynamic workflow inbox in `MiApp.DTOs/DTOs/Workflow/BandejaCorrespondencia`.

#### Scenario: Metadata request contract is required
- **WHEN** a caller needs column metadata by workflow route
- **THEN** `WorkflowRouteColumnConfigRequestDto` exposes `IdRutaWorkflow`, `DefaultDbAlias` and `Mode`
- **AND** `Mode` uses the enum `WorkflowColumnListMode`

#### Scenario: Raw configuration mapping is required
- **WHEN** the metadata source row from `configuracion_listado_ruta` is represented in code
- **THEN** `ConfiguracionListadoRutaDto` exposes exactly the agreed properties for ids, flags and ordering fields

#### Scenario: Resolved metadata result is required
- **WHEN** backend layers prepare resolved dynamic columns
- **THEN** `WorkflowDynamicColumnDefinitionDto` and `WorkflowRouteColumnConfigResultDto` represent column details, mode and the column collection

#### Scenario: Dynamic inbox request is required
- **WHEN** a caller builds the main workflow inbox request
- **THEN** `WorkflowInboxDynamicTableRequestDto` exposes the agreed paging, sorting, route, activity, mode and structured filter fields
- **AND** `StructuredFilters` is initialized as an empty list by default

### Requirement: Scope stays contract-only
The system MUST keep SCRUM-107 limited to DTOs and enums in this phase.

#### Scenario: Non-contract layers are reviewed
- **WHEN** reviewers inspect the change
- **THEN** no controllers, services, repositories, SQL logic, QueryOptions usage or tests are introduced as part of SCRUM-107

