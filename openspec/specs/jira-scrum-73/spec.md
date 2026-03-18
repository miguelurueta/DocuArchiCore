# jira-scrum-73 Specification

## Purpose
TBD - created by archiving change scrum-73-crea-registro-atomico-radicado-tarea-wor. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-73.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-73

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Atomic workflow task registration
The system MUST register the workflow task atomically across `INICIO_TAREAS_WORKFLOW`, `DAT_ADIC_TAR{NombreRuta}` and `ESTADOS_TAREA_WORKFLOW`.

#### Scenario: Successful atomic insert
- **WHEN** `RegistrarTareaWorkflowAsync` receives valid input and all inserts succeed
- **THEN** the repository commits the transaction
- **AND** returns `AppResponses<RegistroTareaWorkflowResultDto>` with the generated `idTareaWorkflow`

#### Scenario: Failure on dynamic route table insert
- **WHEN** the insert into `DAT_ADIC_TAR{NombreRuta}` fails
- **THEN** the repository rolls back the full transaction
- **AND** no partial data remains committed in the previous tables

### Requirement: Route name validation before dynamic table insert
The system MUST validate `NombreRuta` against `rutas_workflow` before concatenating the dynamic table name.

#### Scenario: Invalid route name
- **WHEN** `NombreRuta` is not present for `idRuta` in `rutas_workflow`
- **THEN** the repository returns a controlled error
- **AND** the transaction is rolled back

### Requirement: No extra service or API layer
The system MUST keep this capability as a repository-only backend function.

#### Scenario: Architecture review
- **WHEN** the implementation is reviewed
- **THEN** no controller or service is introduced for this ticket
- **AND** only DTO, model, repository, DI, tests and documentation are added

