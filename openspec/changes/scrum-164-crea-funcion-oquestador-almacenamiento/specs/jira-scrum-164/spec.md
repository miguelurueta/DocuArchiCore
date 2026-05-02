## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-164.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-164

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Storage use case and orchestrator baseline
The system MUST expose a base use case and orchestrator for Storage Engine using SCRUM-163 contracts without infrastructure side effects.

#### Scenario: Base orchestration execution
- **WHEN** `IAlmacenarDocumentoUseCase.ExecuteAsync` receives a valid request
- **THEN** it builds storage context, calls `IDocumentStorageOrchestrator`, maps `AlmacenarDocumentoResult` to `AlmacenarDocumentoResponse`, and returns `AppResponses` success

#### Scenario: Validation and typed error handling
- **WHEN** request fields are invalid or typed storage exceptions occur
- **THEN** the use case returns controlled `AppResponses` errors and avoids DB/FS/XML operations
