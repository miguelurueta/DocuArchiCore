## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-42.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-42

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Registrar entrante endpoint contract
The system MUST expose `POST /api/radicacion/registrar-entrante` using request/response DTOs wrapped in `AppResponses<T>`.

#### Scenario: Successful registrar entrante request
- **WHEN** a valid request is sent to `POST /api/radicacion/registrar-entrante`
- **THEN** the API returns `AppResponses<RegistrarRadicacionResponse>` with success status
- **AND** response includes generated radicado metadata

### Requirement: Validar entrante endpoint contract
The system MUST expose `POST /api/radicacion/validar-entrante` to execute pre-registration business validations.

#### Scenario: Invalid validar entrante request
- **WHEN** request violates validation rules
- **THEN** API returns `AppResponses<ValidarRadicacionResponse>` with failure and traceable validation errors

### Requirement: Flujo inicial endpoint contract
The system MUST expose `GET /api/radicacion/flujo-inicial` as a separate endpoint from registrar/validar operations.

#### Scenario: Flujo inicial query
- **WHEN** frontend requests initial workflow with required params
- **THEN** API returns `AppResponses<FlujoInicialDto>` with minimal fields required by UI

### Requirement: Atomic transaction for Q01-Q08
The system MUST execute Q01-Q08 inside one application transaction with all-or-nothing behavior.

#### Scenario: Failure inside atomic block
- **WHEN** any query between Q01 and Q08 fails
- **THEN** system performs full rollback
- **AND** no partial persistence remains

### Requirement: Default DB alias propagation
The system MUST propagate `defaultDbAlias` to repository query options for all DB operations.

#### Scenario: Repository execution
- **WHEN** service invokes repository methods for registrar/validar/flujo
- **THEN** each DB query uses `QueryOptions.DefaultAlias = defaultDbAlias`

### Requirement: Functional parity evidence
The system MUST provide automated evidence that migrated behavior preserves critical legacy scenarios.

#### Scenario: Parity test execution
- **WHEN** unit/integration/contract tests run for SCRUM-42
- **THEN** report includes critical-path parity results and transactional rollback checks
