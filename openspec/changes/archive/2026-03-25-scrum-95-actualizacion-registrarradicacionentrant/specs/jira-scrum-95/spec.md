## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-95.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-95

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarRadicacionEntranteAsync usa util_tipo_modulo_envio en todo el flujo workflow
`RegistrarRadicacionEntranteAsync` MUST reemplazar las condiciones basadas en `requestCanonico.tipoModuloRadicacion == 2` por la evaluacion de `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio == 2 || citaEstructuraTipoDoEntrante.util_tipo_modulo_envio == 3` en las validaciones workflow relacionadas.

#### Scenario: Tipo de envio workflow controla preregistro y existencia
- **WHEN** `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` es `2` o `3`
- **THEN** el servicio ejecuta preregistro workflow
- **AND** valida `RutaWorkflow`
- **AND** consulta existencia workflow despues del registro

#### Scenario: Tipo de envio no workflow evita prerregistro workflow
- **WHEN** `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` no es `2` ni `3`
- **THEN** el servicio omite preregistro workflow
- **AND** omite consulta de existencia workflow
- **AND** resuelve la ruta workflow interna segun el flujo no workflow

#### Scenario: Tipo de envio workflow exige actividad inicial
- **WHEN** `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` es `2` o `3`
- **AND** `requestCanonico.RE_flujo_trabajo.id_tipo_flujo_workflow > 0`
- **AND** no existe actividad inicial valida
- **THEN** el servicio retorna una validacion controlada
- **AND** no ejecuta el repositorio de registro
