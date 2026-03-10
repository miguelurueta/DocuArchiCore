## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-47.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-47

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Orquestador de validaciones de radicacion
`RegistrarRadicacionEntranteAsync` MUST delegar la validacion de campos a un servicio orquestador dedicado para reducir acoplamiento.

#### Scenario: Servicio de orquestacion ejecuta validaciones
- **WHEN** se invoca `ValidaCamposRadicacionAsync`
- **THEN** ejecuta `ValidaCamposObligatorios`, `ValidaDimensionCampos` y `ValidaCamposDinamicosUnicosRadicacion`
- **AND** consolida errores en `AppResponses<List<ValidationError>?>`
- **AND** maneja excepciones con `try/catch` retornando `success=false`

### Requirement: RegistrarRadicacionEntrante usa orquestador
El metodo `RegistrarRadicacionEntranteAsync` MUST invocar el orquestador y detener el flujo de persistencia cuando la validacion falle.

#### Scenario: Validacion fallida detiene registro
- **WHEN** el orquestador retorna `success=false`
- **THEN** `RegistrarRadicacionEntranteAsync` retorna el error y no invoca el repositorio de registro final
