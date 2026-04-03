## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-125.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-125

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: WorkflowInboxController MUST use real claims
The system MUST resolve `defaulalias` and `usuarioid` from authenticated claims in `WorkflowInboxController`.

#### Scenario: Valid workflow inbox claims
- **WHEN** the request reaches `POST /api/workflowInboxgestion/inboxgestion`
- **THEN** the controller validates `defaulalias`
- **AND** validates `usuarioid`
- **AND** delegates to `WorkflowInboxService` with the parsed `usuarioid` and the real `defaulalias`

### Requirement: WorkflowInboxController MUST reject missing or invalid claims
The system MUST reject requests that do not provide the required claims for workflow inbox execution.

#### Scenario: Missing default alias claim
- **WHEN** `defaulalias` is missing
- **THEN** the controller returns `BadRequest`
- **AND** it does not call `WorkflowInboxService`

#### Scenario: Invalid usuarioid claim
- **WHEN** `usuarioid` exists but is not an integer
- **THEN** the controller throws a security exception
- **AND** it does not call `WorkflowInboxService`
