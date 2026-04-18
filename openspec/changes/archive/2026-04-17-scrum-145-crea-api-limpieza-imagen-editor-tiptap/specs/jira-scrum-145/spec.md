## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-145.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-145

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Orphan editor images dry-run
The system MUST expose a dry-run capability to report how many editor images would be affected by an orphan cleanup operation.

#### Scenario: Dry-run returns affected count only
- **GIVEN** there are editor images without active links (orphans)
- **WHEN** a cleanup request is executed with `DryRun = true`
- **THEN** the response returns the count of images that would be marked
- **AND** no record is updated (no `DeletedAt` changes)

### Requirement: Orphan editor images logical delete only
The system MUST only perform logical deletion (set `DeletedAt`) for orphan editor images in this phase.

#### Scenario: Cleanup marks DeletedAt for orphan images
- **GIVEN** there are editor images without active links (orphans)
- **WHEN** a cleanup request is executed with `DryRun = false`
- **THEN** the system sets `DeletedAt` for the orphan images
- **AND** it does not physically delete bytes or storage objects

### Requirement: Exclude non-orphans and already deleted
The system MUST NOT mark images that still have an active link, and MUST ignore images already logically deleted.

#### Scenario: Images with active links are unaffected
- **GIVEN** an editor image has at least one active link in `ra_editor_document_image_links`
- **WHEN** cleanup is executed
- **THEN** that image remains unchanged

#### Scenario: Already deleted images are ignored
- **GIVEN** an editor image has `DeletedAt IS NOT NULL`
- **WHEN** cleanup is executed
- **THEN** that image is not counted nor updated

### Requirement: Filter by age threshold
The system MUST support filtering candidate orphan images by age, using an `OlderThanMinutes` request parameter.

#### Scenario: Only older orphan images are affected
- **GIVEN** there are orphan images newer and older than the requested threshold
- **WHEN** cleanup is executed with `OlderThanMinutes > 0`
- **THEN** only orphan images older than the threshold are counted/marked

### Requirement: Authorization via claim defaulalias
The system MUST validate the presence of the `defaulalias` claim before executing the cleanup operation.

#### Scenario: Missing defaulalias returns error
- **GIVEN** the request does not include the `defaulalias` claim
- **WHEN** cleanup is requested
- **THEN** it returns an error response in the standard `AppResponses<T>` shape
