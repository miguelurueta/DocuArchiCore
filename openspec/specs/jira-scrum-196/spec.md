# jira-scrum-196 Specification

## Purpose
TBD - created by archiving change scrum-196-implementacion-validacion-metada-data-ga. Update Purpose after archive.
## Requirements
### Requirement: Real cabinet metadata resolution
The system MUST resolve cabinet field metadata from database table `detalle_gabienete` using cabinet name with case-insensitive match.

#### Scenario: Metadata loaded for cabinet
- **WHEN** a storage request arrives for cabinet `CONTABIL`
- **THEN** backend loads metadata rows from `detalle_gabienete` for that cabinet
- **AND** metadata is available in validation context before field validation

#### Scenario: Metadata missing for cabinet
- **WHEN** no metadata rows are found for requested cabinet
- **THEN** validation fails with error code `GAB_FIELDS_NOT_FOUND`
- **AND** storage flow is stopped before persistence

### Requirement: Backend field existence validation
The system MUST reject request fields that are not declared in cabinet metadata.

#### Scenario: Unknown field in request
- **WHEN** request includes field name not present in metadata
- **THEN** validation fails with error code `GAB_FIELD_UNKNOWN`

### Requirement: Backend required-field validation
The system MUST enforce required fields from backend metadata independently of UI enable/disable state.

#### Scenario: Required field empty
- **WHEN** required metadata field has empty/null value in request
- **THEN** validation fails with error code `GAB_REQUIRED_EMPTY`

#### Scenario: UI flag does not bypass backend
- **WHEN** metadata indicates field is required and UI flag (`CAMPO_ENABLE_DISABLE`) marks readonly/disabled
- **THEN** backend still validates required constraint

### Requirement: Legacy type parser and compatibility validation
The system MUST parse and validate legacy types declared in metadata.

#### Scenario: Supported legacy type accepted
- **WHEN** metadata type is one of `VARCHAR(n)`, `CHAR(n)`, `INT`, `INTEGER`, `DATE`, `DATETIME`, `TEXT`, `LONGTEXT`
- **THEN** parser resolves normalized type and optional max length

#### Scenario: Unsupported type rejected
- **WHEN** metadata declares unsupported type format
- **THEN** validation fails with error code `GAB_TYPE_UNSUPPORTED`

#### Scenario: Value type mismatch rejected
- **WHEN** provided value cannot be converted to metadata type
- **THEN** validation fails with error code `GAB_FIELD_TYPE_INVALID`

### Requirement: Length validation by metadata
The system MUST validate max length for fixed/parameterized text types.

#### Scenario: Value exceeds VARCHAR length
- **WHEN** metadata declares `VARCHAR(20)` and request value length is greater than 20
- **THEN** validation fails with error code `GAB_FIELD_LENGTH_INVALID`

#### Scenario: TEXT without fixed limit
- **WHEN** metadata declares `TEXT` or `LONGTEXT`
- **THEN** fixed max-length validation is not applied

### Requirement: Optional physical-schema consistency check
The system MUST support optional cross-check between metadata and physical table schema through `INFORMATION_SCHEMA.COLUMNS`.

#### Scenario: Physical schema mismatch
- **WHEN** metadata field or type does not match physical table definition and check is enabled
- **THEN** validation fails with error code `GAB_SCHEMA_MISMATCH`

### Requirement: Metadata cache per alias and cabinet
The system MUST cache metadata by `alias + gabinete` with configurable TTL.

#### Scenario: Cache hit
- **WHEN** a second request uses same alias and cabinet within TTL
- **THEN** metadata is served from cache
- **AND** database query is skipped

#### Scenario: Cache expiration
- **WHEN** cache TTL is expired
- **THEN** metadata is reloaded from database and cache entry is refreshed

### Requirement: Validation pipeline contract preservation
The system MUST keep integration with current `StorageValidationPipeline` contract and error aggregation model.

#### Scenario: Validation errors aggregated
- **WHEN** one or more metadata validations fail
- **THEN** errors are returned through `StorageValidationResult.Errors`
- **AND** downstream storage persistence is not executed

### Requirement: Backend implementation policy compliance
Implementation MUST follow `openspec/context/OPSXJ_BACKEND_RULES.md` for repository structure, DI registration, try/catch, AppResponses, and automated tests.

#### Scenario: Artifact and implementation review
- **WHEN** reviewers validate proposal/design/tasks and code changes
- **THEN** evidence includes impacted repos, interfaces, DI wiring, and test coverage plan

