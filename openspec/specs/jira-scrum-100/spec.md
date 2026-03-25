# jira-scrum-100 Specification

## Purpose
TBD - created by archiving change scrum-100-importar-actualiza-estado-registro-modul. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-100.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-100

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Legacy function Actualiza_estado_registro_modulo_radicacion is migrated to current repository
The system MUST migrate the legacy update of `ra_rad_estados_modulo_radicacion.estado` into `RaRadEstadosModuloRadicacionR` with the current backend patterns.

#### Scenario: Successful update
- **GIVEN** `defaultDbAlias`, `idRegistroEstado` and `estado` are valid
- **WHEN** `ActualizaEstadoModuloRadicacio` executes
- **THEN** it updates `ra_rad_estados_modulo_radicacion.estado`
- **AND** returns `AppResponses<bool>` with `success=true`

#### Scenario: Invalid alias
- **GIVEN** `defaultDbAlias` is empty
- **WHEN** `ActualizaEstadoModuloRadicacio` executes
- **THEN** it returns a controlled validation error

#### Scenario: Repository exception
- **GIVEN** the data access layer throws an exception
- **WHEN** `ActualizaEstadoModuloRadicacio` executes
- **THEN** it returns `success=false` with controlled error details

