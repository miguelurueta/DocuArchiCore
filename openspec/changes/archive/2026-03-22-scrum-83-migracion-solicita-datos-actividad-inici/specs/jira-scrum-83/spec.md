## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-83.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-83

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Query workflow start activity data
The system MUST expose a repository flow that queries the start activity row from `wf_registro_actividaes_flujos_trabajo` using `idFlujoTrabajo` and `defaultDbAlias`, preserving the legacy behavior of `Solicita_datos_actividad_inicio_flujo`.

#### Scenario: Start activity exists
- **WHEN** the repository is executed with a valid `idFlujoTrabajo` and `defaultDbAlias`
- **THEN** it queries `wf_registro_actividaes_flujos_trabajo`
- **AND** it filters by `wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = idFlujoTrabajo`
- **AND** it filters by `ACTIVIDAD_INICIO = 1`
- **AND** the response returns `success = true`
- **AND** `message = "YES"`
- **AND** the payload contains `IdRegistroActividadFlujoTrabajo`, `IdActividadFlujoTrabajo` and `IdUsuarioWorkflowFlujoTrabajo`

#### Scenario: No start activity exists
- **WHEN** the repository finds no rows for the requested `idFlujoTrabajo`
- **THEN** the response returns `success = true`
- **AND** `message = "Sin resultados"`
- **AND** all output ids return `0`

#### Scenario: Start activity has null workflow user
- **WHEN** the start activity row has `ID_USUARIO_WORKFLOW = null`
- **THEN** the migrated response returns `IdUsuarioWorkflowFlujoTrabajo = 0`

#### Scenario: Input is invalid or query fails
- **WHEN** `idFlujoTrabajo` is invalid, `defaultDbAlias` is empty, or the query layer fails
- **THEN** the response returns `success = false`
- **AND** the error is wrapped in `AppResponses`
