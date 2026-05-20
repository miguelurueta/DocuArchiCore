## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-207.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-207

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Resolve gabinete metadata by radicado
The system MUST expose a GET endpoint to resolve gabinete metadata from a `consecutivoRadicado` without requiring `nombreRuta` from frontend.

#### Scenario: Existing radicado in dynamic route table
- **GIVEN** there is one active workflow route with valid `Nombre_Ruta`
- **AND** `dat_adic_tar{Nombre_Ruta}` contains a row for the requested `RADICADO`
- **WHEN** the client calls `GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`
- **THEN** response is `success=true`, `message="YES"`
- **AND** `data.EstadoExistenciaRadicado="YES"`
- **AND** response includes `Radicado`, `IdTareaWorkflow`, `IdGabinete`, `NombreGabinete`

#### Scenario: Non-existing radicado
- **GIVEN** there is one active workflow route with valid `Nombre_Ruta`
- **AND** `dat_adic_tar{Nombre_Ruta}` has no row for the requested `RADICADO`
- **WHEN** the client calls `GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`
- **THEN** response is `success=true`, `message="YES"`
- **AND** `data.EstadoExistenciaRadicado="NO"`
- **AND** `IdTareaWorkflow=0`, `IdGabinete=0`, `NombreGabinete=""`

### Requirement: Resolve gabinete metadata by workflow task id
The system MUST expose a GET endpoint to resolve gabinete metadata from `idTareaWorkflow` without requiring `nombreRuta` from frontend.

#### Scenario: Existing workflow task id in dynamic route table
- **GIVEN** there is one active workflow route with valid `Nombre_Ruta`
- **AND** `dat_adic_tar{Nombre_Ruta}` contains a row for `INICIO_TAREAS_WORKFLOW_ID_TAREA`
- **WHEN** the client calls `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`
- **THEN** response is `success=true`, `message="YES"`
- **AND** `data.EstadoExistenciaRadicado="YES"`
- **AND** response includes `Radicado`, `IdTareaWorkflow`, `IdGabinete`, `NombreGabinete`

#### Scenario: Non-existing workflow task id
- **GIVEN** there is one active workflow route with valid `Nombre_Ruta`
- **AND** `dat_adic_tar{Nombre_Ruta}` has no row for `INICIO_TAREAS_WORKFLOW_ID_TAREA`
- **WHEN** the client calls `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`
- **THEN** response is `success=true`, `message="YES"`
- **AND** `data.EstadoExistenciaRadicado="NO"`

### Requirement: Route name and dynamic table safety
The system MUST validate route name before dynamic table resolution.

#### Scenario: Invalid route name
- **GIVEN** resolved `Nombre_Ruta` does not match `^[A-Za-z0-9_]+$`
- **WHEN** service attempts dynamic table resolution
- **THEN** operation returns `success=false`
- **AND** error includes `Field="Nombre_Ruta"` and `Type="Validation"`

### Requirement: Claims and alias resolution
The system MUST resolve aliases from claims and never from frontend.

#### Scenario: Missing workflow alias claim
- **WHEN** claim `defaulaliaswf` is missing or empty
- **THEN** operation returns `success=false`
- **AND** error includes `Field="defaulaliaswf"` and `Type="Validation"`

#### Scenario: Gabinete alias fallback
- **GIVEN** claim `defaulalias` is missing or empty
- **WHEN** querying `configuracion_gabinete`
- **THEN** system uses `defaulaliaswf` as fallback alias

### Requirement: Backward compatibility with existing endpoint
The change MUST not alter contracts or behavior of `SolicitaExistenciaRadicadoRutaWorkflow`.

#### Scenario: Existing endpoint remains stable
- **WHEN** integration tests execute against existing endpoint
- **THEN** response contract and status behavior remain unchanged
