# jira-scrum-71 Specification

## Purpose
TBD - created by archiving change scrum-71-actualiza-api-lista-radicados-pendientes. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-71.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-71

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, `AppResponses`, `try/catch`, and test requirements

### Requirement: ApListaRadicadosPendientes returns DynamicUiTable
The system MUST update `ApListaRadicadosPendientes` so the API returns `AppResponses<DynamicUiTableDto>` instead of a flat list and uses token claims for `defaulalias` and `usuarioid`.

#### Scenario: Controller resolves claims and delegates to service
- **WHEN** frontend calls `GET /api/tramite/tramites/apListaRadicadosPendientes` with a valid bearer token
- **THEN** the controller resolves `defaulalias` and `usuarioid`
- **AND** delegates to `IListaRadicadosPendientesService`
- **AND** returns a controlled `AppResponses<DynamicUiTableDto>` payload

#### Scenario: Controller handles invalid usuarioid claim
- **WHEN** claim `usuarioid` is missing or cannot be parsed as integer
- **THEN** the endpoint returns a controlled error response through `AppResponses`

### Requirement: Service composes pending radicados into DynamicUiTable
The system MUST adapt `ListaRadicadosPendientesService` to keep the current dependency chain and build a `DynamicUiTableDto` with fixed columns and action metadata.

#### Scenario: Service resolves data chain successfully
- **WHEN** service receives `idUsuarioGestion` and `defaultDbAlias`
- **THEN** it retrieves `Relacion_Id_Usuario_Radicacion`
- **AND** retrieves the default plantilla
- **AND** retrieves pending radicados
- **AND** builds a `DynamicUiTableDto`

#### Scenario: Service blocks invalid related radication user
- **WHEN** `Relacion_Id_Usuario_Radicacion` is null or `<= 0`
- **THEN** service returns `success=false` with validation error in `errors`
- **AND** does not query plantilla or pendientes

#### Scenario: Service returns no-results contract
- **WHEN** no pending radicados exist for the current user/template
- **THEN** service returns `success=true`, `message="Sin resultados"`, and `data=null`

#### Scenario: Service returns controlled exception contract
- **WHEN** any dependency or table build step throws
- **THEN** service returns `success=false` with a controlled exception detail in `errors`

### Requirement: DynamicUiTable action contract includes id_estado_radicado
The system MUST expose an explicit action request contract so frontend can trigger `asignacion de tarea` from the action column using `id_estado_radicado`.

#### Scenario: DynamicUiTable response includes row action request mapping
- **WHEN** pending radicados are returned
- **THEN** the response includes an action column
- **AND** the action metadata defines `ActionId = "asignacion-tarea"`
- **AND** the request mapping includes `RowIdField = "id_estado_radicado"`
- **AND** payload field mapping includes `id_estado_radicado`

#### Scenario: DynamicUiTable response keeps expected row fields
- **WHEN** the table payload is built
- **THEN** row values include `id_estado_radicado`, `consecutivo_radicado`, `remitente`, and `fecha_registro`
- **AND** the action column is appended through `DynamicUiTableBuilder`

### Requirement: Documentation and automated evidence are updated
The system MUST update technical evidence for the pending radicados flow in coordinator artifacts and frontend integration docs.

#### Scenario: SCRUM-30 docs reflect the new response contract
- **WHEN** frontend/backend teams review `Docs/Radicacion/Tramite`
- **THEN** documentation explains the `DynamicUiTableDto` contract
- **AND** shows the action request payload for `asignacion de tarea`

#### Scenario: Automated evidence exists for SCRUM-71
- **WHEN** validation is executed for SCRUM-71
- **THEN** unit tests cover success, no-results, and controlled error paths
- **AND** contract tests cover the controller payload
- **AND** integration evidence is either implemented or explicitly marked as skipped for Docker/Testcontainers

