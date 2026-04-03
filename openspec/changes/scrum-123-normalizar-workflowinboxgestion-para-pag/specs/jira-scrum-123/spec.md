## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-123.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-123

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: workflowInboxgestion MUST return a consistent table payload
The system MUST return a `DynamicUiTableDto` for `workflowInboxgestion` even when the inbox query has no rows.

#### Scenario: Inbox without rows
- **WHEN** `WorkflowInboxService` resolves context and the inbox repository returns zero rows
- **THEN** the response is successful
- **AND** `data` contains a `DynamicUiTableDto`
- **AND** `Rows` is an empty array
- **AND** the message is `Sin resultados`

### Requirement: workflowInboxgestion MUST propagate real user claims
The system MUST include the authenticated user claims in the request used to build the dynamic table payload.

#### Scenario: Authenticated user with workflow claims
- **WHEN** `WorkflowInboxService` builds the `DynamicUiTableDto`
- **THEN** it copies `ICurrentUserService.Permisos` into `Request.UserClaims`
- **AND** the resulting table payload exposes those claims for UI authorization decisions

### Requirement: workflowInboxgestion MUST normalize effective pagination
The system MUST publish pagination values consistent with the effective query rules used by workflow inbox.

#### Scenario: NumeroTareaLista defines the page size
- **WHEN** the resolved workflow context includes `NumeroTareaLista > 0`
- **THEN** the table build request uses that value as `PageSize`
- **AND** it overrides the requested page size from the client

#### Scenario: Invalid page requested
- **WHEN** the client sends `Page <= 0`
- **THEN** the table build request uses `Page = 1`

#### Scenario: Repository returns total records
- **WHEN** the inbox repository returns `TotalRecords`
- **THEN** `WorkflowInboxService` preserves that value in the pagination metadata sent to the dynamic table builder
