## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-200.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-200

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Transactional storage MUST insert dynamic cabinet row
The storage transaction MUST persist one row in the dynamic cabinet table (`context.NombreGabinete`) for every successful storage operation.

#### Scenario: Successful transactional storage persists dynamic cabinet
- **GIVEN** a valid storage request with `nombreGabinete=contabil`
- **WHEN** the transactional coordinator executes and commits
- **THEN** table `contabil` contains one row with `ID = IdAlmacen` returned by the operation
- **AND** base columns `ID`, `DISC`, `PAG`, `DBT`, `IDEX`, `USER`, `DATE1`, `TIME1` are persisted

#### Scenario: Dynamic cabinet insert failure rolls back all DB changes
- **GIVEN** identity reservation and disk quota update succeeded in the same transaction
- **WHEN** the dynamic cabinet insert fails
- **THEN** the transaction is rolled back
- **AND** `system1` and `disco_detalle` updates are not persisted

### Requirement: DBT mapping MUST come from DA_EXTENSION
The cabinet `DBT` value MUST be resolved from `DA_EXTENSION.ESTADO_NORMAL` using file extension mapping and not from TRD type ids.

#### Scenario: Extension normalization accepts dot and non-dot variants
- **GIVEN** an incoming file extension `PDF` or `.PDF`
- **WHEN** extension mapping is resolved
- **THEN** the same `DA_EXTENSION` record is used (case-insensitive, dot-normalized)
- **AND** `DBT` is populated from `ESTADO_NORMAL`

### Requirement: Inventario TRD descriptors MUST be completed
When TRD IDs are present in inventory context, descriptive TRD fields in `registro_producion_documental` MUST not remain null.

#### Scenario: Request provides TRD descriptive names
- **GIVEN** TRD ids and names are present in request
- **WHEN** inventory row is built
- **THEN** `NOMBRE_AREA_DEPARTAMENTO`, `SERIE_DOCUMENTO`, `SUBSERIE_DOCUMENTO` are persisted from request data

#### Scenario: Request omits TRD descriptive names and fallback exists
- **GIVEN** TRD ids are present but names are missing
- **WHEN** inventory row is built
- **THEN** names are resolved from metadata repository before insert
- **AND** persisted descriptive fields are not null

### Requirement: Logical electronic index MUST be inserted when expediente applies
For expediente-enabled flows, the transaction MUST insert `ra_cert_indice_expediente` when prerequisites are met.

#### Scenario: Expediente valid and inventory id exists
- **GIVEN** expediente is active and `IdRegistroProduccionDocumental > 0`
- **WHEN** transactional flow reaches index stage
- **THEN** one row is inserted in `ra_cert_indice_expediente`

#### Scenario: Expediente inactive prevents logical index insert
- **GIVEN** expediente exists but is inactive/closed
- **WHEN** transactional flow evaluates expediente rules
- **THEN** logical index insert is rejected
- **AND** transaction fails according to business rule

### Requirement: Storage changes MUST remain atomic across DB entities
Cabinet, inventory, expediente updates, logical index and workflow log MUST belong to the same DB transaction.

#### Scenario: Any pre-commit failure aborts all writes
- **GIVEN** the flow is executing before commit
- **WHEN** any mandatory step fails
- **THEN** no partial records remain in any affected DB tables

### Requirement: Post-commit physical failure MUST trigger DB compensation
If the physical phase fails after DB commit, compensation MUST revert DB artifacts created by transactional storage.

#### Scenario: Physical phase fails after commit
- **GIVEN** DB commit already happened
- **WHEN** file/XML phase throws an exception
- **THEN** compensation is executed
- **AND** dynamic cabinet row, inventory/workflow artifacts, quota counters and expediente side-effects are reverted according to policy
