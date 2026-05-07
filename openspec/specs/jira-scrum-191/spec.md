# jira-scrum-191 Specification

## Purpose
TBD - created by archiving change scrum-191-implementacion-paridad-ruta-archivos. Update Purpose after archive.
## Requirements
### Requirement: Legacy physical path parity
The storage engine MUST resolve final storage path using legacy route semantics from `SYSTEM1RUT`.

#### Scenario: Final route is built from legacy root
- **GIVEN** a gabinete with active `SYSTEM1RUT.ruta_gabi`
- **WHEN** physical phase executes
- **THEN** final path is resolved as legacy root + gabinete + disco + carpeta
- **AND** `%TEMP%` is not used as final destination

### Requirement: Legacy naming parity for principal and attachments
The storage engine MUST generate file names compatible with legacy `DIG` convention.

#### Scenario: Principal name format
- **WHEN** a principal document is stored
- **THEN** file name follows `DIG########.ext`
- **AND** extension is preserved/normalized from effective document format

#### Scenario: Attachment name format
- **WHEN** attachment files are stored
- **THEN** attachment names follow `DIG########.0000(.ext)`, `DIG########.0001(.ext)`, ...
- **AND** sequence is deterministic and collision-safe

### Requirement: Legacy XML document parity
The storage engine MUST generate XML document metadata compatible with legacy consumers.

#### Scenario: XML document structure
- **WHEN** XML metadata is generated
- **THEN** root and node structure match legacy semantic (`Gabinetes/Gabinete`)
- **AND** required attributes/values are present for retrieval compatibility

### Requirement: Expediente index XML update consistency
The storage engine MUST update expediente index XML safely and consistently.

#### Scenario: Expediente XML route is resolved from expediente routing table
- **GIVEN** an expediente eligible for índice electrónico update
- **WHEN** XML índice route is resolved
- **THEN** route source is `ra_ruta_expediente` (`RUTA`, `DISCO`) + expediente id zero-fill
- **AND** this route is independent from `SYSTEM1RUT`

#### Scenario: Safe update of `DocumentoIndizado`
- **WHEN** expediente electrónico branch applies
- **THEN** index XML is updated with `DocumentoIndizado` entry
- **AND** write path uses safe strategy to prevent corruption on partial failure

### Requirement: Physical compensation on post-commit failures
The storage engine MUST compensate physical artifacts when physical/XML phase fails after DB commit.

#### Scenario: Compensation execution
- **WHEN** physical copy or XML update fails post-commit
- **THEN** compensation manager executes cleanup plan
- **AND** operation returns controlled failure with traceability data

### Requirement: Path security and non-overwrite policy
The storage engine MUST prevent unsafe paths and accidental overwrite.

#### Scenario: Path traversal attempt
- **WHEN** a resolved path escapes allowed storage root
- **THEN** operation fails with controlled physical exception

#### Scenario: Existing target file
- **WHEN** destination file already exists
- **THEN** operation fails with controlled error
- **AND** existing file is never overwritten

### Requirement: Legacy regression evidence
This change MUST provide evidence comparing VB source behavior vs C# implementation for physical/XML phase.

#### Scenario: Regression matrix present
- **WHEN** documentation is reviewed
- **THEN** there is a VB vs C# matrix covering path, naming, XML, attachments, and compensation behavior

