## ADDED Requirements

### Requirement: Technical file classification must come from DA_EXTENSION by file extension
Storage Engine MUST resolve technical file classification from `DA_EXTENSION` using the document extension, not from `TRD.IdTipoDocumento`.

#### Scenario: Extension mapping exists
- **GIVEN** an incoming document with extension `pdf`
- **WHEN** the engine queries `DA_EXTENSION`
- **THEN** it resolves by `ESTENSION = 'PDF'` (normalized)
- **AND** it obtains `ESTADO_NORMAL`, `ESTADO_ADJUNTO`, `ESTADO_LINK`

#### Scenario: Extension mapping does not exist
- **GIVEN** an incoming document extension without row in `DA_EXTENSION`
- **WHEN** storage is requested
- **THEN** the flow returns a functional validation error before commit
- **AND** does not reach physical post-commit failure for that cause

### Requirement: Gabinete DBT must be derived from DA_EXTENSION.ESTADO_NORMAL
For gabinete insert (`contabil`), field `DBT` MUST come from `DA_EXTENSION.ESTADO_NORMAL`.

#### Scenario: TRD value differs from technical classification
- **GIVEN** `TRD.IdTipoDocumento = 43`
- **AND** extension `PDF` maps to `ESTADO_NORMAL = X` in `DA_EXTENSION`
- **WHEN** gabinete row is generated
- **THEN** `DBT = X`
- **AND** `DBT` is not forced to `43`

### Requirement: Mandatory gabinete technical fields are fully populated
The engine MUST populate mandatory gabinete base fields for insert consistency.

#### Scenario: contabil mandatory base fields
- **WHEN** a gabinete row is prepared for `contabil`
- **THEN** fields `ID`, `DISC`, `PAG`, `DBT`, `IDEX`, `USER`, `DATE1`, `TIME1` are populated
- **AND** `TIME1` is always provided because column is `NOT NULL`
