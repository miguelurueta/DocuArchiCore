## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-68.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-68

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Query active workflow route structure
The system MUST expose a backend flow that queries active rows from `rutas_workflow` using `defaultDbAlias` and returns the result wrapped in `AppResponses`.

#### Scenario: Active routes exist
- **WHEN** the repository is executed with a valid `defaultDbAlias`
- **THEN** it queries `rutas_workflow` with filter `Estado_Ruta = 1`
- **AND** the service/controller return `success = true`
- **AND** the payload contains the active routes

#### Scenario: No active routes exist
- **WHEN** the repository finds no rows matching `Estado_Ruta = 1`
- **THEN** the response returns `success = true`
- **AND** `message = "Sin resultados"`
- **AND** `data = null`

#### Scenario: Alias is missing or query fails
- **WHEN** `defaultDbAlias` is empty or the query layer fails
- **THEN** the response returns `success = false`
- **AND** the error is wrapped in `AppResponses`
