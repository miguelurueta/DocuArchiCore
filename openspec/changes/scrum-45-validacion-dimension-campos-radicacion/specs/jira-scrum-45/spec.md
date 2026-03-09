## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-45.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-45

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Dimension validation for incoming radicacion fields
The system MUST validate that fixed and dynamic fields sent by frontend do not exceed the allowed length for the radicacion template.

#### Scenario: Dynamic field exceeds configured size
- **GIVEN** a dynamic field defined in `DetallePlantillaRadicado` with `tam_campo=10`
- **AND** request payload contains the same field with a value of length 11+
- **WHEN** `ValidaDimensionCamposAsync` is executed
- **THEN** it returns `success=false`
- **AND** includes a `ValidationError` with `Type="MaxLength"` for that field

#### Scenario: Fixed field exceeds DB column length
- **GIVEN** the template table configured in `system_plantilla_radicado.Nombre_Plantilla_Radicado`
- **AND** `information_schema.columns` reports a max length for a fixed field
- **WHEN** frontend sends a value larger than that max length
- **THEN** the service returns `success=false` with validation errors

#### Scenario: No template metadata available
- **GIVEN** there is no row in `system_plantilla_radicado` for `IdPlantilla`
- **WHEN** `ValidaDimensionCamposAsync` runs
- **THEN** it returns `success=true`, `message="Sin resultados"`, `data=null`

### Requirement: Service and repository contracts
Dimension validation MUST follow `Service -> Repository` separation with controlled error handling and AppResponses contract.

#### Scenario: Repository throws exception
- **WHEN** repository operation fails unexpectedly
- **THEN** service returns `success=false`
- **AND** response includes `AppError` with `Field="IdTipoTramite"` and exception message

#### Scenario: Dependency injection registration
- **WHEN** application starts
- **THEN** `IValidaDimensionCamposService` is registered in `Program.cs` under `// Services (L)`
- **AND** `IValidaDimensionCamposRepository` is registered under `// Repositories (R)`
