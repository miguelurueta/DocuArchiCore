## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-158.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-158

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API usuario principal respuesta
The API MUST expose `GET /api/gestion-correspondencia/firmas/usuario-principal-respuesta` and return `AppResponses<ResponseDropdownDto>` based on `idUsuarioGestion`.

#### Scenario: Claim requerido ausente
- **WHEN** `defaulalias` or `usuarioid` claim is missing/invalid
- **THEN** the controller returns `BadRequest` with validation response

#### Scenario: Parámetro inválido
- **WHEN** `idUsuarioGestion <= 0`
- **THEN** the controller returns `BadRequest` with validation error

#### Scenario: Usuario encontrado
- **WHEN** repository returns `success=true` with `RemitDestInterno` and active status
- **THEN** service returns `success=true`, `meta.status="success"` and mapped `ResponseDropdownDto`

### Requirement: Reuse legacy repository without contract changes
The implementation MUST reuse `IRemitDestInternoR.SolicitaEstructuraIdUsuarioGestion` without modifying legacy repository contract.

#### Scenario: Repository error
- **WHEN** legacy repository returns `success=false`
- **THEN** service returns controlled `success=false`, `meta.status="error"` and propagates errors

#### Scenario: Repository without data
- **WHEN** legacy repository returns `success=true` with `data=null`
- **THEN** service returns `success=true`, `data=null`, `meta.status="not_found"`

### Requirement: Defensive mapping for dropdown description
The dropdown description MUST be mapped from `Nombre_Remitente` and `Cargo_Remite` with null-safe rules.

#### Scenario: nombre y cargo presentes
- **WHEN** `Nombre_Remitente` and `Cargo_Remite` have values
- **THEN** `Descripcion = "{Nombre_Remitente} - {Cargo_Remite}"`

#### Scenario: nombre ausente
- **WHEN** `Nombre_Remitente` is null/empty
- **THEN** base description uses `"Sin nombre"`

#### Scenario: cargo ausente
- **WHEN** `Cargo_Remite` is null/empty
- **THEN** description contains only the normalized nombre

### Requirement: DI and test coverage
The implementation MUST register new service in `DocuArchi.Api/Program.cs` and include automated tests for controller and service behavior.

#### Scenario: Registro DI
- **WHEN** API starts
- **THEN** `IServiceSolicitaUsuarioPrincipalRespuesta` is registered under `// Services (L)`

#### Scenario: Pruebas mínimas
- **WHEN** unit/contract tests run
- **THEN** they validate invalid claims, invalid input, repository error, not found, and success mapping
