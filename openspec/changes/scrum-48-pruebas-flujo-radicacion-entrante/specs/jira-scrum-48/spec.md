## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-48.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-48

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarEntrante con manejo de errores controlado
`POST /api/radicacion/registrar-entrante` MUST validar claims requeridos y manejar excepciones con respuesta estandarizada.

#### Scenario: Excepcion en flujo registrar entrante
- **WHEN** ocurre una excepcion durante `RegistrarEntrante`
- **THEN** el controller responde `500` con `AppResponses<RegistrarRadicacionEntranteResponseDto>`
- **AND** incluye detalle en `errors`

### Requirement: Campos obligatorios excluyen valores generados en backend
La validacion de campos obligatorios MUST excluir campos que el backend resuelve durante el registro de radicacion.

#### Scenario: Request sin campos backend-generados
- **WHEN** el request no incluye `Usuario_Radicador_id_usuario`, `Codigo_Sede`, `Id_area_remit_dest_interno`, `Area_remit_dest_interno`, `CARGO_DESTINATARIO`, `Consecutivo_Rad`, `Consecutivo_CodBarra` y `Fecha_Radicado`
- **THEN** la validacion de obligatorios no falla por ausencia de esos campos

### Requirement: Pruebas de concurrencia y documentacion tecnica
El sistema MUST incluir pruebas automatizadas de concurrencia y documentacion tecnica del flujo registrar-entrante.

#### Scenario: Carga concurrente controlada
- **WHEN** se ejecutan 50 solicitudes concurrentes sobre el flujo de registro simulado
- **THEN** no hay colisiones en el consecutivo devuelto
- **AND** se publica documentacion en `Docs/Radicacion/Tramite/PruebasConcurrencia.md` y `Docs/Radicacion/Tramite/PruebasRadicacion.md`
