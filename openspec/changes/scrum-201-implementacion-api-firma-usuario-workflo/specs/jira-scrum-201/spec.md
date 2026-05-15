## ADDED Requirements

### Requirement: Workflow claims validation
The API MUST require valid workflow claims before querying signature data.

#### Scenario: Missing `defaulaliaswf` claim
- **WHEN** request is received without `defaulaliaswf`
- **THEN** API returns validation error in `AppResponses` and does not query repository

#### Scenario: Missing `IdUsuarioWorkflow` claim
- **WHEN** request is received without `IdUsuarioWorkflow`
- **THEN** API returns validation error in `AppResponses` and does not query repository

### Requirement: Signature lookup by workflow identity
The API MUST resolve workflow signature using `IdUsuarioWorkflow` from claim and alias from `defaulaliaswf`.

#### Scenario: Valid claim-based lookup
- **WHEN** claims are valid and user exists in `usuario_workflow`
- **THEN** repository is queried by `idU_suario = IdUsuarioWorkflow` and `Firma_Usuario` is loaded

### Requirement: Temporary signature resource generation
The API MUST generate a temporary file resource from signature blob and return consumable metadata.

#### Scenario: Signature exists
- **WHEN** `Firma_Usuario` contains bytes
- **THEN** system writes a temporary file in signatures temp folder
- **AND** returns `IdUsuarioWorkflow`, `FileName`, `ContentType`, `RelativePath`/`UrlTemporal`, and `ExpiresAt`

#### Scenario: Signature missing
- **WHEN** `Firma_Usuario` is null or empty
- **THEN** API returns validation error and does not return file metadata

### Requirement: Secure response and storage behavior
The API MUST avoid exposing sensitive filesystem or blob details.

#### Scenario: Success response
- **WHEN** temporary resource is generated
- **THEN** response does not include raw blob bytes
- **AND** response does not include absolute physical server path

#### Scenario: Invalid request path input
- **WHEN** any computed file name/path is unsafe
- **THEN** API rejects operation with controlled error
