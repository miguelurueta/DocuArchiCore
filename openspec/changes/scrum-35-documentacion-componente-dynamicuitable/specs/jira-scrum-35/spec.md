## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-35.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-35

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: DynamicUiTable docs relocation
The system MUST keep DynamicUiTable documentation under `Docs/UI/MuiTable`.

#### Scenario: Existing SCRUM-34 docs are relocated
- **WHEN** documentation is reviewed for SCRUM-35
- **THEN** `SCRUM-34-Diagramas.md` and `SCRUM-34-Integracion-Frontend.md` exist in `Docs/UI/MuiTable`
- **AND** old location under `Docs/Radicacion/Tramite` is no longer the active source

### Requirement: DynamicUiTableBuilder technical documentation
The system MUST document `DynamicUiTableBuilder` public methods with XML-style technical descriptions.

#### Scenario: Public methods include XML-style docs
- **WHEN** a developer reads technical docs
- **THEN** each public method (`BuildAsync`, `BuildRowsOnlyAsync`) includes purpose, parameter origin, and expected return
- **AND** docs include behavior notes for action column injection and row mapping

### Requirement: Internal service consumption guide
The system MUST document how internal backend services consume DynamicUiTable query/action services.

#### Scenario: Service-to-service usage is documented
- **WHEN** backend team integrates a new handler/service
- **THEN** docs explain `TableId`, `DefaultDbAlias`, claims origin (controller/token), and expected `AppResponses<DynamicUiTableDto>` / `AppResponses<DynamicUiRowsOnlyDto>`

### Requirement: End-to-end execution example with real table shape
The system MUST include a realistic execution example based on `ra_rad_estados_modulo_radicacion`.

#### Scenario: Request/response reference is available
- **WHEN** frontend/backend teams validate payloads
- **THEN** docs include sample query request and response using fields such as `id_estado_radicado`, `consecutivo_radicado`, `remitente`, `fecha_registro`, and `estado`

### Requirement: Frontend integration details for React/MUI
The system MUST document frontend integration details for DynamicUiTable APIs.

#### Scenario: Frontend implementation reference exists
- **WHEN** frontend team consumes DynamicUiTable
- **THEN** docs include API route, DTO mapping, required auth claims, request parameters, and React/MUI consumption example
