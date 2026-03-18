# jira-scrum-74 Specification

## Purpose
TBD - created by archiving change scrum-74-crea-repository-solicita-estructura-conf. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-74.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-74

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Query configuration list by plantilla id
The system MUST expose a repository-only function that queries `ra_rad_config_plantilla_radicacion` by `system_plantilla_radicado_id_Plantilla`.

#### Scenario: Existing configuration rows
- **WHEN** `SolicitaListaEstructuraConfiguracionPlantillaRadicacionAsync` is executed with a valid `idPlantilla`
- **THEN** it returns `AppResponses<List<RaRadConfigPlantillaRadicacion>?>`
- **AND** the query uses `DefaultAlias = defaultDbAlias`

#### Scenario: No matching rows
- **WHEN** the table has no records for the requested `idPlantilla`
- **THEN** the repository returns `success = true`
- **AND** `data = null`
- **AND** `message = "Sin resultados"`

### Requirement: Keep scope repository-only
The system MUST implement this ticket without adding new service or API layers.

#### Scenario: Architecture review
- **WHEN** the implementation is reviewed
- **THEN** it only adds repository wiring, tests, OpenSpec artifacts and documentation for internal use

