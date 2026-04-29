# jira-scrum-160 Specification

## Purpose
TBD - created by archiving change scrum-160-crea-api-lista-firmas-autorizadas. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-160.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-160

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API de firmas autorizadas por usuario
The system MUST expose `GET /api/gestion-correspondencia/firmas/autorizadas-documento` returning `AppResponses<List<ResponseDropdownDto>>`.

#### Scenario: Request válida con resultados
- **WHEN** `idUsuarioAutorizado > 0`, claims válidos y el usuario está autorizado
- **THEN** el endpoint responde `success=true`, `meta.status=success` y lista deduplicada de máximo 100 elementos

#### Scenario: Request válida sin resultados
- **WHEN** la consulta no encuentra firmantes
- **THEN** el endpoint responde `success=true`, `meta.status=empty`, `data=[]`, `message=Sin resultados`

### Requirement: Autorización y validación de claims
The system MUST validate `defaulalias` and `usuarioid` claims and enforce that `usuarioid == idUsuarioAutorizado`.

#### Scenario: Usuario no autorizado
- **WHEN** `usuarioid` es distinto a `idUsuarioAutorizado`
- **THEN** el servicio retorna `success=false`, `meta.status=error`, `message=No autorizado`

### Requirement: Query segura y acotada
The repository MUST execute query with explicit joins, filters and `Limit=100`.

#### Scenario: Dataset grande o duplicado
- **WHEN** existen más de 100 registros o IDs repetidos
- **THEN** el resultado final se deduplica por `Id` y se trunca a 100

