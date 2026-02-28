# jira-scrum-27 Specification

## Purpose
TBD - created by archiving change scrum-27-actualiza-daper-diasvencimiento. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-27.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-27

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Alias obligatorio en consulta de dias vencidos
La funcion `SolicitaTotalDiasVencimientoTramite` MUST recibir y validar `defaultDbAlias` en repository y service.

#### Scenario: Alias valido
- **WHEN** se recibe `defaultDbAlias` con valor valido
- **THEN** la consulta usa `QueryOptions.DefaultAlias = defaultDbAlias` y retorna resultado normal

#### Scenario: Alias nulo o vacio
- **WHEN** `defaultDbAlias` es nulo, vacio o espacios
- **THEN** retorna `success=false`, `message=\"DefaultDbAlias requerido\"`, `data=0` y `errors` con `Field=\"defaultDbAlias\"`

### Requirement: Cobertura de pruebas para alias
El sistema MUST incluir pruebas unitarias e integracion que validen el uso de `defaultDbAlias`.

#### Scenario: Unit tests de validacion de alias
- **WHEN** se ejecutan pruebas unitarias de service y repository
- **THEN** se valida el error controlado para alias invalido y el flujo exitoso existente

#### Scenario: Integration test con alias
- **WHEN** se ejecuta la prueba de integracion con Testcontainers/MySQL
- **THEN** la consulta funciona con alias valido y devuelve el numero correcto de dias vencidos

