## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-93.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-93

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Registro valida actividad workflow relacionada al grupo
`RegistrarRadicacionEntranteAsync` MUST consultar `SolicitaIdActividadRelacionadaGrupo` antes del registro cuando el tipo de envio requiera actividad workflow relacionada.

#### Scenario: Tipo modulo de envio requiere actividad workflow
- **WHEN** `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` es `2` o `3`
- **AND** `usuarioWorkflowInterno.Grupos_Workflow_Id_Grupo > 0`
- **THEN** el servicio consulta `SolicitaIdActividadRelacionadaGrupo`
- **AND** asigna el resultado a una variable local `GruposWorkflow`
- **AND** solo continua con `_registrarRepository.RegistrarRadicacionEntranteAsync` si `GruposWorkflow.id_Actividad > 0`

#### Scenario: Usuario workflow no tiene actividad relacionada
- **WHEN** `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` es `2` o `3`
- **AND** el repositorio `SolicitaIdActividadRelacionadaGrupo` retorna `null` o `id_Actividad <= 0`
- **THEN** `RegistrarRadicacionEntranteAsync` retorna un error controlado
- **AND** el repositorio de registro no se ejecuta
