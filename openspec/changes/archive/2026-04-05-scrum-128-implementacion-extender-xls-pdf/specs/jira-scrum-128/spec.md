## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-128.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-128

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox export format expansion
The system MUST extend the existing `allMatching` workflow inbox export endpoint to support `csv`, `xlsx` and `pdf` without creating a parallel query path.

#### Scenario: Export endpoint receives a valid XLSX request
- **WHEN** the client sends `POST /api/AppTable/export` with `Format = "xlsx"` and `ExportMode = "allMatching"`
- **THEN** the backend returns a valid XLSX file response
- **AND** it reuses the same dataset semantics already implemented for CSV

#### Scenario: Export endpoint receives a valid PDF request
- **WHEN** the client sends `POST /api/AppTable/export` with `Format = "pdf"` and `ExportMode = "allMatching"`
- **THEN** the backend returns a valid PDF file response
- **AND** it reuses the same dataset semantics already implemented for CSV

#### Scenario: Export request uses unsupported format
- **WHEN** the client sends a format different from `csv`, `xlsx` or `pdf`
- **THEN** the backend returns a controlled validation error

### Requirement: Export query must keep workflow inbox semantics
The system MUST reuse the same workflow inbox filtering, sorting, security and visible-column metadata rules already used by the existing inbox pipeline.

#### Scenario: Export ignores frontend pagination but keeps active query state
- **WHEN** the export request is transformed into the internal workflow inbox request
- **THEN** it preserves `ColumnMode`, `EstadoTramite`, `SearchType`, `Search`, `SortField`, `SortDir` and `StructuredFilters`
- **AND** `Page` and `PageSize` do not change the exported dataset

#### Scenario: Export keeps workflow security scope
- **WHEN** an authenticated user exports the inbox in any supported format
- **THEN** the backend resolves claims and workflow context with the existing services
- **AND** it does not export rows outside the user visibility already enforced by the inbox query

### Requirement: Exported columns and data must remain consistent across formats
The system MUST use the same visible metadata columns and the same filtered dataset for `csv`, `xlsx` and `pdf`.

#### Scenario: Same rows and columns are reused for every format
- **WHEN** the backend resolves route column metadata and executes the export query
- **THEN** the generated CSV, XLSX and PDF use the same standard workflow columns and visible dynamic columns
- **AND** no format is allowed to add or omit rows compared to the same query state

#### Scenario: Client attempts to define arbitrary columns
- **WHEN** the client sends unsupported field names or tries to control exported columns directly
- **THEN** the backend ignores or rejects them through the existing controlled metadata pipeline

### Requirement: XLSX and PDF exports must include branding and report metadata
The system MUST embed corporate branding and report metadata in XLSX and PDF outputs while keeping CSV backward compatible.

#### Scenario: XLSX includes executive header and metadata
- **WHEN** the backend generates an XLSX export
- **THEN** the file includes an executive header with embedded corporate logo
- **AND** it shows report metadata including title, module, generation timestamp, user, applied filters, applied sort and total records

#### Scenario: PDF includes executive header and metadata
- **WHEN** the backend generates a PDF export
- **THEN** the file includes an executive header with embedded corporate logo
- **AND** it shows report metadata including title, module, generation timestamp, user, applied filters, applied sort and total records

#### Scenario: CSV remains backward compatible
- **WHEN** the backend generates a CSV export
- **THEN** it does not require embedded logo support
- **AND** it keeps the current CSV contract compatible for existing consumers

### Requirement: Export files must use controlled file contracts
The system MUST return deterministic file contracts with the content type and extension matching the requested format.

#### Scenario: CSV file contract is returned
- **WHEN** the CSV export completes successfully
- **THEN** the response content type is `text/csv`
- **AND** the file name follows `{modulo}_{yyyyMMdd_HHmmss}.csv`

#### Scenario: XLSX file contract is returned
- **WHEN** the XLSX export completes successfully
- **THEN** the response content type is `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- **AND** the file name follows `{modulo}_{yyyyMMdd_HHmmss}.xlsx`

#### Scenario: PDF file contract is returned
- **WHEN** the PDF export completes successfully
- **THEN** the response content type is `application/pdf`
- **AND** the file name follows `{modulo}_{yyyyMMdd_HHmmss}.pdf`

### Requirement: Export execution must remain bounded
The system MUST apply the existing controlled export threshold before materializing `csv`, `xlsx` or `pdf`.

#### Scenario: Matching rows stay within the configured threshold
- **WHEN** the filtered total is less than or equal to the export limit
- **THEN** the backend generates the requested file successfully

#### Scenario: Matching rows exceed the configured threshold
- **WHEN** the filtered total is greater than the export limit
- **THEN** the backend returns a controlled functional error instead of loading an unbounded dataset

### Requirement: Empty state handling must be valid for every supported format
The system MUST generate a valid empty export artifact when the current filters do not return rows.

#### Scenario: CSV has no matching rows
- **WHEN** the CSV export produces zero rows
- **THEN** the backend still returns a valid CSV file with headers and zero data rows

#### Scenario: XLSX or PDF has no matching rows
- **WHEN** the XLSX or PDF export produces zero rows
- **THEN** the backend still returns a valid file for the requested format
- **AND** the report metadata remains visible even with an empty dataset

### Requirement: Export expansion must be covered by automated tests
The system MUST include automated tests for controller, service and export builders covering the new formats and metadata rules.

#### Scenario: Tests cover supported formats and controlled failures
- **WHEN** automated tests run
- **THEN** they verify valid CSV, XLSX and PDF generation
- **AND** they verify invalid format rejection, threshold exceeded and controlled error propagation

#### Scenario: Tests cover branding and semantic consistency
- **WHEN** automated tests run
- **THEN** they verify branding metadata is present in XLSX and PDF
- **AND** they verify the export reuses the same filters, sort and visible-column metadata already used by the workflow inbox query
