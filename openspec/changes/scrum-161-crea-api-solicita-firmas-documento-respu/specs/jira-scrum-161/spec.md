## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-161.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-161

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API orquestada de firmas documento respuesta
The system MUST expose `GET /api/gestion-correspondencia/firmas/documento-respuesta-orquestado` that returns `AppResponses<List<ResponseDropdownDto>>`.

#### Scenario: Consolidación exitosa
- **WHEN** claims son válidos y `idUsuarioGestion` es válido
- **THEN** combina usuario principal y firmas autorizadas en una sola respuesta deduplicada

#### Scenario: Sin resultados
- **WHEN** no hay usuario principal activo ni firmas autorizadas
- **THEN** responde `success=true`, `meta.status=empty`, `data=[]`
