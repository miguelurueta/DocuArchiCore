# jira-scrum-77 Specification

## Purpose
TBD - created by archiving change scrum-77-desacoplar-docuarchi-api-de-docuarchicor. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-77.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-77

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Remove DocuArchiCore runtime dependency from DocuArchi.Api
`DocuArchi.Api` MUST stop referencing `DocuArchiCore.csproj` as a web runtime dependency.

#### Scenario: API project no longer references DocuArchiCore web
- **WHEN** a reviewer opens `DocuArchi.Api.csproj`
- **THEN** the project keeps `DocuArchiCore.Abstractions` as shared contract
- **AND** it does not include `..\DocuArchiCore\DocuArchiCore.csproj` as `ProjectReference`

### Requirement: Session implementation remains available from the API runtime
The current `SesionActual` behavior MUST remain resolvable from dependency injection after the decoupling.

#### Scenario: Program registers session services without DocuArchiCore web
- **WHEN** `DocuArchi.Api` starts
- **THEN** `SesionActual` is registered from code local to the API runtime or a non-web shared library
- **AND** `ISesionActual`, `ISesionGeneral`, `ISesionDocuArchi`, `ISesionGestionDocumental`, `ISesionRadicacion`, and `ISesionWorkflow` resolve to that implementation

### Requirement: Publish must not include duplicated static or configuration assets from DocuArchiCore
The API publish output MUST not pull `DocuArchiCore` web assets as referenced-project content.

#### Scenario: API publish runs after decoupling
- **WHEN** `dotnet publish` is executed for `DocuArchi.Api`
- **THEN** the publish completes without duplicate output conflicts caused by `appsettings.json`, `appsettings.Development.json`, or `Tools/jira-open/package.json`
- **AND** the generated static web assets manifest does not include `DocuArchiCore` as a referenced web project

