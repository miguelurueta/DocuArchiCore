## ADDED Requirements

### Requirement: Disk capacity validation must not require ESTADO_DISCO column
The system MUST validate disk capacity without requiring `disco_detalle.ESTADO_DISCO` to exist.

#### Scenario: Schema without ESTADO_DISCO
- **WHEN** transaction locks `disco_detalle` for the target `gabinete + disco`
- **AND** table schema does not contain `ESTADO_DISCO`
- **THEN** disk validation executes normally
- **AND** request does not fail with unknown-column SQL error.

### Requirement: Legacy parity for disk capacity thresholds
The system MUST replicate legacy VB threshold behavior using `tamdisc` and `numero_imagenes`.

#### Scenario: High-capacity disk over threshold
- **WHEN** `tamdisc > 572523149`
- **AND** `numero_imagenes > 80000`
- **THEN** validation blocks operation with legacy over-capacity error.

#### Scenario: Low-capacity disk over threshold
- **WHEN** `tamdisc < 572523149`
- **AND** `numero_imagenes > 7500`
- **THEN** validation blocks operation with legacy over-capacity error.

#### Scenario: Border tamdisc value
- **WHEN** `tamdisc == 572523149`
- **THEN** threshold rules do not set `SL` by comparison
- **AND** operation is not blocked by threshold rule alone.

### Requirement: Legacy parity for unsynchronized disk state
The system MUST preserve legacy behavior when `numero_imagenes` is null or zero.

#### Scenario: numero_imagenes is null
- **WHEN** locked `disco_detalle` row has `numero_imagenes = null`
- **THEN** validation fails with legacy message for unsynchronized disk (`estado null`).

#### Scenario: numero_imagenes is zero
- **WHEN** locked `disco_detalle` row has `numero_imagenes = 0`
- **THEN** validation fails with legacy message for unsynchronized disk.

### Requirement: Optional compatibility with ESTADO_DISCO
The system MAY use `EstadoDisco == "SL"` as additional blocking condition when available, but it MUST NOT replace legacy primary rule.

#### Scenario: Schema with ESTADO_DISCO marked SL
- **WHEN** `EstadoDisco` is available and equals `SL`
- **THEN** validation blocks operation
- **AND** behavior remains compatible with legacy threshold decisions.

### Requirement: Validation must execute under transaction lock before commit
Disk capacity validation MUST run before commit and while lock context is active.

#### Scenario: Validation fails before persistence
- **WHEN** disk capacity validation fails
- **THEN** transaction coordinator aborts the operation before storage persistence commit
- **AND** returns controlled transactional error.
