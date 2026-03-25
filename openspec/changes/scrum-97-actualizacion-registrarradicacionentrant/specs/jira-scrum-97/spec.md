## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-97.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-97

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarRadicacionEntranteAsync tolerates successful existencia workflow responses without data
`RegistrarRadicacionEntranteAsync` MUST continue when `ConsultarExistenciaRadicadoRutaWorkflowAsync` returns `success=true` without payload data.

#### Scenario: Existencia workflow returns success without data
- **GIVEN** el registro termina con `success=true` y `data` valida
- **AND** `ConsultarExistenciaRadicadoRutaWorkflowAsync` retorna `success=true` con `data=null`
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** el servicio continua y retorna `registroResult`

#### Scenario: Existencia workflow result remains available after registro validation
- **GIVEN** el registro termina con `success=true` y `data` valida
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** la variable `existenciaWorkflowResult` queda declarada fuera del bloque que valida `registroResult`
- **AND** el resultado puede reutilizarse posteriormente en el metodo
- **AND** no se utiliza una inicializacion nula para controlar el flujo

#### Scenario: Existencia workflow returns technical error
- **GIVEN** el registro termina con `success=true` y `data` valida
- **AND** `ConsultarExistenciaRadicadoRutaWorkflowAsync` retorna `success=false`
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** el servicio retorna el error controlado de existencia workflow
