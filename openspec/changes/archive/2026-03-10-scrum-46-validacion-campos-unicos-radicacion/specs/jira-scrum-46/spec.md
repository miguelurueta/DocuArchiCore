## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-46.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-46

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Validate unique dynamic fields in incoming radicacion
The system MUST validate unique dynamic fields from `DetallePlantillaRadicado` and reject duplicate values already stored in the plantilla table.

#### Scenario: Dynamic unique value already exists
- **GIVEN** a dynamic field marked as unique (`Comportamiento_Campo` or `TagSesion` includes `UNICO`)
- **AND** request includes value for that field
- **AND** the value is already registered in table `system_plantilla_radicado.Nombre_Plantilla_Radicado`
- **WHEN** uniqueness validation service executes
- **THEN** it returns `success=false`
- **AND** includes `ValidationError` with `Type="Unique"` for the duplicated field

#### Scenario: Dynamic unique values are not duplicated
- **GIVEN** unique dynamic fields in plantilla
- **AND** request values are not registered in table
- **WHEN** uniqueness validation service executes
- **THEN** it returns `success=true`, `message="Sin resultados"`, `data=null`

#### Scenario: No unique dynamic fields configured
- **GIVEN** plantilla has no dynamic fields marked as unique
- **WHEN** uniqueness validation service executes
- **THEN** it returns `success=true`, `message="Sin resultados"`, `data=null`

### Requirement: Service and repository contracts for unique validation
Unique dynamic field validation MUST follow `Service -> Repository` pattern with parameterized queries and controlled error responses.

#### Scenario: Repository technical exception
- **WHEN** repository query fails
- **THEN** service returns `success=false`
- **AND** includes `AppError` with `Field="IdTipoTramite"` and exception message

#### Scenario: Dependency registration
- **WHEN** API starts
- **THEN** `IValidaCamposDinamicosUnicosRadicacionService` is registered under `// Services (L)`
- **AND** `IValidaCamposDinamicosUnicosRadicacionRepository` is registered under `// Repositories (R)`
