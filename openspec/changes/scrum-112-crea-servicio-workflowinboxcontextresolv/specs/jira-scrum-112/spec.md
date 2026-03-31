## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-112.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-112

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Backend workflow context resolution
The system MUST resolve `IdUsuarioWorkflow`, `IdGrupoWorkflow`, `NombreRuta` and `IdActividad` in backend through existing repositories before the workflow inbox final flow executes.

#### Scenario: Resolve full workflow inbox context
- **WHEN** `WorkflowInboxContextResolverService.ResolveAsync` receives a valid `WorkflowInboxDynamicTableRequestDto` with `IdUsuarioGestion` and a valid `defaultDbAlias`
- **THEN** it resolves `Relacion_Workflow` from `RemitDestInterno`
- **AND** resolves `IdGrupoWorkflow` and `id_Ruta` from `UsuarioWorkflow`
- **AND** resolves `NombreRuta` from `RutasWorkflow`
- **AND** resolves `IdActividad` from `GruposWorkflow`
- **AND** returns `AppResponses<WorkflowInboxResolvedContextDto>` with `success = true`

### Requirement: Frontend must not provide workflow-derived context
The backend contract MUST accept only the base request plus `IdUsuarioGestion`, while `IdUsuarioWorkflow`, `NombreRuta` and `IdActividad` are treated as derived values.

#### Scenario: Base request is extended for backend resolution
- **WHEN** the request contract is reviewed
- **THEN** `WorkflowInboxDynamicTableRequestDto` exposes `IdUsuarioGestion`
- **AND** `WorkflowInboxResolvedContextDto` carries the resolved workflow context

### Requirement: Controlled failures during context resolution
The resolver service MUST return controlled validation or functional errors when intermediate repositories fail or when the workflow context is incomplete.

#### Scenario: Missing intermediate data
- **WHEN** any of the existing repositories returns no data or incomplete data for the workflow context
- **THEN** the service does not throw an unhandled exception
- **AND** it returns `AppResponses<WorkflowInboxResolvedContextDto>` with `success = false`
- **AND** it includes a field-specific error

#### Scenario: Unexpected exception while resolving context
- **WHEN** an exception occurs while querying intermediate repositories
- **THEN** the service returns `AppResponses<WorkflowInboxResolvedContextDto>` with `success = false`
- **AND** it includes an `AppError` of type `Exception`
