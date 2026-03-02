# jira-scrum-30 Specification

## Purpose
TBD - created by archiving change scrum-30. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-30.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-30

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture, and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal, design, and tasks are reviewed
- **THEN** they explicitly include architecture and testing constraints as implementation baseline

### Requirement: API returns pending radicados for a radication user
The system MUST provide `ApListaRadicadosPendientes` in `DocuArchi.Api/Controllers/Radicacion/Tramite` that resolves `defaulalias` and `usuarioid` from claims and returns pending radicados as `AppResponses` for MUI table consumption.

#### Scenario: Controller delegates to service and returns contract
- **WHEN** frontend calls `ApListaRadicadosPendientes` with a valid bearer token
- **THEN** the controller delegates to `ServiceListaRadicadosPendientes` and returns an `AppResponses` payload

#### Scenario: Controller handles unexpected errors
- **WHEN** an exception happens during controller execution
- **THEN** the response is controlled through `AppResponses` using a `try/catch` wrapper

### Requirement: Service composes user, template, and pending list dependencies
The system MUST implement `ServiceListaRadicadosPendientes` in `MiApp.Service/Service/Radicacion/Tramite` to resolve user context and return pending radicados list.

#### Scenario: Service resolves dependency chain successfully
- **WHEN** service receives `defaultDbAlias`
- **THEN** it retrieves `Relacion_Id_Usuario_Radicacion`, then `IdPlantilla`, then pending radicados list and returns success with data

#### Scenario: Service blocks invalid related radication user
- **WHEN** `Relacion_Id_Usuario_Radicacion` is null or `<= 0`
- **THEN** service returns `success=false` with validation error in `errors` and does not query plantilla or pendientes

#### Scenario: Service returns no-results contract
- **WHEN** repository returns no matching pending radicados
- **THEN** service returns `Success=true`, `Data=null`, and `Message="Sin resultados"`

#### Scenario: Service returns controlled error contract
- **WHEN** any dependency throws an exception
- **THEN** service returns `Success=false`, `ErrorMessage` from exception, and empty data payload

### Requirement: Repository query for pending radicados is parameterized
The system MUST implement `SolicitaListaRadicadosPendientes` in `MiApp.Repository/Repositorio/Radicador/Tramite` using `IDapperCrudEngine` with parameterized filters and `DefaultAlias`.

#### Scenario: Repository applies template and user filters
- **WHEN** repository executes query with `IdPlantilla`, `idUsuarioRadicacion`, and `defaultDbAlias`
- **THEN** it filters `ra_rad_estados_modulo_radicacion` by `id_usuario_radicado`, `system_plantilla_radicado_id_plantilla`, and `estado = 1`

#### Scenario: Repository sets query default alias
- **WHEN** repository builds `QueryOptions`
- **THEN** it sets `DefaultAlias = defaultDbAlias`

#### Scenario: Repository prevents SQL injection
- **WHEN** repository executes the database query
- **THEN** parameters are bound and no dynamic SQL concatenation is used for user input

### Requirement: DTO/model/mapping and test coverage are provided
The system MUST provide DTO/model/mapping and automated tests for the pending radicados flow according to backend rules.

#### Scenario: Output contract supports MUI table fields
- **WHEN** pending radicados are returned
- **THEN** output contains `id_estado_radicado` (hidden), `consecutivo_radicado` (Radicado), `remitente` (Remitente), `fecha_registro` (Fecha), and `Opciones`

#### Scenario: Mapping and registrations are complete
- **WHEN** implementation is wired in application startup
- **THEN** interfaces are registered in `Program.cs` and AutoMapper mapping is added in `Service/Mapping/Radicacion/Tramite`

#### Scenario: Required automated tests are implemented
- **WHEN** validation is executed for SCRUM-30
- **THEN** unit tests, integration tests (MySQL Testcontainers/Docker), and documentation artifacts are included

