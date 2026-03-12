## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-59.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-59

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarRadicacionEntrante uses configured module value
RegistrarRadicacionEntranteAsync MUST obtain module registration value from SolicitaConfiguracionPlantillaAsync and MUST NOT use hardcoded value "RADICACION".

#### Scenario: Configuracion encontrada
- **GIVEN** la configuración de plantilla existe para `idPlantilla` y `tipoRadicacionPlantilla`
- **WHEN** RegistrarRadicacionEntranteAsync continúa el flujo de persistencia
- **THEN** envía `Data.Descripcion_tipo_radicacion` como parámetro `moduloRegistro` a `_registrarRepository.RegistrarRadicacionEntranteAsync`

### Requirement: RegistrarRadicacionEntrante handles missing configuration
RegistrarRadicacionEntranteAsync MUST stop the process with controlled response when configuration query returns success=true and data=null.

#### Scenario: Configuracion sin resultados
- **GIVEN** SolicitaConfiguracionPlantillaAsync retorna `success=true` y `data=null`
- **WHEN** se intenta registrar la radicación
- **THEN** la respuesta es controlada de negocio indicando que no existe configuración para la plantilla/tipo
- **AND** no se invoca `_registrarRepository.RegistrarRadicacionEntranteAsync`

### Requirement: RegistrarRadicacionEntrante handles configuration errors
RegistrarRadicacionEntranteAsync MUST stop the process when configuration query returns success=false.

#### Scenario: Configuracion con error
- **GIVEN** SolicitaConfiguracionPlantillaAsync retorna `success=false`
- **WHEN** se intenta registrar la radicación
- **THEN** retorna error controlado propagando mensaje/errores de la consulta
- **AND** no se invoca `_registrarRepository.RegistrarRadicacionEntranteAsync`
