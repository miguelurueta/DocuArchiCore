# jira-scrum-144 Specification

## Purpose
TBD - created by archiving change scrum-144-crear-api-cargar-documento-editor-tiptap. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-144.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-144

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Get editor document by id (read-only)
The system MUST expose an API endpoint to retrieve a previously saved editor document by `documentId` for re-edition in Tiptap.

#### Scenario: Existing document with images
- **GIVEN** a `documentId` that exists in `ra_editor_documents`
- **AND** the document has active image links in `ra_editor_document_image_links`
- **WHEN** the client requests the document detail
- **THEN** the response includes the persisted `DocumentHtml`
- **AND** includes document metadata (`TemplateId`, `TemplateVersion`, `DocumentTitle`, `FormatCode`, `StatusCode`, timestamps, users)
- **AND** includes a list of associated images with at least `ImageId` + `ImageUid`
- **AND** does not return binary data (no `image_bytes`)

#### Scenario: Existing document without images
- **GIVEN** a `documentId` that exists in `ra_editor_documents`
- **AND** the document has no active image links
- **WHEN** the client requests the document detail
- **THEN** the response returns the document
- **AND** returns an empty image list

#### Scenario: Document not found
- **GIVEN** a `documentId` that does not exist
- **WHEN** the client requests the document detail
- **THEN** the response indicates no results (`success = false`) with a meaningful message and error details

### Requirement: Claim validation and db alias
The API MUST validate the required claim(s) and resolve `defaultDbAlias` consistently with existing backend patterns.

#### Scenario: Missing required claim or db alias
- **GIVEN** a request without the required claim/db alias
- **WHEN** the endpoint is invoked
- **THEN** it returns a validation error using `AppResponses<T>` conventions

