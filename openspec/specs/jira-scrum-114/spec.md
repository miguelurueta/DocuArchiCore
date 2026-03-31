# jira-scrum-114 Specification

## Purpose
TBD - created by archiving change scrum-114-crea-api-bandeja-workflow-gestion-corres. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-114.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-114

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox API returns dynamic inbox table for gestion
The system MUST expose an API endpoint that returns the workflow inbox as a `DynamicUiTableDto` for the authenticated gestion user.

#### Scenario: Valid request returns workflow inbox table
- **GIVEN** a request with filters, paging and sort information
- **AND** the authenticated user provides valid `defaulalias` and `usuarioid` claims
- **WHEN** the client calls the workflow inbox gestion endpoint
- **THEN** the controller delegates to the application service
- **AND** the response payload is a `DynamicUiTableDto`

### Requirement: Workflow inbox service resolves workflow context before querying
The system MUST resolve workflow context from the gestion user and use that resolved context as the source of truth for the inbox query.

#### Scenario: Resolved context defines route and workflow alias
- **GIVEN** a gestion user id and a valid workflow alias claim
- **WHEN** the workflow inbox service resolves the context
- **THEN** it obtains `IdUsuarioWorkflow`, `IdActividad`, `IdRutaWorkflow` and route name from the workflow user repository
- **AND** it uses that resolved context to request dynamic columns and build the inbox query

#### Scenario: Missing workflow alias claim blocks inbox generation
- **WHEN** the workflow alias claim is missing or empty
- **THEN** the workflow inbox service returns a validation failure
- **AND** the inbox query is not executed

### Requirement: Workflow inbox query uses dynamic route metadata
The system MUST build the workflow inbox query with the resolved workflow route, route-specific dynamic columns and workflow alias.

#### Scenario: Query builder prefers resolved context over request defaults
- **GIVEN** a request that contains legacy workflow values
- **AND** a resolved workflow context with different route, activity and workflow user values
- **WHEN** the query builder generates the `QueryOptions`
- **THEN** it uses the resolved route name as table name
- **AND** it uses the workflow alias as default alias
- **AND** it filters by resolved `IdActividad` and `IdUsuarioWorkflow`

### Requirement: Workflow inbox response includes row actions
The system MUST include the standard workflow action cell in the generated inbox table.

#### Scenario: Dynamic table contains action metadata
- **WHEN** the workflow inbox service builds the `DynamicUiTableDto`
- **THEN** the result includes an `acciones` cell action
- **AND** the action exposes the workflow menu items for gestionar, reasignar, redireccionar externo y archivar tramite

