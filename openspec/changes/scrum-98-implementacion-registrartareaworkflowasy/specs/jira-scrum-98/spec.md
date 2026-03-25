## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-98.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-98

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarRadicacionEntranteAsync registra tarea workflow faltante
`RegistrarRadicacionEntranteAsync` MUST consumir `RegistrarTareaWorkflowAsync` despues de consultar existencia workflow cuando no exista una tarea previa para la ruta.

#### Scenario: No existe tarea workflow previa
- **GIVEN** el registro termina con `success=true` y `data` valida
- **AND** `ConsultarExistenciaRadicadoRutaWorkflowAsync` retorna `success=true` con `IdTareaWorkflow = 0`
- **AND** existen `RutaWorkflow`, `UsuarioWorkflow`, `GruposWorkflow` y `ActividadInicioFlujo` validos
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** el servicio ejecuta `RegistrarTareaWorkflowAsync`
- **AND** usa el resultado solo como dato interno del flujo

#### Scenario: Ya existe tarea workflow previa
- **GIVEN** el registro termina con `success=true` y `data` valida
- **AND** `ConsultarExistenciaRadicadoRutaWorkflowAsync` retorna `success=true` con `IdTareaWorkflow != 0`
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** el servicio no ejecuta `RegistrarTareaWorkflowAsync`
- **AND** continua con la respuesta del registro

#### Scenario: Registro de tarea workflow falla
- **GIVEN** el registro termina con `success=true` y `data` valida
- **AND** `ConsultarExistenciaRadicadoRutaWorkflowAsync` retorna `success=true` con `IdTareaWorkflow = 0`
- **AND** `RegistrarTareaWorkflowAsync` retorna `success=false` o un resultado invalido
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** el servicio interrumpe el flujo con error controlado
