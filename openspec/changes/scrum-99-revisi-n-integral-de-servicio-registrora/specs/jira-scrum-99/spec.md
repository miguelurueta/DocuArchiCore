## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-99.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-99

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: SCRUM-99 produces a production-oriented technical review
The change MUST deliver a review of `RegistroRadicadoTareaWorkflowRepository` and the workflow service integration with concrete findings, severity and recommendations.

#### Scenario: Review identifies repository architecture risks
- **WHEN** the review inspects `RegistroRadicadoTareaWorkflowRepository`
- **THEN** it evaluates layering, transaction handling, dynamic SQL safety and metadata access
- **AND** it records concrete findings with impact and severity

#### Scenario: Review inspects service integration
- **WHEN** the review inspects `RegistrarRadicacionEntranteService`
- **THEN** it evaluates how the repository result is consumed inside the workflow flow
- **AND** it records dead state, coupling or maintainability risks when present
