## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-115.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-115

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox API exposes only the public request contract
The system MUST receive workflow inbox requests through a public API DTO that excludes backend-resolved workflow fields.

#### Scenario: API contract omits internal workflow fields
- **WHEN** the workflow inbox endpoint receives a request body
- **THEN** the body type is `WorkflowInboxApiRequestDto`
- **AND** it exposes only paging, sorting, search, `EstadoTramite`, `ColumnMode` and `StructuredFilters`
- **AND** it does not expose `IdUsuarioGestion`, `IdUsuarioWorkflow`, `IdRutaWorkflow`, `IdActividad`, `NombreRuta` or `DefaultDbAlias`

### Requirement: Workflow inbox service builds the internal request in backend
The system MUST derive the internal workflow inbox request from claims plus the resolved workflow context instead of trusting frontend-provided workflow identifiers.

#### Scenario: Service enriches the internal request from context
- **GIVEN** a valid `WorkflowInboxApiRequestDto`
- **AND** valid claims `defaulalias`, `usuarioid` and `defaulaliaswf`
- **WHEN** `WorkflowInboxService.SolicitaBandejaWorkflowAsync` is executed
- **THEN** it resolves the workflow context with `WorkflowInboxContextResolverService`
- **AND** it constructs a `WorkflowInboxDynamicTableRequestDto` using `IdUsuarioGestion`, `IdRutaWorkflow`, `NombreRuta`, `IdActividad` and `IdUsuarioWorkflow` from backend sources
- **AND** it preserves only the public paging, sorting, search and filter fields from the API request

#### Scenario: Context resolution failure is returned as controlled error
- **WHEN** the workflow context cannot be resolved
- **THEN** the service returns a failed `AppResponses<DynamicUiTableDto>`
- **AND** it does not execute the final workflow inbox query

### Requirement: Controller validates claims before delegating
The workflow inbox controller MUST validate `defaulalias` and `usuarioid` claims before delegating to the service.

#### Scenario: Missing alias claim returns bad request
- **WHEN** `defaulalias` is missing or invalid
- **THEN** the controller returns `BadRequest`

#### Scenario: Invalid user id claim throws security exception
- **WHEN** `usuarioid` cannot be parsed as integer
- **THEN** the controller throws `SecurityException` with message `Claim invalido: usuarioid`

#### Scenario: Valid claims delegate with public request contract
- **WHEN** both claims are valid
- **THEN** the controller calls `IWorkflowInboxService.SolicitaBandejaWorkflowAsync`
- **AND** it passes `WorkflowInboxApiRequestDto`, parsed `idUsuarioGestion` and `defaulalias`
