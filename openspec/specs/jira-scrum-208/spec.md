# jira-scrum-208 Specification

## Purpose
TBD - created by archiving change scrum-208-implementacion-api-valida-firma-digital. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-208.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-208

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Firma electronica lookup endpoint
The system MUST expose `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}` to determine whether a document has an electronic signature record.

#### Scenario: Signed document returns certificate id
- **GIVEN** a valid `idArchivo`, valid `nombreGabinete`, and valid `defaulalias` claim
- **AND** table `ra_cert_registro_certificado_archivo` contains matching rows
- **WHEN** endpoint is executed
- **THEN** response is `AppResponses<FirmaElectronicaDocumentoResponseDto>` with `success=true`, `message="OK"`
- **AND** `data.FirmadoElectronico=true`
- **AND** `data.IdCertificado` equals `ra_cert_certificado_id_certificado` from the latest row

#### Scenario: Unsigned document returns fallback
- **GIVEN** a valid `idArchivo`, valid `nombreGabinete`, and valid `defaulalias` claim
- **AND** table `ra_cert_registro_certificado_archivo` has no matching rows
- **WHEN** endpoint is executed
- **THEN** response is `AppResponses<FirmaElectronicaDocumentoResponseDto>` with `success=true`, `message="OK"`
- **AND** `data.FirmadoElectronico=false`
- **AND** `data.IdCertificado=0`

### Requirement: Deterministic legacy-equivalent selection
The lookup MUST preserve legacy behavior by selecting the latest certificate record by descending registry id.

#### Scenario: Multiple rows select latest one
- **GIVEN** multiple rows exist for the same (`id_archivo`, `nombre_gabinete`)
- **WHEN** lookup is executed
- **THEN** repository orders by `id_registro_certificado_archivo DESC`
- **AND** only first row is used to resolve `IdCertificado`

### Requirement: Mandatory claims and request validation
The endpoint MUST validate claims and inputs before data access.

#### Scenario: Missing defaulalias claim
- **GIVEN** request without `defaulalias`
- **WHEN** endpoint is executed
- **THEN** API returns controlled `BadRequest` with `AppResponses` validation error

#### Scenario: Invalid idArchivo
- **GIVEN** `idArchivo <= 0`
- **WHEN** endpoint is executed
- **THEN** API returns controlled validation error with `Field="idArchivo"`

#### Scenario: Invalid nombreGabinete format
- **GIVEN** `nombreGabinete` empty after trim or not matching `^[A-Za-z0-9_]+$`
- **WHEN** endpoint is executed
- **THEN** API returns controlled validation error with `Field="nombreGabinete"`

### Requirement: Data access policy enforcement
Repository implementation MUST use `DapperCrudEngine` + `QueryOptions` and MUST NOT use manual SQL.

#### Scenario: Repository implementation review
- **WHEN** repository code is reviewed
- **THEN** query is parameterized through `QueryOptions` filters
- **AND** no direct `QueryAsync`/`ExecuteAsync` or string-concatenated SQL is present

### Requirement: Layered error handling and response envelope
Controller, Service and Repository MUST implement `try/catch` and return controlled `AppResponses` contract.

#### Scenario: Repository exception is controlled
- **GIVEN** an exception during data access
- **WHEN** request pipeline completes
- **THEN** response remains in `AppResponses` format
- **AND** no internal stacktrace is exposed

### Requirement: Backward compatibility guard
The change MUST NOT break existing behavior from SCRUM-206 and current claim conventions.

#### Scenario: Existing endpoints remain stable
- **WHEN** regression suite for SCRUM-206-related flows runs
- **THEN** no contract or status-code regressions are introduced by SCRUM-208

