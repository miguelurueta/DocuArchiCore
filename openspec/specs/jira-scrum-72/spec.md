# jira-scrum-72 Specification

## Purpose
TBD - created by archiving change scrum-72-crea-repository-solicita-estructura-conf. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-72.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-72

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Missing implementation constraints
- **WHEN** proposal, design, and tasks are reviewed
- **THEN** they explicitly include repository scope, DI registration, `AppResponses`, `try/catch`, and test requirements

### Requirement: Repository queries configuracion_listado_ruta by idRuta
The system MUST implement `SolicitaEstructuraConfiguracionListadoRutaRepository` in `MiApp.Repository/Repositorio/Workflow/RutaTrabajo` using `IDapperCrudEngine` and `QueryOptions`, exposing the valid C# method name `SolicitaEstructuraConfiguracionListadoRutaAsync`.

#### Scenario: Repository applies route filter and default alias
- **WHEN** repository executes with `idRuta` and `defaultDbAlias`
- **THEN** it queries table `configuracion_listado_ruta`
- **AND** filters by `Rutas_Workflow_id_Ruta = idRuta`
- **AND** sets `DefaultAlias = defaultDbAlias`

#### Scenario: Repository returns success with rows
- **WHEN** matching rows exist for the route
- **THEN** it returns `success = true`
- **AND** `message = "YES"`
- **AND** `data` contains the matching `ConfiguracionListadoRuta` rows

#### Scenario: Repository returns no-results contract
- **WHEN** no matching rows exist
- **THEN** it returns `success = true`
- **AND** `message = "Sin resultados"`
- **AND** `data = null`

#### Scenario: Repository returns controlled validation
- **WHEN** `idRuta <= 0` or `defaultDbAlias` is empty
- **THEN** it returns `success = false`
- **AND** includes validation details in `errors`
- **AND** does not query the database

#### Scenario: Repository returns controlled exception
- **WHEN** the query layer fails or throws
- **THEN** it returns `success = false`
- **AND** includes the technical detail in `errors`

### Requirement: Model and DI registration match repository scope
The system MUST keep the model and startup registration aligned with the new repository-only flow.

#### Scenario: Model reflects configuracion_listado_ruta columns
- **WHEN** reviewers inspect `ConfiguracionListadoRuta`
- **THEN** properties map to the table columns used by the repository
- **AND** XML comments describe the model purpose and key fields

#### Scenario: Repository is available for internal consumption
- **WHEN** application startup is configured
- **THEN** `ISolicitaEstructuraConfiguracionListadoRutaRepository` is registered in `Program.cs`
- **AND** no new service or API endpoint is introduced for this ticket

### Requirement: Documentation and automated evidence exist
The system MUST provide repository-focused docs and tests for the internal consumption of `configuracion_listado_ruta`.

#### Scenario: Repository docs describe internal consumption
- **WHEN** teams review `Docs/Workflow/RutaTrabajo`
- **THEN** docs explain parameters, response contract, and example internal invocation
- **AND** docs do not describe a new public API endpoint

#### Scenario: Automated evidence is available
- **WHEN** validation is executed for SCRUM-72
- **THEN** unit tests cover success, no-results, and controlled error paths
- **AND** integration evidence exists or is explicitly skipped when Docker/Testcontainers is unavailable

