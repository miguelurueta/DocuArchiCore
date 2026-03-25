# jira-scrum-96 Specification

## Purpose
TBD - created by archiving change scrum-96-actualizacion-registrarradicacionentrant. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-96.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-96

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: RegistrarRadicacionEntranteAsync scopes existencia workflow response outside the success condition
`RegistrarRadicacionEntranteAsync` MUST resolve `existenciaWorkflowResult` outside the `if (registroResult.success && registroResult.data != null)` block while preserving current success/error behavior.

#### Scenario: Registro exitoso con existencia workflow valida
- **GIVEN** el registro retorna `success=true` con `data` valida
- **AND** la consulta `ConsultarExistenciaRadicadoRutaWorkflowAsync` retorna `success=true`
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** la respuesta final conserva `registroResult`

#### Scenario: Registro exitoso con error en existencia workflow
- **GIVEN** el registro retorna `success=true` con `data` valida
- **AND** la consulta `ConsultarExistenciaRadicadoRutaWorkflowAsync` retorna `success=false`
- **WHEN** se ejecuta `RegistrarRadicacionEntranteAsync`
- **THEN** la respuesta final retorna el error controlado de existencia workflow

