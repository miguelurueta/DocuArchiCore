# jira-scrum-126 Specification

## Purpose
TBD - created by archiving change scrum-126-apptable-export-19-definir-estrategia-ba. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-126.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-126

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox export endpoint
The system MUST expose a backend endpoint that exports all matching workflow inbox rows to CSV without depending on frontend pagination.

#### Scenario: Export endpoint receives a valid allMatching request
- **WHEN** the client sends `POST /api/AppTable/export` with `Format = "csv"` and `ExportMode = "allMatching"`
- **THEN** the backend returns a CSV file response
- **AND** the dataset ignores `Page` and `PageSize`

#### Scenario: Export request uses unsupported format
- **WHEN** the client sends a format different from `csv`
- **THEN** the backend returns a controlled validation error

### Requirement: Export request contract
The system MUST use a dedicated DTO for export requests that preserves the existing workflow inbox query state.

#### Scenario: Export request preserves the active query state
- **WHEN** the export DTO is built from the API request
- **THEN** it includes `ColumnMode`, `EstadoTramite`, `SearchType`, `Search`, `SortField`, `SortDir` and `StructuredFilters`
- **AND** it also includes `Format`, `ExportMode` and optional `ReportTitle`

#### Scenario: Export contract does not expose internal security context
- **WHEN** the public export DTO is reviewed
- **THEN** it does not expose workflow internal fields resolved from claims or backend context

### Requirement: Export query must reuse workflow inbox semantics
The system MUST reuse the same workflow inbox filtering, sorting, security and column metadata rules already used by the existing inbox pipeline.

#### Scenario: Export query reuses controlled filters and sorting
- **WHEN** the export query is built
- **THEN** it uses the same base filters, advanced filters, `StructuredFilters` sanitization and safe sort resolution used by `WorkflowInboxQueryBuilder`

#### Scenario: Export query keeps workflow security scope
- **WHEN** an authenticated user exports the inbox
- **THEN** the backend resolves claims and workflow context with the existing services
- **AND** it does not export rows outside the user visibility already enforced by the inbox query

### Requirement: Exported columns must come from backend metadata
The system MUST export only the visible columns resolved from workflow metadata and MUST reject free-form client columns.

#### Scenario: Visible metadata columns are exported
- **WHEN** the backend resolves route column metadata for the active workflow route
- **THEN** the CSV headers are generated from the visible columns plus the standard workflow columns already present in the inbox query

#### Scenario: Client attempts to define arbitrary columns
- **WHEN** the export request includes arbitrary column definitions or unsupported field names
- **THEN** the backend ignores or rejects them through the existing controlled metadata pipeline

### Requirement: Export must enforce bounded execution
The system MUST apply a controlled maximum number of exported rows and return a controlled error when the threshold is exceeded.

#### Scenario: Matching rows stay within the configured threshold
- **WHEN** the filtered total is less than or equal to the export limit
- **THEN** the backend generates the CSV file successfully

#### Scenario: Matching rows exceed the configured threshold
- **WHEN** the filtered total is greater than the export limit
- **THEN** the backend returns a controlled functional error instead of loading an unbounded dataset

### Requirement: CSV response must be valid for empty and non-empty results
The system MUST return a valid CSV file with deterministic naming and header handling.

#### Scenario: Export returns matching rows
- **WHEN** the export completes successfully with data
- **THEN** the response content type is `text/csv`
- **AND** the file name follows `{modulo}_{yyyyMMdd_HHmmss}.csv`

#### Scenario: Export has no matching rows
- **WHEN** the filters produce zero rows
- **THEN** the backend still returns a valid CSV file
- **AND** the file includes headers and zero data rows

### Requirement: Export must be covered by automated tests
The system MUST include automated tests for controller, service, query builder and repository behavior around export.

#### Scenario: Tests cover active query compatibility
- **WHEN** automated tests run
- **THEN** they verify export consistency for `SearchType` 1, 2 and 3
- **AND** they verify `StructuredFilters`, empty state, threshold exceeded and controlled invalid filters

#### Scenario: Tests cover the export contract and file response
- **WHEN** controller and service tests run
- **THEN** they verify the CSV file contract, the controlled error path and the fact that `Page` and `PageSize` do not change the exported dataset

