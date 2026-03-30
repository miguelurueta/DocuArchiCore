## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-108.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-108

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include repository scope confirmation, DI registration, AppResponses/try-catch and test requirements

### Requirement: Repository returns dynamic workflow column metadata
The system MUST expose a repository capable of reading `configuracion_listado_ruta` and returning dynamic column metadata for a workflow route without creating a service or controller in this phase.

#### Scenario: Query ListaTarea columns
- **GIVEN** a valid `WorkflowRouteColumnConfigRequestDto` with `Mode = ListaTarea`
- **WHEN** the repository resolves the route configuration
- **THEN** it filters by `Rutas_Workflow_id_Ruta` and `Lista_Tarea = 1`
- **AND** it orders rows by `Ordena_Tarea ASC` and `Id_Configuracion ASC`
- **AND** it returns `AppResponses<WorkflowRouteColumnConfigResultDto?>` with `success = true` and `message = "YES"` when columns exist

#### Scenario: Query ListaGestionTramite columns
- **GIVEN** a valid `WorkflowRouteColumnConfigRequestDto` with `Mode = ListaGestionTramite`
- **WHEN** the repository resolves the route configuration
- **THEN** it filters by `Rutas_Workflow_id_Ruta` and `Lista_gestion_tamite = 1`
- **AND** it orders rows by `Orden_lista_gestion_tamite ASC` and `Id_Configuracion ASC`
- **AND** it returns `AppResponses<WorkflowRouteColumnConfigResultDto?>` with `success = true` and `message = "YES"` when columns exist

#### Scenario: Query returns no matching columns
- **GIVEN** a valid request
- **WHEN** the repository finds no usable rows for the selected mode
- **THEN** it returns `success = true`
- **AND** `message = "Sin resultados"`
- **AND** a non-null result payload with the route and an empty `Columns` collection

### Requirement: Repository validates and normalizes route column configuration
The system MUST validate request data and normalize route column rows into safe dynamic column definitions.

#### Scenario: Invalid request is rejected before hitting the engine
- **GIVEN** a null request, an empty `DefaultDbAlias`, or `IdRutaWorkflow <= 0`
- **WHEN** `GetColumnsByRouteAsync` is executed
- **THEN** it returns `success = false`
- **AND** `errors` contains an `AppError` of type `Validation`
- **AND** `IDapperCrudEngine` is not invoked

#### Scenario: Invalid column names are excluded from the result
- **GIVEN** query rows where `NombreCampo` is blank or does not match `^[A-Za-z0-9_]+$`
- **WHEN** the repository maps the rows
- **THEN** those rows are excluded from `Columns`
- **AND** valid rows continue to be returned

#### Scenario: Data type is normalized for UI consumers
- **GIVEN** rows with `TipoCampo` values from `configuracion_listado_ruta`
- **WHEN** the repository maps them to `WorkflowDynamicColumnDefinitionDto`
- **THEN** textual database types normalize to `text`
- **AND** date/time database types normalize to `date` or `datetime`
- **AND** numeric database types normalize to `number`
- **AND** unknown types normalize to `text`

### Requirement: Repository uses controlled errors and DI registration
The system MUST log execution, wrap technical failures in `AppResponses`, and register the repository in API dependency injection.

#### Scenario: Engine failure returns controlled response
- **GIVEN** the Dapper engine returns an unsuccessful response or throws an exception
- **WHEN** `GetColumnsByRouteAsync` executes
- **THEN** the repository returns `success = false`
- **AND** it includes an `AppError` describing the failure
- **AND** it logs the exception through `ILogger<WorkflowRouteColumnConfigRepository>`

#### Scenario: API startup registers the repository
- **WHEN** the API container is built
- **THEN** `IWorkflowRouteColumnConfigRepository` resolves to `WorkflowRouteColumnConfigRepository`
- **AND** no service or controller is added for this phase
