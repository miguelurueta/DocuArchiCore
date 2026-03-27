## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-105.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-105

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Migrate legacy Solicita_campos_lista_tramite to repository contract
The system MUST provide a repository method `SolicitaCamposListaGestionCorrespondenciaAsync(string defaultDbAlias, int idRutaWorkflow)` that preserves the legacy selection behavior for workflow pending-management columns.

#### Scenario: Route has configured management columns
- **WHEN** the repository queries `configuracion_listado_ruta` with `Rutas_Workflow_id_Ruta = idRutaWorkflow` and `Lista_gestion_tamite = 1`
- **THEN** it returns `success = true`
- **AND** `data` contains a CSV string built from `Nombre_Campo`
- **AND** the values preserve ascending order by `Orden_lista_gestion_tamite`

#### Scenario: Route has no active management columns
- **WHEN** the repository finds no rows for the route
- **THEN** it returns `success = true`
- **AND** `data = null`
- **AND** `message = "Sin resultados"`

#### Scenario: Technical error while querying workflow configuration
- **WHEN** the underlying `DapperCrudEngine` returns `Success = false` or throws an exception
- **THEN** the repository returns a controlled `AppResponses`
- **AND** the error message preserves traceability to `SolicitaCamposListaGestionCorrespondencia`

### Requirement: Migration must avoid unnecessary layers
The migration MUST stay at repository level when the legacy function only performs data lookup and formatting.

#### Scenario: Reviewer inspects implementation scope
- **WHEN** the implementation is reviewed
- **THEN** no new controller or service is created for this migration
- **AND** the existing workflow configuration model is reused when sufficient
