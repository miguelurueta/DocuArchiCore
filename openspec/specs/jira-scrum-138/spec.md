# jira-scrum-138 Specification

## Purpose
TBD - created by archiving change scrum-138-implementacion-solicita-estructura-confi. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-138.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-138

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Repository query by `NAME_PROCESO`
The system MUST implement a repository query to fetch rows from `ra_config_upload_gestion` filtered only by `NAME_PROCESO`.

#### Scenario: Contract and location
- **WHEN** the backend repository is implemented
- **THEN** it exposes the interface `ISolicitaEstructuraConfiguracionUploadNameProcesoRepository`
- **AND** it exposes the method `SolicitaEstructuraConfiguracionUploadNameProcesoAsync(string nameProceso, string defaultDbAlias)`
- **AND** it returns `AppResponses<List<RaConfiguracionUploadModel>>` (or the project-equivalent `AppResponse<List<...>>`) consistently with existing repositories
- **AND** it follows `IDapperCrudEngine` + `QueryOptions` usage conventions defined in `openspec/context/OPSXJ_BACKEND_RULES.md`

#### Scenario: Input validation for `defaultDbAlias`
- **WHEN** `defaultDbAlias` is `null`, empty, or whitespace
- **THEN** the repository returns `success = false`
- **AND** `data = []`
- **AND** it returns a descriptive validation message per project standard (AppError/AppResponse errors)
- **AND** `IDapperCrudEngine` is not invoked

#### Scenario: Input validation for `nameProceso`
- **WHEN** `nameProceso` is `null`, empty, or whitespace
- **THEN** the repository returns `success = false`
- **AND** `data = []`
- **AND** it returns a descriptive validation message per project standard (AppError/AppResponse errors)
- **AND** `IDapperCrudEngine` is not invoked

#### Scenario: Filter only by `NAME_PROCESO`
- **WHEN** the repository builds its `QueryOptions`
- **THEN** it sets `TableName = "ra_config_upload_gestion"`
- **AND** it sets exactly one filter `Filters["NAME_PROCESO"] = nameProceso.Trim()`
- **AND** it does not add any other filters/conditions beyond `NAME_PROCESO`

#### Scenario: No rows returned
- **WHEN** the repository query returns 0 rows
- **THEN** it returns `success = true`
- **AND** `data = []`
- **AND** `message = "Sin resultados"` (or the project-equivalent standard message)

#### Scenario: Rows returned
- **WHEN** the repository query returns 1+ rows
- **THEN** it returns `success = true`
- **AND** `data` contains the returned rows mapped to `RaConfiguracionUploadModel`
- **AND** `message = "YES"` (or the project-equivalent standard success message)

#### Scenario: Engine exception
- **WHEN** `IDapperCrudEngine` throws an exception while querying
- **THEN** the repository returns `success = false`
- **AND** `data = []`
- **AND** it returns a controlled error per project standard (AppError/AppResponse errors)

