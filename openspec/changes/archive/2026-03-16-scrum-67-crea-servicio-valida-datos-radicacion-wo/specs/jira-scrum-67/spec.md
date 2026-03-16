## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-67.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-67

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: In-memory workflow validation service
The system MUST provide an internal service that validates `RelacionCamposRutaWorklflow` data in memory before workflow task construction continues.

#### Scenario: Validation succeeds
- **GIVEN** a list of `RelacionCamposRutaWorklflow` with values compatible with the declared metadata
- **WHEN** the validation service is executed
- **THEN** it returns `success=true`
- **AND** `message="Validación exitosa"`
- **AND** `data` is empty

#### Scenario: Validation finds functional errors
- **GIVEN** one or more related fields whose assigned data violates declared length, type or explicit required metadata
- **WHEN** the validation service is executed
- **THEN** it returns the complete list of `ValidationError`
- **AND** the caller can stop workflow task construction

### Requirement: No persistence or transport layers for SCRUM-67
SCRUM-67 MUST remain as an internal validation service without new repository or controller components.

#### Scenario: Architecture review
- **WHEN** the implementation is reviewed
- **THEN** there is no new controller for SCRUM-67
- **AND** there is no new repository for SCRUM-67
- **AND** all validations run only with data available in memory

### Requirement: Safe required-rule handling
The validation service MUST NOT invent required-field rules when `RelacionCamposRutaWorklflow` metadata is insufficient.

#### Scenario: Missing required metadata
- **GIVEN** a related field with empty value but no explicit required marker in its metadata
- **WHEN** the validation service is executed
- **THEN** it does not fail the field as `Required`
- **AND** the limitation is documented in technical documentation
