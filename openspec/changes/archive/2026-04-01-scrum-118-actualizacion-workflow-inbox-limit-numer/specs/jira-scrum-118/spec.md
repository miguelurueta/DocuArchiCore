## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-118.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-118

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Resolve workflow inbox limit from persisted user configuration
The system MUST resolve `Numero_Tarea_Lista` from `configuracion_usuario` during workflow inbox context resolution and expose it in the resolved context without breaking the existing controller or service contract.

#### Scenario: Valid persisted configuration is available
- **GIVEN** `WorkflowInboxContextResolverService.ResolveAsync` resolves a valid workflow user
- **AND** `ISolicitaConfiguracionListaUsuarioWorkflowRepository` returns a record with `Numero_Tarea_Lista > 0`
- **WHEN** the resolved context is returned
- **THEN** `WorkflowInboxResolvedContextDto.NumeroTareaLista` contains that value

#### Scenario: Configuration does not exist or is invalid
- **GIVEN** `WorkflowInboxContextResolverService.ResolveAsync` resolves the workflow context successfully
- **AND** the configuration repository returns no record, `null`, `0`, or a negative number for `Numero_Tarea_Lista`
- **WHEN** the resolved context is returned
- **THEN** `WorkflowInboxResolvedContextDto.NumeroTareaLista` is `null`
- **AND** the workflow inbox flow continues without failing

### Requirement: Query builder prioritizes resolved workflow limit
The system MUST use `WorkflowInboxResolvedContextDto.NumeroTareaLista` as the effective query limit for workflow inbox data when that value is valid, with fallback to the existing pagination rules otherwise.

#### Scenario: Context limit overrides request page size
- **GIVEN** `WorkflowInboxQueryBuilder.Build` receives a context with `NumeroTareaLista > 0`
- **WHEN** query options are built
- **THEN** `QueryOptions.Limit` uses `NumeroTareaLista`
- **AND** `QueryOptions.Offset` is calculated from that effective limit

#### Scenario: Invalid context limit falls back to current behavior
- **GIVEN** `WorkflowInboxQueryBuilder.Build` receives a context with `NumeroTareaLista` missing or invalid
- **WHEN** query options are built
- **THEN** `QueryOptions.Limit` falls back to `request.PageSize`
- **AND** if `request.PageSize` is invalid, the default limit remains `1000`
