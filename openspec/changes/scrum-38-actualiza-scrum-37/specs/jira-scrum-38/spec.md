## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-38.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-38

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Remove estado_validacion synthetic column
The repository function `BuildColumns` MUST NOT inject the synthetic `estado_validacion` column for `TipoModuloDeConsulta = 1`.

#### Scenario: Validation module request
- **WHEN** `SolicitaEstructuraCamposConsultaCoinsidenciaRadicados` is called with `tipoModuloDeConsulta = 1`
- **THEN** output columns do not contain `estado_validacion`
- **AND** base columns remain unchanged

### Requirement: Keep existing mapping and actions behavior
Removing `estado_validacion` MUST NOT break AutoMapper column mapping or row action generation.

#### Scenario: Mapping and actions after removal
- **WHEN** service builds `DynamicUiTableDto` from repository columns
- **THEN** `DetallePlantillaRadicado -> UiColumnDto` mapping still works
- **AND** `UiActionDto` generation remains active by `TipoModuloDeConsulta`

### Requirement: Documentation and tests updated
Documentation and automated tests MUST reflect the removed column.

#### Scenario: Repository tests
- **WHEN** repository tests execute for `tipoModuloDeConsulta = 1`
- **THEN** they assert that `estado_validacion` is absent in output
