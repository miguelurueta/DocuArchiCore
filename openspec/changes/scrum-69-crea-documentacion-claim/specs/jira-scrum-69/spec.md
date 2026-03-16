## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-69.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-69

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Claims architecture documentation
The system MUST document the current claims-based security architecture of DocuArchiCore using only behavior verified in the codebase.

#### Scenario: Document security architecture
- **WHEN** a reviewer opens the claims documentation package
- **THEN** it explains JWT configuration, token creation, core claim types, claim consumption, and authorization flow
- **AND** it references the real files where each behavior is implemented

### Requirement: Backend claims usage documentation
The system MUST document how claims are consumed in controllers, services, and permission-based logic.

#### Scenario: Document backend claim consumption
- **WHEN** a reviewer inspects the backend usage guide
- **THEN** it includes examples of `ICurrentUserService`, `IClaimValidationService`, and endpoints that validate `defaulalias` or `usuarioid`

### Requirement: Authorization flow diagrams
The system MUST provide textual diagrams that describe the authentication and authorization flow based on the current implementation.

#### Scenario: Document authorization flow
- **WHEN** a reviewer opens the flow document
- **THEN** it includes the path login -> JWT -> claims -> middleware -> controller/service consumption
