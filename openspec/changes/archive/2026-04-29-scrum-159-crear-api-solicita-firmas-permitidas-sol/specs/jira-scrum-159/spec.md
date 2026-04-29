## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-159.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-159

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: API de firmas permitidas por solicitud
The API MUST expose `GET /api/gestion-correspondencia/firmas/permitidas-por-solicitud` and return `AppResponses<List<ResponseDropdownDto>>`.

#### Scenario: Claims obligatorios inválidos
- **WHEN** `defaulalias` or `usuarioid` claim is missing/invalid
- **THEN** controller returns `BadRequest` with validation response

#### Scenario: Parámetro inválido
- **WHEN** `idSolicitudAprobacion <= 0`
- **THEN** controller returns `BadRequest` with validation error

#### Scenario: Lista con datos
- **WHEN** service resolves authorized signers for the request
- **THEN** response returns `success=true`, non-empty `data`, `meta.status="success"`

### Requirement: Filtro estricto de autorizaciones
Repository query MUST include `ESTADO_AUTORIZACION_FIRMA = 1` and requested `idSolicitudAprobacion`.

#### Scenario: Sin firmantes autorizados
- **WHEN** there are no rows matching authorization filter
- **THEN** service returns `success=true`, `data=[]`, `meta.status="empty"`

### Requirement: Descripción defensiva y deduplicación
Service MUST build dropdown description defensively and deduplicate records by `Id`.

#### Scenario: Nombre o cargo nulos
- **WHEN** source fields have null/empty values
- **THEN** description uses `"Sin nombre"` fallback and omits empty cargo

#### Scenario: Duplicados en origen
- **WHEN** repository returns repeated signers
- **THEN** service returns unique elements by `Id`

### Requirement: Integración DI y pruebas
The implementation MUST register service/repository interfaces in `DocuArchi.Api/Program.cs` and include automated tests for controller and service.

#### Scenario: Registro de dependencias
- **WHEN** app starts
- **THEN** `IServiceSolicitaListaFirmasPermitidasSolicitudAprobacion` and `ISolicitaListaFirmasPermitidasSolicitudAprobacionRepository` are resolvable

#### Scenario: Evidencia de pruebas
- **WHEN** focused automated tests run
- **THEN** they validate claim failures, invalid input, empty response, error response, deduplication and success mapping
