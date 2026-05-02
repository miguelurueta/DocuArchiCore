## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-165.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-165

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Validation pipeline for storage command
The storage orchestration layer MUST execute a validation pipeline that accumulates functional errors before transactional phases.

#### Scenario: Invalid request stops orchestration
- **WHEN** any validator returns at least one error in `StoragePhase.Validation`
- **THEN** `DocumentStorageOrchestrator` throws `StorageValidationException` and does not continue to transaction/filesystem/XML phases

#### Scenario: Valid request keeps orchestrator pending flow
- **WHEN** all validators return no errors
- **THEN** orchestrator returns `AlmacenarDocumentoResult` with `StorageDocumentState.Pending`

### Requirement: Batch preindex validation
When `TipoAlmacenamiento = BatchPreindex`, the system MUST validate controlled preindex availability using `IStoragePreindexReader`.

#### Scenario: Missing preindex in batch mode
- **WHEN** request runs in batch mode and no preindex file is found
- **THEN** validator adds `PREINDEX_NOT_FOUND`

#### Scenario: Invalid preindex path
- **WHEN** controlled preindex path validation fails
- **THEN** validator adds `PREINDEX_PATH_INVALID`

### Requirement: Gabinete metadata and required fields
The system MUST validate request index fields against metadata returned by `IStorageGabineteMetadataProvider`.

#### Scenario: Metadata unavailable
- **WHEN** metadata provider returns empty result
- **THEN** validator adds `GAB_FIELDS_NOT_FOUND`

#### Scenario: Metadata and request mismatch
- **WHEN** metadata field count differs from request field count
- **THEN** validator adds `GAB_FIELDS_MISMATCH`

#### Scenario: Required metadata field missing value
- **WHEN** `EvaluarCamposObligatorios = true` and a required field is empty
- **THEN** validator adds `GAB_REQUIRED_EMPTY`

### Requirement: Legacy options validation
The system MUST resolve legacy storage options using `IStorageOptionsResolver` and enforce minimum inventory/TRD/expediente-unit rules.

#### Scenario: Inventory required by option
- **WHEN** `AplicaInventarioDocumental = true` and inventario is null
- **THEN** validator adds `INV_REQUIRED`

#### Scenario: TRD option with negative identifiers
- **WHEN** `AplicaTrd = true` and any TRD id is negative
- **THEN** validator adds `TRD_INVALID_AREA`, `TRD_INVALID_SERIE`, `TRD_INVALID_SUBSERIE`, or `TRD_INVALID_TIPO_DOCUMENTO`

#### Scenario: Unidad/expediente requires class document
- **WHEN** `AplicaUnidadConservacion = true` and expediente/unidad is informed without `IdClaseDocumento`
- **THEN** validator adds `EXP_CLASE_REQUIRED` and/or `UNI_CLASE_REQUIRED`
