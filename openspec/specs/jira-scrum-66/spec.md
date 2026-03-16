## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-66.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-66

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: In-memory assignment service for workflow fields
The system MUST provide an internal service that assigns `DatoCampoPlantilla` for `RelacionCamposRutaWorklflow` using only `RegistrarRadicacionEntranteRequestDto` as input source.

#### Scenario: Successful assignment from request fields
- **GIVEN** a list of `RelacionCamposRutaWorklflow` and a valid `RegistrarRadicacionEntranteRequestDto`
- **WHEN** the assignment service processes the list
- **THEN** each item with a matching `NombreCampoPlantilla` receives its corresponding `DatoCampoPlantilla`
- **AND** the response returns `AppResponses<List<RelacionCamposRutaWorklflow>>` with `success=true`

#### Scenario: Missing source field does not break flow
- **GIVEN** a relation item whose `NombreCampoPlantilla` does not exist in the request
- **WHEN** the assignment service processes the list
- **THEN** the item remains in the output
- **AND** `DatoCampoPlantilla` is returned as empty string

#### Scenario: No relations to process
- **GIVEN** an empty relation list
- **WHEN** the assignment service is invoked
- **THEN** the response returns `success=true`
- **AND** `message="Sin resultados"`
- **AND** `data=[]`

### Requirement: No infrastructure dependency for assignment
The assignment flow MUST remain isolated from persistence and transport concerns.

#### Scenario: Assignment service wiring
- **WHEN** the implementation is reviewed
- **THEN** there is no controller or repository created for SCRUM-66
- **AND** the service is registered in dependency injection for internal use
