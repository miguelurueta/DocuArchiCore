## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-152.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-152

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Map legacy DB columns deterministically (aliasing)
The `SolicitaEstructuraRespuestaIdTareaAsync` repository query MUST map legacy database columns to model/DTO properties using explicit SQL aliases, to avoid returning `null/0` values when the row exists in DB.

#### Scenario: Repository maps `ID_TAREA_WF` reliably
- **GIVEN** `ra_respuesta_radicado.ID_TAREA_WF = X` exists
- **WHEN** `SolicitaEstructuraRespuestaIdTareaAsync(X, defaultDbAlias)` is executed
- **THEN** every returned row has `IdTareaWf == X`

#### Scenario: Repository maps token-dependent fields reliably
- **GIVEN** `ra_respuesta_radicado` row contains values for token fields (e.g., `RADICADO`, `DESTINATARIO`, `DIRECCION_DESTINATARIO`, `ASUNTO`, `USUARIO_RESPONSABLE`, `CARGO_RESPONSABLE`, `AREA_RESPONSABLE`)
- **WHEN** `SolicitaEstructuraRespuestaIdTareaAsync(X, defaultDbAlias)` is executed
- **THEN** the returned model/DTO exposes those values in the expected properties (non-empty when present in DB)

### Requirement: Keep contract and response semantics unchanged
The public contract and `AppResponses` semantics of `SolicitaEstructuraRespuestaIdTareaAsync` MUST remain unchanged.

#### Scenario: No results behavior remains stable
- **WHEN** no row exists for the given `idTareaWf`
- **THEN** response is `success=true`, `message="Sin resultados"`, and `data=[]`

### Requirement: Prevent regressions with tests
The change MUST include a regression test (contract/integration when possible) that fails if aliases are removed or `IdTareaWf` is not populated.
