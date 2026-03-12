## ADDED Requirements

### Requirement: Consulta configuracion plantilla radicacion
The system MUST expose a plantilla configuration query filtered by `idPlantilla`, `tipoRadicacionPlantilla`, and `defaultDbAlias` using layered architecture.

#### Scenario: Query with valid data
- **WHEN** a consumer calls the endpoint with valid `idPlantilla`, `tipoRadicacionPlantilla`, and claim `defaulalias`
- **THEN** backend queries `ra_rad_config_plantilla_radicacion` with parameterized filters
- **AND** returns `AppResponses` with `success=true`, `message="OK"`, and matching `data`

#### Scenario: Query without rows
- **WHEN** no record matches the filters
- **THEN** backend returns `success=true`, `message="Sin resultados"`, and `data=null`

#### Scenario: Controlled technical error
- **WHEN** an exception occurs during repository/service/controller flow
- **THEN** backend returns `success=false` with descriptive `errors` and keeps `AppResponses` contract

### Requirement: Backend baseline OPSXJ rules
Backend updates MUST follow `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Pattern and DI compliance
- **WHEN** implementation is reviewed
- **THEN** Controller -> Service -> Repository separation exists
- **AND** interfaces and classes are registered in `Program.cs`
- **AND** flow uses `try/catch` and `AppResponses` in all layers
