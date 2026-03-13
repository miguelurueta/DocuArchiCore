## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-62.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-62

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Legacy-compatible existence check for workflow radicado
The system MUST provide a migrated implementation of legacy `csfc_verifica_existencia_radicado` preserving functional behavior using Repository + Service + API Controller.

#### Scenario: Existing radicado in workflow route table
- **GIVEN** a valid `consecutivoRadicado` and `nombreRuta`
- **WHEN** the query to `dat_adic_tar{nombreRuta}` returns one row
- **THEN** response is successful and includes `EstadoExistenciaRadicado = "YES"` with `IdTareaWorkflow` from `INICIO_TAREAS_WORKFLOW_ID_TAREA`

#### Scenario: Missing radicado in workflow route table
- **GIVEN** a valid `consecutivoRadicado` and `nombreRuta`
- **WHEN** no records are found
- **THEN** response is successful with `EstadoExistenciaRadicado = "NO"` and `IdTareaWorkflow = 0`

#### Scenario: Invalid input or repository failure
- **GIVEN** invalid parameters or database/query failure
- **WHEN** the repository/service executes the operation
- **THEN** response uses `AppResponses` with controlled errors and no unhandled exception leaks

### Requirement: Security and deterministic query migration
The migrated implementation MUST avoid SQL concatenation and use `QueryOptions` with parameterized filters, including explicit alias via `defaultDbAlias`, as mandated by `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Dynamic table name validation
- **GIVEN** an input `nombreRuta` with invalid characters
- **WHEN** the repository validates inputs
- **THEN** it returns validation error and does not execute the data query
