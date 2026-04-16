## ADDED Requirements

### Requirement: Endpoint `SolicitaEstructuraRespuestaIdTarea`
The system MUST expose a GET endpoint to query `ra_respuesta_radicado` by `ID_TAREA_WF` through a service layer.

#### Scenario: Missing `defaulalias` claim
- **GIVEN** the authenticated request does not include claim `defaulalias`
- **WHEN** `GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf=123` is called
- **THEN** it returns `400 BadRequest` with `validation.Response`

#### Scenario: Invalid `idTareaWf`
- **GIVEN** a valid `defaulalias` claim
- **WHEN** `idTareaWf <= 0`
- **THEN** it returns `400 BadRequest` with an `AppResponses<List<RaRespuestaRadicado>>` validation error

#### Scenario: Successful query with results
- **GIVEN** a valid `defaulalias` claim and `idTareaWf > 0`
- **WHEN** the repository returns rows
- **THEN** it returns `200 OK` with `AppResponses<List<RaRespuestaRadicado>>` and `success = true`

#### Scenario: Successful query without results
- **GIVEN** a valid `defaulalias` claim and `idTareaWf > 0`
- **WHEN** the repository returns no rows
- **THEN** it returns `200 OK` with `success = true`, `data = []`, and `message = "Sin resultados"`

#### Scenario: Controlled error from service/repository
- **GIVEN** a valid `defaulalias` claim and `idTareaWf > 0`
- **WHEN** the service/repository returns a controlled error (`success = false`)
- **THEN** it returns `400 BadRequest` with the same `AppResponses` error payload

### Requirement: Frontend integration documentation
The system MUST include a frontend integration guide for SCRUM-137 documenting the endpoint, parameters, required claim, and response semantics.

#### Scenario: Frontend guide exists
- **WHEN** a frontend developer reviews documentation
- **THEN** a document `Docs/GestionCorrespondencia/SCRUM-137-Integracion-Frontend.md` exists and includes request/response examples and empty/error state guidance

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-137.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-137

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

Reference: `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements
