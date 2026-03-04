## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-37.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-37

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API de coincidencia de radicados
The system MUST expose an authenticated API endpoint to retrieve radicado matches using a search text and a module type.

#### Scenario: Request with valid claims and request payload
- **GIVEN** a caller has `defaulalias` and `usuarioid` claims
- **WHEN** it calls `POST /api/tramite/consulta-radicacion/apListaCoinsidenciaRadicados`
- **THEN** the API returns `AppResponses<DynamicUiTableDto>` with `success=true` and a MUI-compatible table payload

#### Scenario: Invalid claim value
- **WHEN** `usuarioid` is missing or non-numeric
- **THEN** the API returns a controlled error response

### Requirement: Service orchestration and response contract
The service layer MUST implement `ServiceListaCoinsidenciaRadicados` using Controller -> Service -> Repository and return data wrapped in `AppResponses`.

#### Scenario: No rows found
- **WHEN** repositories return no records
- **THEN** the service returns `success=true`, `message="Sin resultados"`, and `data=null`

#### Scenario: Unexpected exception
- **WHEN** an internal exception occurs
- **THEN** the service returns `success=false` with `errors` including exception details

### Requirement: Dynamic column and row retrieval
The repository layer MUST resolve visible columns from `detalle_plantilla_radicado`, include base columns, and query matches with parameterized LIKE conditions.

#### Scenario: Build visible columns
- **WHEN** default template exists
- **THEN** repository returns base and dynamic columns as `CamposConsultaCoinsidenciaRadicadosDTO`

#### Scenario: SQL injection hardening in search values
- **WHEN** a search text is provided
- **THEN** values are passed as query parameters and not concatenated into SQL literals

### Requirement: Dependency registration and mapping
The API and service projects MUST register new interfaces and mapping profile for consulta radicacion.

#### Scenario: Runtime composition
- **WHEN** application starts
- **THEN** DI contains `IConsultaCoinsidenciaRadicadosRepository` and `IListaCoinsidenciaRadicadosService`
- **AND** AutoMapper registers `DetallePlantillaRadicado -> CamposConsultaCoinsidenciaRadicadosDTO`

### Requirement: Automated test coverage for new flow
The codebase MUST include unit tests for the new service and repository paths.

#### Scenario: Service success path test
- **WHEN** dependencies return columns and rows
- **THEN** service test validates `success=true` and `DynamicUiTableDto` payload creation

#### Scenario: Repository validation path test
- **WHEN** required inputs are missing
- **THEN** repository test validates controlled validation errors without opening DB connections
