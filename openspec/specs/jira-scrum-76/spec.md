# jira-scrum-76 Specification

## Purpose
TBD - created by archiving change scrum-76-actualiza-servicio-radicacion-entrante. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-76.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-76

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarEntrante request without IdPlantilla
`POST /api/radicacion/registrar-entrante` MUST NOT require or serialize `IdPlantilla` in the public request contract.

#### Scenario: Request DTO serialization
- **WHEN** frontend serializes `RegistrarRadicacionEntranteRequestDto`
- **THEN** the payload does not include `IdPlantilla`

### Requirement: Default template resolution in backend
`RegistrarRadicacionEntranteAsync` MUST resolve the filing template using `SolicitaEstructuraPlantillaRadicacionDefault(defaultDbAlias)` instead of consuming a client-provided `IdPlantilla`.

#### Scenario: Default template available
- **WHEN** `RegistrarRadicacionEntranteAsync` executes with valid claims and request body
- **THEN** the service consults `SolicitaEstructuraPlantillaRadicacionDefault(defaultDbAlias)`
- **AND** uses the resolved `id_Plantilla` for downstream validation and persistence

#### Scenario: Default template not available
- **WHEN** `SolicitaEstructuraPlantillaRadicacionDefault(defaultDbAlias)` returns `success=false` or `data=null`
- **THEN** the process stops with controlled `AppResponses`
- **AND** no registration repository call is executed

### Requirement: Registration flow uses resolved template internally
The registration flow MUST preserve internal use of the resolved template id for dynamic fields, validation, configuration lookup and persistence.

#### Scenario: Successful registration with default template
- **WHEN** the default template is resolved successfully
- **THEN** `SolicitaCamposDnamicos`, `SolicitaConfiguracionPlantillaAsync` and `RegistrarRadicacionEntranteAsync` use that internal template id
- **AND** the response remains wrapped in `AppResponses<RegistrarRadicacionEntranteResponseDto>`

