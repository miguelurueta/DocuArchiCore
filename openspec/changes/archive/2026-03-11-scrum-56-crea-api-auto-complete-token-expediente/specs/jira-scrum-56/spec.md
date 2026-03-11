## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-56.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-56

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API autocomplete token expediente radicado
The system MUST expose `POST /api/PlantillaRadicado/solicitaAutoCompleteTokenExpedienteRadicado` returning `AppResponses<List<rowTomSelect>>`.

#### Scenario: Request valid with matches
- **GIVEN** claim `defaulalias` is valid
- **WHEN** frontend sends `ParameterAutoComplete.TextoBuscado`
- **THEN** controller delegates to service and returns `success=true` with coincidencias de expediente

#### Scenario: Request valid without matches
- **GIVEN** claim `defaulalias` is valid
- **WHEN** no records match configured fields in `expediente_archivo`
- **THEN** response is `success=true`, `message=\"Sin resultados\"`, `data=null`

### Requirement: Repository multi-field lookup
Repository MUST execute parameterized multi-field lookup over `expediente_archivo` and set `QueryOptions.DefaultAlias = defaultDbAlias`.

#### Scenario: Search across expediente fields
- **WHEN** repository executes query with search text
- **THEN** it applies `LIKE` over `CODIGO_UNICO`, `ALEAS_EXPEDIENTE`, `NOMBRE_PERSONA_EXPEDIENTE`, `IDENTIFICACION_PERSONA_EXPEDIENTE`, `NOMBRE_RESPONSABLE_EXPEDIENTE`, `IDENFICACION_RESPONSABLE_EXPEDIENTE`
- **AND** maps `idValue = ID_EXPEDIENTE` and `texValue = CODIGO_UNICO`

### Requirement: Registration and tests
Implementation MUST register DI and include unit, contract and integration coverage.

#### Scenario: Service wiring and test coverage
- **WHEN** application starts and tests run
- **THEN** interfaces are registered in `Program.cs`
- **AND** tests cover success, no-results and controlled-exception paths
