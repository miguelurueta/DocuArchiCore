## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-26.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-26

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Consulta de dias feriados activos
El sistema MUST exponer la funcion `SolicitaListaDiasFeriados` bajo el patron Controller -> Service -> Repository para consultar `rea_001_feriados` usando `IDapperCrudEngine`.

#### Scenario: Consulta exitosa con datos
- **WHEN** existen registros con `ESTADO_DIA = 1`
- **THEN** la respuesta retorna `success=true`, `message="OK"` y una lista de fechas con formato `yyyy-MM-dd`

#### Scenario: Consulta sin resultados
- **WHEN** no existen registros activos en `rea_001_feriados`
- **THEN** la respuesta retorna `success=true`, `message="Sin resultados"` y lista vacia

#### Scenario: Excepcion controlada
- **WHEN** ocurre una excepcion en repository o service
- **THEN** la respuesta retorna `success=false`, `data` vacio y `errors` con `Field="IdTipoTramite"`

### Requirement: Registro en DI y endpoint API
El sistema MUST registrar las interfaces y clases nuevas en `DocuArchi.Api/Program.cs` y exponer endpoint HTTP en `TramiteController`.

#### Scenario: Resolucion por contenedor de dependencias
- **WHEN** inicia `DocuArchi.Api`
- **THEN** `IListaDiasFeriadosTramiteRepository` e `IListaDiasFeriadosTramiteService` se resuelven correctamente

### Requirement: Cobertura de pruebas unitarias e integracion
El sistema MUST incluir pruebas unitarias y de integracion para la consulta de dias feriados.

#### Scenario: Unit tests de flujo de servicio
- **WHEN** se ejecutan pruebas unitarias de servicio
- **THEN** se validan los casos de exito, sin resultados y excepcion simulada

#### Scenario: Integration test con MySQL/Testcontainers
- **WHEN** se ejecuta la prueba de integracion
- **THEN** la consulta filtra `ESTADO_DIA = 1` y retorna fechas correctas en formato `yyyy-MM-dd`
