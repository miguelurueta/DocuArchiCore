## ADDED Requirements

### Requirement: Temporary upload session initialization
The system MUST provide an API to initialize a temporary upload session for large files and return server-generated identifiers.

#### Scenario: Init upload session successfully
- **WHEN** a client sends a valid init request with filename, extension, total size, and chunk count
- **THEN** the API returns `rutaTemporalId`, `archivoTemporalId`, configured `chunkSizeBytes`, and status `IN_PROGRESS`

#### Scenario: Init rejected for invalid size or extension
- **WHEN** total size exceeds configured maximum OR extension is not allowed
- **THEN** the API rejects the request with a typed upload validation error

### Requirement: Chunked upload via streaming
The system MUST receive file content in chunks using streaming and MUST NOT require loading the full file into memory.

#### Scenario: Upload chunk accepted
- **WHEN** a valid chunk request is sent for an active upload session
- **THEN** the chunk is persisted under the temporary path and upload progress is updated

#### Scenario: Upload chunk rejected
- **WHEN** chunk index is out of range OR session is invalid OR ownership does not match
- **THEN** the API rejects the chunk and does not mutate final upload state

### Requirement: Upload status and resumability
The system MUST expose upload status so clients can resume interrupted uploads.

#### Scenario: Query upload status
- **WHEN** client requests upload status for an active session
- **THEN** the API returns current state, bytes received, and missing/pending chunks

### Requirement: Upload completion with integrity checks
The system MUST validate upload completeness and integrity before marking a session as `COMPLETED`.

#### Scenario: Complete upload successfully
- **WHEN** all expected chunks exist and final size/hash checks pass
- **THEN** chunks are assembled into final temporary file and session status becomes `COMPLETED`

#### Scenario: Complete upload fails integrity validation
- **WHEN** final size mismatch OR expected hash mismatch occurs
- **THEN** session status becomes `FAILED` and completion is rejected

### Requirement: Upload cancellation and cleanup
The system MUST allow explicit cancellation and cleanup of temporary upload artifacts.

#### Scenario: Cancel upload
- **WHEN** client sends cancel request for an owned upload session
- **THEN** temporary chunks/final file are removed and status becomes `CANCELLED`

### Requirement: Storage endpoint integration by references
The final storage endpoint MUST consume temporary references and MUST NOT accept direct binary payload in this change.

#### Scenario: AlmacenarDocumento accepts completed upload reference
- **WHEN** `POST /almacenamiento` receives `rutaTemporalId` and `archivoTemporalId` for a `COMPLETED` session owned by the user
- **THEN** the existing storage pipeline proceeds with those references

#### Scenario: AlmacenarDocumento rejects non-completed upload reference
- **WHEN** upload reference is missing, foreign, or status is not `COMPLETED`
- **THEN** request is rejected with validation error and storage transaction is not started

### Requirement: Configurable temporary storage root in IIS/runtime
The temporary upload path MUST be resolved from configuration (`StoragePaths:Temp`) and remain inside that root.

#### Scenario: Missing temp configuration
- **WHEN** temp path is missing or inaccessible
- **THEN** upload operations fail fast with operational configuration error

### Requirement: TTL-based cleanup
The system MUST support expiration and cleanup of stale temporary uploads.

#### Scenario: Cleanup expired sessions
- **WHEN** cleanup job runs and finds expired upload sessions
- **THEN** stale chunks/files are removed and sessions are marked as expired/cleaned

### Requirement: Backend implementation policy compliance
Backend implementation MUST follow architecture/testing constraints in `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Design and tasks review
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they include repository impact, interface policy, DI registration, AppResponses/try-catch handling, and test evidence plan

