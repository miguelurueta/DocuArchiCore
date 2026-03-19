## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-80.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-80

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly reference `openspec/context/OPSXJ_BACKEND_RULES.md` as the baseline for implementation and testing discipline

### Requirement: Generate base web.config for IIS packages
`opsxdeploy publish-package` MUST leave a valid base `web.config` in the package output when the publish folder does not already contain one.

#### Scenario: Generate web.config when missing in publish output
- **WHEN** the operator runs `opsxdeploy publish-package` against a publish folder without `web.config`
- **THEN** the package output includes a generated `web.config` compatible with `AspNetCoreModuleV2`
- **AND** the generated file derives `aspNetCore/arguments` from the main published assembly when possible
- **AND** the generated file includes placeholder environment variables for the mandatory operational settings

### Requirement: Preserve and validate existing web.config
`opsxdeploy` MUST preserve an existing `web.config` and validate a minimum IIS structure before declaring the package ready.

#### Scenario: Preserve existing web.config
- **WHEN** the publish folder already contains `web.config`
- **THEN** `opsxdeploy publish-package` copies it into the package output without overwriting its contents automatically

#### Scenario: Fail on missing required aspNetCore structure
- **WHEN** an existing `web.config` omits the `aspNetCore/processPath` or `aspNetCore/arguments` configuration
- **THEN** `opsxdeploy doctor` and `opsxdeploy publish-package` report a failing validation

#### Scenario: Warn when environmentVariables block is absent or incomplete
- **WHEN** an existing `web.config` lacks an `environmentVariables` block or omits expected placeholders
- **THEN** `opsxdeploy` reports a warning without rewriting the file automatically

### Requirement: Documentation is part of the deliverable
The ticket MUST update the tool documentation and the IIS operational guide together with the implementation.

#### Scenario: Publish docs updated
- **WHEN** the change is reviewed
- **THEN** `Tools/iis-deploy/README.md` explains web.config generation/preservation behavior
- **AND** `Docs/Publicacion/IIS-DocuArchiApi.md` explains how operators should handle the generated or validated `web.config`
