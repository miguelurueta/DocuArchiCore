## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-206.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-206

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Effective permissions endpoint for authenticated user
The system MUST expose `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos` and return effective PDF permissions for the authenticated user (`usuarioid` claim).

#### Scenario: Valid request returns effective permissions map
- **GIVEN** a valid `codigoImpl`, and valid claims `defaulalias` + `usuarioid`
- **WHEN** the endpoint is executed
- **THEN** response is `AppResponses<VisorPdfPermissionsResponseDto>` with `Permissions` as `{ "pdf.xxx": true|false }`

#### Scenario: All active permissions are included in response
- **GIVEN** active entries in `ra_vis_per_permiso`
- **WHEN** effective permissions are resolved
- **THEN** every active permission code exists in `Permissions`, even if value is `false` by fallback

### Requirement: Effective permissions endpoint for specific user (admin)
The system MUST expose `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos` for administrative lookup over a target user.

#### Scenario: Admin user can query another user
- **GIVEN** caller has administrative authorization and target user exists
- **WHEN** endpoint is executed
- **THEN** system returns `AppResponses<VisorPdfPermissionsResponseDto>` for `idUsuario` target

#### Scenario: Non-admin user is rejected
- **GIVEN** caller without administrative authorization
- **WHEN** endpoint for specific user is executed
- **THEN** system returns controlled authorization failure without exposing internals

### Requirement: User override mutation endpoints
The system MUST expose user-level override mutation endpoints:
- `PUT /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
- `DELETE /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`

#### Scenario: Upsert overrides replaces or creates targeted records
- **GIVEN** a valid `UpsertUserOverridesRequestDto` payload
- **WHEN** `PUT .../overrides` is executed
- **THEN** override rows are inserted/updated transactionally and response is `AppResponses<SimpleOperationResultDto>`

#### Scenario: Delete override removes targeted row
- **GIVEN** an existing override (`codigoImpl`, `idUsuario`, `codigoPermiso`)
- **WHEN** `DELETE .../overrides/{codigoPermiso}` is executed
- **THEN** targeted override is removed and response is `AppResponses<SimpleOperationResultDto>`

### Requirement: Deterministic precedence rules
The system MUST resolve effective permission values using deterministic precedence:
1) user override, 2) profile permission (if active assignment), 3) implementation default, 4) fallback deny.

#### Scenario: Override wins over all lower sources
- **GIVEN** same permission exists in override, profile and default
- **WHEN** resolution is executed
- **THEN** effective value comes from override

#### Scenario: Missing override uses profile/default/fallback
- **GIVEN** no override for permission
- **WHEN** resolution is executed
- **THEN** system applies profile, then default, then `false` fallback

### Requirement: Per-user API contract without codiperfil input
The API contract MUST be centered on user identity (`idUsuario` or `usuarioid` claim) and MUST NOT require `codiperfil` as request parameter.

#### Scenario: Consumer does not send codiperfil
- **GIVEN** a valid request without `codiperfil`
- **WHEN** permission resolution is executed
- **THEN** system resolves profile context internally from active assignments (`ra_vis_per_usuario_perfil`) when available

### Requirement: Mandatory claims and controlled input validation
The system MUST validate `defaulalias` and `usuarioid`, and sanitize route inputs before any data access.

#### Scenario: Missing defaulalias claim
- **GIVEN** request without `defaulalias`
- **WHEN** endpoint is executed
- **THEN** system returns controlled validation error in `AppResponses`

#### Scenario: Invalid implementation code
- **GIVEN** `codigoImpl` with invalid format
- **WHEN** endpoint is executed
- **THEN** system rejects request with controlled validation failure

### Requirement: Data access policy enforcement
The implementation MUST use `DapperCrudEngine` with `QueryOptions` for permission reads/writes and avoid manual SQL execution patterns.

#### Scenario: Repository implementation review
- **WHEN** repository code is reviewed
- **THEN** all operations use `DapperCrudEngine`/`QueryOptions` with parameterized criteria

### Requirement: Response contract and error handling policy
All endpoints MUST return `AppResponses<T>` and handle exceptions in controller, service and repository with `try/catch`.

#### Scenario: Controlled failure envelope
- **GIVEN** any technical exception in internal layers
- **WHEN** request pipeline is completed
- **THEN** API returns `AppResponses` with `Success=false` and controlled error message
