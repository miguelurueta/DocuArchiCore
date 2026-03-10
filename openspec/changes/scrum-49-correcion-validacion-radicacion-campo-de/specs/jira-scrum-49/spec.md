## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-49.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-49

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Mapeo de descripcion de tramite
La validacion de radicacion entrante MUST aceptar la descripcion enviada en `Tipo_tramite.Descripcion` como fuente para `Descripcion_Documento`.

#### Scenario: Request envia Descripcion en Tipo_tramite
- **WHEN** el request no incluye `Campos["Descripcion_Documento"]`
- **AND** incluye `Tipo_tramite.Descripcion`
- **THEN** la validacion de campos obligatorios usa ese valor para `Descripcion_Documento`

### Requirement: Validacion de dimension alineada al mapeo
La validacion de longitud MUST evaluar `Tipo_tramite.Descripcion` contra la regla de `Descripcion_Documento`.

#### Scenario: Descripcion excede longitud maxima
- **WHEN** `Tipo_tramite.Descripcion` supera la longitud permitida de `Descripcion_Documento`
- **THEN** la respuesta retorna `success=false` y error de tipo `MaxLength`
