# jira-scrum-61 Specification

## Purpose
TBD - created by archiving change scrum-61-migraci-n-net-solicita-campos-relacion-r. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-61.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-61

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Migrar consulta de campos relacionados plantilla-ruta
El sistema MUST exponer una consulta para `solicita_campos_relacion_ruta_plantilla` usando arquitectura Controller -> Service -> Repository con `defaultDbAlias`.

#### Scenario: Consulta exitosa
- **GIVEN** `idPlantillaRadicado` y `idRuta` validos
- **WHEN** se invoca el endpoint de consulta
- **THEN** retorna `AppResponses<List<RelacionCamposRutaWorklflowDto>>` con `success=true` y `message=OK`

#### Scenario: Sin resultados
- **GIVEN** no existen filas para la plantilla y ruta solicitadas
- **WHEN** se ejecuta la consulta
- **THEN** retorna `success=true`, `message=\"Sin resultados\"` y `data=[]`

#### Scenario: Error controlado
- **GIVEN** ocurre una excepcion en el repositorio
- **WHEN** se ejecuta la consulta
- **THEN** retorna `success=false` y errores controlados en `AppResponses`

