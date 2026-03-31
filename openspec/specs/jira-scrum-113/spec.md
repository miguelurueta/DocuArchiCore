# jira-scrum-113 Specification

## Purpose
TBD - created by archiving change scrum-113-actualiza-repository-workflowinboxcontex. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-113.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-113

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox context resolver must use claim-backed aliases
The system MUST resolve workflow inbox context by using `defaulalias` for gestión repositories and `defaulaliaswf` for workflow repositories through `ICurrentUserService`.

#### Scenario: Resolve workflow context with separated aliases
- **WHEN** `WorkflowInboxContextResolverService.ResolveAsync` receives a valid `idUsuarioGestion`
- **THEN** it reads claim `defaulalias` before calling `SolicitaEstructuraIdUsuarioGestion`
- **AND** it reads claim `defaulaliaswf` before calling `SolicitaEstructuraIdUsuarioWorkflowId`, `SolicitaEstructuraRutaWorkflowAsync` and `SolicitaEstructuraGrupoWorkflow`

### Requirement: Resolver contract is simplified to the real backend input
The service contract MUST receive only `idUsuarioGestion` and derive the rest of the workflow context internally.

#### Scenario: Simplified resolver signature
- **WHEN** the service contract is reviewed
- **THEN** `IWorkflowInboxContextResolverService` exposes `ResolveAsync(int idUsuarioGestion)`
- **AND** it does not require the workflow inbox request DTO as input

### Requirement: Controlled failures for missing claims or repository errors
The resolver MUST return controlled validation or functional errors if claims or dependent repositories fail.

#### Scenario: Missing alias claims
- **WHEN** claim `defaulalias` or `defaulaliaswf` is absent
- **THEN** the service returns `AppResponses<WorkflowInboxResolvedContextDto>` with `success = false`
- **AND** the error points to the missing claim field

