# jira-scrum-94 Specification

## Purpose
TBD - created by archiving change scrum-94-actualizacion-registrarradicacionentrant. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-94.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-94

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarRadicacionEntranteAsync usa tipo de envio para actividad relacionada
`RegistrarRadicacionEntranteAsync` MUST decidir la validacion de actividad workflow relacionada con base en `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` y no solo en `requestCanonico.tipoModuloRadicacion`.

#### Scenario: Tipo de envio requiere actividad relacionada aun en modulo workflow
- **WHEN** `requestCanonico.tipoModuloRadicacion` es `2`
- **AND** `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` es `2` o `3`
- **THEN** el servicio consulta `UsuarioWorkflow`
- **AND** consulta `SolicitaIdActividadRelacionadaGrupo`
- **AND** solo continua con `_registrarRepository.RegistrarRadicacionEntranteAsync` si existe una actividad relacionada valida

#### Scenario: Workflow sin tipo de envio relacionado conserva compatibilidad
- **WHEN** `requestCanonico.tipoModuloRadicacion` es `2`
- **AND** `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` no es `2` ni `3`
- **THEN** el servicio conserva el flujo workflow actual sin consultar `UsuarioWorkflow` interno

