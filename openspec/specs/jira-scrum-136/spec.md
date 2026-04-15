# jira-scrum-136 Specification

## Purpose
TBD - created by archiving change scrum-136-crea-repository-solicita-estructura-resp. Update Purpose after archive.
## Requirements
### Requirement: Repository query by `ID_TAREA_WF`
The system MUST provide a repository query to read rows from `ra_respuesta_radicado` filtering **only** by `ID_TAREA_WF`.

#### Scenario: Invalid `defaultDbAlias`
- **GIVEN** `defaultDbAlias` is null, empty or whitespace
- **WHEN** `SolicitaEstructuraRespuestaIdTareaAsync` is called
- **THEN** it returns `success = false`, `data = []`, and a validation message consistent with the project standard

#### Scenario: Invalid `idTareaWf`
- **GIVEN** `idTareaWf <= 0`
- **WHEN** `SolicitaEstructuraRespuestaIdTareaAsync` is called
- **THEN** it returns `success = false`, `data = []`, and a validation message consistent with the project standard

#### Scenario: No rows found
- **GIVEN** a valid `defaultDbAlias` and `idTareaWf > 0`
- **WHEN** the query returns no rows
- **THEN** it returns `success = true`, `data = []`, `message = "Sin resultados"` (or the equivalent standard message)

#### Scenario: Rows found
- **GIVEN** a valid `defaultDbAlias` and `idTareaWf > 0`
- **WHEN** the query returns one or more rows
- **THEN** it returns `success = true`, `data = <list>`, and a standard success message

#### Scenario: Engine throws exception
- **GIVEN** a valid `defaultDbAlias` and `idTareaWf > 0`
- **WHEN** the underlying data access engine throws
- **THEN** it returns `success = false`, `data = []`, and a controlled error consistent with the repository standard

### Requirement: Repository contract and location
The implementation MUST follow the existing repository + `IDapperCrudEngine` + `QueryOptions` pattern.

#### Scenario: Repository contract
- **WHEN** the repository is reviewed
- **THEN** it exposes `ISolicitaEstructuraRespuestaIdTareaRepository`
- **AND** it includes `Task<AppResponses<List<RaRespuestaRadicado>>> SolicitaEstructuraRespuestaIdTareaAsync(long idTareaWf, string defaultDbAlias);`
- **AND** it is placed under `MiApp.Repository/Repositorio/GestionCorrespondencia/`

### Requirement: Model definition
The system MUST define `RaRespuestaRadicado` under `MiApp.Models/Models/GestionCorrespondencia/` following the dominant mapping style already used in `MiApp.Models`.

#### Scenario: Model follows existing mapping convention
- **WHEN** `RaRespuestaRadicado` is implemented
- **THEN** it uses the same mapping convention already predominant in `MiApp.Models` (attributes vs plain POCO)

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-136.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-136

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

Reference: `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

