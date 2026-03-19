# jira-scrum-78 Specification

## Purpose
TBD - created by archiving change scrum-78-opsxdeploy-automatizar-preparaci-n-y-val. Update Purpose after archive.
## Requirements
### Requirement: Validate IIS-oriented publish folders
The system MUST provide a `doctor` command that validates a publish folder before it is used for IIS packaging.

#### Scenario: Required runtime files exist
- **WHEN** the operator runs `opsxdeploy doctor` against a valid publish folder
- **THEN** the command confirms presence of `*.dll`, `*.deps.json`, `*.runtimeconfig.json` and `web.config`

#### Scenario: Development artifacts are blocked
- **WHEN** the publish folder contains `appsettings.Development.json` or tooling content such as `Tools\`
- **THEN** `opsxdeploy doctor` fails and reports the forbidden artifacts

#### Scenario: Obvious secrets are detected
- **WHEN** `appsettings.json` contains populated secrets or unsafe connection strings
- **THEN** `opsxdeploy doctor` fails unless secrets validation is explicitly relaxed

### Requirement: Prepare site and storage directories without deploying to IIS
The system MUST provide a `prepare` command that creates the folder structure needed for an IIS deployment without requiring direct IIS mutation.

#### Scenario: Site and storage paths are prepared
- **WHEN** the operator runs `opsxdeploy prepare --sitePath <path> --dataPath <path>`
- **THEN** the site folder is created and storage folders `temp`, `uploads`, `avatars`, `exports` and `logs` are created under `dataPath`

### Requirement: Build a clean package ready for manual IIS application
The system MUST provide a `publish-package` command that creates a cleaned package ready to be copied into an IIS site.

#### Scenario: Clean package is generated
- **WHEN** the operator runs `opsxdeploy publish-package --publishPath <path> --outputPath <path>`
- **THEN** the command copies runtime files into `outputPath` and excludes development-only artifacts

#### Scenario: Direct IIS deployment is not mandatory
- **WHEN** the MVP flow is executed successfully
- **THEN** the result is a validated package and prepared directories, without requiring App Pool or site creation in IIS

### Requirement: Publication manual is mandatory
The change MUST include an operational manual that documents the complete path from Visual Studio publish to IIS application.

#### Scenario: Operator follows documented process
- **WHEN** the operator opens the delivery documentation
- **THEN** the manual explains publish, validation, package generation, IIS preparation, environment variables, permissions and troubleshooting

