# jira-scrum-202 Specification

## Purpose
TBD - created by archiving change scrum-202-implementacion-api-actualiza-pdf. Update Purpose after archive.
## Requirements
### Requirement: Replace PDF content from temporary upload
The system MUST provide an API endpoint to replace the physical PDF content of an existing stored document using an already uploaded temporary file.

#### Scenario: Replace document content successfully
- **GIVEN** an existing document identified by `nombreGabinete` and `idDocumento`
- **AND** a valid temporary file identified by `rutaTemporalId` and `archivoTemporalId`
- **WHEN** the replacement endpoint is executed
- **THEN** the original PDF content is replaced in the final storage path
- **AND** the API returns a successful `AppResponses` payload with replacement metadata

### Requirement: Reuse existing StorageEngine path resolution
The replacement flow MUST reuse existing StorageEngine components for physical and temporary path resolution and traversal protection.

#### Scenario: Path resolution uses existing services
- **WHEN** the system resolves temporary and final paths
- **THEN** it uses existing path resolver services
- **AND** no new parallel pathing implementation is introduced

### Requirement: Create backup before overwrite
Before replacing the physical file, the system MUST create a backup copy under temporary root in a replacement-version subfolder.

#### Scenario: Backup path structure
- **WHEN** a replacement is about to execute
- **THEN** the system creates backup in `replacement-versions/{gabinete}/{idDocumento}/{timestamp}/`
- **AND** the backup file is persisted before overwrite

### Requirement: Register audit in logdocuarchi
After successful replace, the system MUST insert an audit record into `logdocuarchi` including operational metadata and JSON details in `CAMPOS`.

#### Scenario: Audit fields are complete
- **WHEN** replacement completes successfully
- **THEN** `logdocuarchi` is inserted with `desc_op`, `USER_OPER`, `DATE_TRANS`, `RUT_DOCU`, `GABINETE`, `IP_TRANS`, `HORA_REGISTRO`, `MODULO_REGISTRO`
- **AND** `CAMPOS` includes `idDocumento`, `rutaTemporalId`, `archivoTemporalId`, `rutaArchivoOriginal`, `rutaRespaldo`, `hashAnterior`, `hashNuevo`, `tamanoAnterior`, `tamanoNuevo`, `motivo`

### Requirement: Hash and size evidence
The system MUST compute and return previous and new file hash/size to provide replace evidence.

#### Scenario: Evidence returned in response
- **WHEN** replacement is successful
- **THEN** response includes `hashAnterior`, `hashNuevo`, `tamanoAnterior`, and `tamanoNuevo`

### Requirement: Controlled failure behavior
The system MUST return controlled error responses for replacement failures and preserve consistency according to hybrid DB/FS behavior.

#### Scenario: Replace fails before write completion
- **WHEN** file replacement fails
- **THEN** the API returns controlled error response
- **AND** no success audit record is written

#### Scenario: Replace succeeds but audit insert fails
- **WHEN** physical replacement succeeds and audit insert fails
- **THEN** the API returns controlled error response
- **AND** the inconsistency is logged for operational follow-up

