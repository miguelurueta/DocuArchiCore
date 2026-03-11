## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-55.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-55

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API autocomplete token radicado
The system MUST expose `POST /api/PlantillaRadicado/solicitaAutoCompleteTokenRadicado` returning `AppResponses<List<rowTomSelect>>`.

#### Scenario: Request valid with matches
- **GIVEN** claim `defaulalias` is valid
- **WHEN** frontend sends `ParameterAutoComplete.TextoBuscado`
- **THEN** controller delegates to service and returns `success=true` with matching `consecutivo_rad`

#### Scenario: Request valid without matches
- **GIVEN** claim `defaulalias` is valid
- **WHEN** no records match `consecutivo_rad LIKE '%TextoBuscado%'`
- **THEN** response is `success=true`, `message="Sin resultados"`, `data=null`

### Requirement: Service and repository responsibilities
Service and repository MUST keep separation of concerns and low coupling.

#### Scenario: Service resolves plantilla and repository executes query
- **WHEN** service receives request
- **THEN** service resolves default plantilla via `SolicitaEstructuraPlantillaRadicacionDefault`
- **AND** repository executes parameterized query against resolved table with `DefaultAlias = defaultDbAlias`

### Requirement: Registration and tests
The implementation MUST register DI and include automated tests.

#### Scenario: DI registration and automated coverage
- **WHEN** application starts
- **THEN** interfaces and implementations are registered in `Program.cs`
- **AND** unit, contract and integration tests cover success, no-results and controlled exception paths
