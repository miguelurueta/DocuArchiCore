## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-86.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-86

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Repository registration returns a dedicated radicado identifier payload
`RegistrarRadicacionEntranteAsync` in repository and service layers MUST expose a `ReturnRegistraRadicacionDto` payload with the persisted radicado identifier and consecutive value.

#### Scenario: Successful registration exposes identifier payload
- **WHEN** the registration transaction completes successfully
- **THEN** the repository response contains `ReturnRegistraRadicacion.ConsecutivoRadicado`
- **AND** the repository response contains `ReturnRegistraRadicacion.IdRadicado`
- **AND** the existing response contract remains compatible for current consumers

### Requirement: Service flow stores and normalizes repository identifier payload
`RegistrarRadicacionEntranteService` MUST keep the repository identifier payload available in the final response, even when older repository implementations only fill legacy fields.

#### Scenario: Repository returns legacy top-level fields only
- **WHEN** the repository response is successful
- **AND** `ReturnRegistraRadicacion` is missing or empty
- **THEN** the service reconstructs `ReturnRegistraRadicacion` from `ConsecutivoRadicado` and available metadata
- **AND** the response remains successful
