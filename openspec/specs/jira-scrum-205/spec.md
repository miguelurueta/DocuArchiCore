# jira-scrum-205 Specification

## Purpose
TBD - created by archiving change scrum-205-crea-api-lista-documentos-radicados. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-205.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-205

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Query endpoint for ListaDocumentosRadicados
The system MUST expose `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query` and return data using `AppResponses<object>`.

#### Scenario: Query with IncludeConfig=true
- **GIVEN** a valid request with `IncludeConfig=true` and valid claims
- **WHEN** query endpoint is executed
- **THEN** response `data` contains table-level contract (`DynamicUiTableDto`) with rows and metadata

#### Scenario: Query with IncludeConfig=false
- **GIVEN** a valid request with `IncludeConfig=false`
- **WHEN** query endpoint is executed
- **THEN** response `data` contains rows-only contract (`DynamicUiRowsOnlyDto`)

### Requirement: ViewMode behavior consistency
The system MUST support both `hierarchical` and `flatDocuments` modes with deterministic behavior.

#### Scenario: Hierarchical mode expansion
- **GIVEN** `ViewMode=hierarchical` and a valid `ParentRowId`
- **WHEN** query endpoint is executed
- **THEN** response includes the node children for the requested level

#### Scenario: FlatDocuments mode normalization
- **GIVEN** `ViewMode=flatDocuments`
- **WHEN** query endpoint is executed
- **THEN** every row is returned as document node (`NodeType=documento`, `ParentId=null`, `HasChildren=false`)

### Requirement: Legacy migration for related documents list
The system MUST migrate legacy related-document behavior to the new contract without bootstrap-table semantics.

#### Scenario: Legacy fields are preserved in normalized row
- **GIVEN** legacy source row with fields `ID`, `DBT`, `PAG`, `TIPODOCUMENTO`, `ESTADO_FIRMA_DIGITAL`
- **WHEN** mapping to dynamic row contract is executed
- **THEN** row `Values`/`Meta` preserves those values in the new normalized structure

### Requirement: Action endpoint with controlled dispatch
The system MUST expose `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action` and dispatch supported actions.

#### Scenario: Action ver_documento
- **GIVEN** a valid action request with `ActionId=ver_documento`
- **WHEN** action endpoint is executed
- **THEN** response contains `Operation=view` and `DocumentResolveRequest` for `/api/gestor-documental/documentos/visualizacion/resolve`

#### Scenario: Action agregar_item
- **GIVEN** a valid action request with `ActionId=agregar_item`
- **WHEN** action endpoint is executed
- **THEN** response contains `Operation=added` and affected row metadata

#### Scenario: Action eliminar_item
- **GIVEN** a valid action request with `ActionId=eliminar_item`
- **WHEN** action endpoint is executed
- **THEN** response contains `Operation=deleted` and affected row metadata

#### Scenario: Unsupported action
- **GIVEN** an unknown `ActionId`
- **WHEN** action endpoint is executed
- **THEN** system returns `success=false` with controlled error details

### Requirement: Mandatory claims and controlled failures
The system MUST validate `defaulalias` and `usuarioid` claims before processing query/action.

#### Scenario: Missing defaulalias claim
- **GIVEN** request without `defaulalias`
- **WHEN** endpoint is executed
- **THEN** system returns validation error (`BadRequest`) with controlled `AppResponses`

#### Scenario: Invalid usuarioid
- **GIVEN** request with non-numeric or invalid `usuarioid`
- **WHEN** endpoint is executed
- **THEN** system returns controlled security/validation failure

### Requirement: Data access policy enforcement
The implementation MUST use `DapperCrudEngine` with `QueryOptions` and avoid manual SQL execution patterns.

#### Scenario: Repository implementation review
- **WHEN** repository code is reviewed
- **THEN** data access is implemented through `DapperCrudEngine`/`QueryOptions` and parameterized criteria

### Requirement: Compatibility with existing AppTable endpoints
The implementation MUST not regress current workflow inbox/app table behavior.

#### Scenario: Regression validation
- **WHEN** backend regression tests are executed
- **THEN** existing endpoints `/api/workflowInboxgestion/inboxgestion`, `/api/workflowInboxgestion/inboxgestion/autocomplete`, and `/api/AppTable/export` keep their current behavior

