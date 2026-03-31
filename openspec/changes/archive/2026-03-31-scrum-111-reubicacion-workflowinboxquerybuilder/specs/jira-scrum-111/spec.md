## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-111.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-111

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox query builder service relocation
The system MUST relocate `WorkflowInboxQueryBuilder` out of `MiApp.Repository` and into the workflow service layer without changing its query-building behavior.

#### Scenario: Builder is moved to the service layer
- **GIVEN** `WorkflowInboxQueryBuilder` currently exists under `MiApp.Repository/Repositorio/Workflow/BandejaCorrespondencia`
- **WHEN** the change is implemented
- **THEN** the builder, its interface and policy live under `MiApp.Services/Service/Workflow/BandejaCorrespondencia`
- **AND** their namespace matches the new service location

#### Scenario: DI resolves builder from service namespace
- **WHEN** `DocuArchi.Api` compiles after the relocation
- **THEN** `Program.cs` imports the builder from `MiApp.Services.Service.Workflow.BandejaCorrespondencia`
- **AND** dependency injection still resolves `IWorkflowInboxQueryBuilder`

#### Scenario: Query behavior remains compatible after relocation
- **WHEN** existing unit tests for `WorkflowInboxQueryBuilder` run after the move
- **THEN** the builder still produces the same `QueryOptions`, filters, ordering, paging and joins as before
