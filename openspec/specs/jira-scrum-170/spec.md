# jira-scrum-170 Specification

## Purpose
TBD - created by archiving change scrum-170-crea-funcion-manager-archivo-compensacio. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-170.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-170

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Secure path resolution for physical storage
The system MUST resolve temporary and final filesystem paths with path-traversal protection and strict segment validation.

#### Scenario: Valid storage route segments
- **WHEN** gabinete, ruta temporal and archivo temporal identifiers are valid
- **THEN** the resolver builds normalized full paths under approved storage roots
- **AND** final path includes deterministic folder segments by disk and folder identifiers

#### Scenario: Path traversal attempt
- **WHEN** identifiers include `..`, path separators or invalid characters
- **THEN** the resolver rejects the operation with controlled physical error

### Requirement: Atomic file copy outside DB transaction
The system MUST copy files with atomic move semantics and MUST avoid overwrite of existing target files.

#### Scenario: Successful physical copy
- **WHEN** source files exist and destination files do not exist
- **THEN** each file is copied to a temporary `.tmp` artifact and then moved atomically to final location

#### Scenario: Source or destination conflict
- **WHEN** source file is missing or destination already exists
- **THEN** physical phase fails with controlled error and triggers compensation

### Requirement: XML generation for stored document
The system MUST generate an XML metadata artifact aligned with storage transaction data.

#### Scenario: Successful XML generation
- **WHEN** physical phase receives valid `StorageXmlModel`
- **THEN** it writes XML using safe writer settings and atomic finalization
- **AND** includes identity and metadata fields needed for traceability

### Requirement: Compensation manager on physical failure
The system MUST execute compensation when a physical step fails after DB commit.

#### Scenario: Failure during copy or XML write
- **WHEN** any physical step throws
- **THEN** compensation removes generated files and temporary artifacts
- **AND** removes empty created directories when possible

### Requirement: Orchestrator integration of physical phase
The storage orchestrator MUST execute validation -> DB transaction phase -> physical phase in order.

#### Scenario: End-to-end success
- **WHEN** validation, transaction and physical phases succeed
- **THEN** response state is `Completed` and includes `NombreArchivoFinal`

#### Scenario: Physical phase failure after transaction success
- **WHEN** transaction commits but physical phase fails
- **THEN** orchestrator raises `StoragePhysicalException`
- **AND** reports `PhysicalFailed` behavior through controlled exception path

### Requirement: Testability of physical phase
The system MUST provide unit-level evidence for path hardening, physical success flow and compensation on failure.

#### Scenario: Unit tests for path hardening
- **WHEN** path resolver receives malicious path inputs
- **THEN** tests assert rejection with controlled physical errors

#### Scenario: Unit tests for compensation
- **WHEN** physical writer simulation fails
- **THEN** tests assert compensation manager invocation

