## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-155.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-155

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Canonical API route remains stable
The system MUST keep the canonical endpoint route as implemented by the current controller for `SolicitaEstructuraRespuestaIdTarea`.

#### Scenario: Canonical route is used
- **WHEN** the consumer calls the endpoint
- **THEN** it calls `GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf={idTareaWf}`
- **AND** it is authorized (requires authentication) and validates the `defaulalias` claim

#### Scenario: Compatibility with previously documented routes
- **GIVEN** there exists legacy documentation or clients referencing `/api/gestor-documental/solicita-estructura-respuesta-id-tarea`
- **WHEN** the change is implemented
- **THEN** no existing route is removed as part of this ticket
- **AND** the system either preserves any existing alias route (if present) or updates the consuming docs/clients to the canonical route

### Requirement: Additive contract for empty/pending/success (non-breaking)
The endpoint response MUST remain in the standard `AppResponses<List<RaRespuestaRadicado>>` shape and MUST add a canonical status under `meta.status` to disambiguate `empty` vs `pending` vs `success`.

Contract notes:
- This is an additive (non-breaking) change: existing fields (`success`, `message`, `data`, `meta`, `errors`) are preserved.
- Consumers MUST NOT use `message` for decision-making; `meta.status` is canonical when present.
- If `meta.status` is missing (older backend), the consumer MUST fall back to legacy semantics.

Allowed statuses:
- `success`: data exists and is usable (`data.length > 0`)
- `empty`: definitive absence of data
- `pending`: data may appear later, only when the backend has an objective signal of processing/eventual consistency

#### Scenario: Response with data
- **WHEN** `idTareaWf` has results
- **THEN** response is `success=true`
- **AND** `data` contains elements
- **AND** `meta.status="success"`

#### Scenario: Definitive empty response
- **WHEN** `idTareaWf` has no results and there is no objective signal that data will appear later
- **THEN** response is `success=true`
- **AND** `data=[]`
- **AND** `meta.status="empty"`
- **AND** it MUST NOT be possible for the same real-world condition to later return data without an explicit, objective transition path

#### Scenario: Pending response (objective signal required)
- **GIVEN** the backend can determine an objective “still processing / eventual consistency” signal for the case
- **WHEN** `idTareaWf` currently has no rows available
- **THEN** response is `success=true`
- **AND** `data=[]`
- **AND** `meta.status="pending"`
- **AND** the backend MUST NOT use `"Sin resultados"` as a definitive message in this state

#### Scenario: Legacy fallback when meta.status is absent
- **GIVEN** the backend does not send `meta.status`
- **WHEN** the consumer receives `success=true` and `data=[]`
- **THEN** the consumer treats it as legacy empty (`empty`) to preserve backwards compatibility

### Requirement: Consistency across repeated calls
For the same `idTareaWf` in short intervals, the backend MUST NOT alternate between “definitive empty” and “success with data” without an explicit, objective explanation.

#### Scenario: No empty→success flicker without pending/signal
- **WHEN** a consumer performs repeated calls for the same `idTareaWf`
- **THEN** the backend does not return `meta.status="empty"` and later `meta.status="success"` unless there is a documented, objective signal/transition that justifies it (or the underlying data truly changed)

### Requirement: Observability to diagnose inconsistency
The backend MUST emit minimal diagnostic logs per request to allow troubleshooting of the inconsistent behavior.

#### Scenario: Logs include correlation and origin details
- **WHEN** the endpoint is invoked
- **THEN** logs include at least: `idTareaWf`, resolved `defaulalias`, resolved database/schema (e.g. `SELECT DATABASE()`), row count, duration, request/correlation id, and resulting `meta.status`

### Requirement: Include TramiteDocumento when it exists in DB
If the backing table contains `TRAMITE_DOCUMENTO` and it must be exposed, the backend MUST select and map it explicitly to `TramiteDocumento` in the returned model.

#### Scenario: TramiteDocumento is returned when present
- **GIVEN** the row has `TRAMITE_DOCUMENTO` populated
- **WHEN** the endpoint is called
- **THEN** the response data includes `TramiteDocumento` with the persisted value

### Requirement: Frontend rendering rules (anti-flicker)
The frontend MUST render based on `meta.status` when present and must not block definitively on a non-definitive state.

#### Scenario: Success renders details
- **WHEN** `meta.status="success"`
- **THEN** the UI renders `GestionRespuesta` master-detail without showing “bloqueado/sin resultados” first

#### Scenario: Empty blocks definitively (no later details)
- **WHEN** `meta.status="empty"`
- **THEN** the UI shows the definitive blocked state
- **AND** it MUST NOT later render details without user action and without a contractually different backend response

#### Scenario: Pending shows skeleton (no definitive block)
- **WHEN** `meta.status="pending"`
- **THEN** the UI shows a stable loading/skeleton state
- **AND** it MUST NOT show the definitive “sin resultados/bloqueado” state
