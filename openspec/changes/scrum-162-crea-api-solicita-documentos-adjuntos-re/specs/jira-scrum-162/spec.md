## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-162.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-162

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API de adjuntos por idTareaWf
The system MUST expose an authorized endpoint to return attachment metadata for response records filtered by `idTareaWf`.

#### Scenario: Authorized request with records
- **WHEN** an authorized user calls `GET /api/GestionCorrespondencia/solicita-documentos-adjuntos-respuesta-radicado?idTareaWf={id}`
- **THEN** response is `AppResponses<List<DocumentoAdjuntoRespuestaRadicadoDto>>` with `success=true`, `meta.status=success`

#### Scenario: Authorized request without records
- **WHEN** query returns no rows
- **THEN** response is `success=true`, `meta.status=empty`, `data=[]`

#### Scenario: Invalid claim or invalid idTareaWf
- **WHEN** `defaulalias` claim is missing/invalid or `idTareaWf<=0`
- **THEN** endpoint returns `BadRequest` with validation error payload

### Requirement: Business rules for dedup and max cardinality
The service MUST deduplicate rows and cap output at 100 records.

#### Scenario: Repository returns duplicates and more than 100 rows
- **WHEN** rows repeat same `(IdRespuestaRadicado, TipoAdjunto, IdImagen)` and total unique > 100
- **THEN** service returns unique rows only and trims to first 100
