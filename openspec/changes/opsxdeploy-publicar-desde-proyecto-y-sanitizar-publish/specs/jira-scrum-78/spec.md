## MODIFIED Requirements

### Requirement: IIS package preparation workflow
The system MUST provide an `opsxdeploy publish-package` command that can build the source project or consume an existing publish folder, then produce a cleaned package ready to be copied into an IIS site.

#### Scenario: Build package directly from project source
- **WHEN** the operator runs `opsxdeploy publish-package` with a `.csproj` source such as `DocuArchi.Api.csproj`
- **THEN** the tool executes `dotnet publish` into a temporary staging folder
- **AND** the final package is created from that staging output without requiring a manual publish step

#### Scenario: Keep compatibility with existing publish-path mode
- **WHEN** the operator runs `opsxdeploy publish-package` with `-PublishPath <path>`
- **THEN** the tool preserves the current mode based on an existing publish folder
- **AND** the final package still goes through the cleaned-package validation rules

### Requirement: Cleaned package excludes non-production artifacts
The system MUST ensure that the final IIS package excludes known non-production artifacts even if they exist in the source project or intermediate publish.

#### Scenario: Drop development artifacts from final package
- **WHEN** the source project or staging publish contains `appsettings.Development.json` or `Tools/**`
- **THEN** those artifacts are excluded from the final package output
- **AND** the tool reports the cleanup actions in its operational output

### Requirement: Final package does not carry known populated secrets
The system MUST prevent the final IIS package from shipping known populated secrets in `appsettings.json`.

#### Scenario: Sanitize known secret fields in final package
- **WHEN** `appsettings.json` in the source publish contains populated values for `Jwt.Key`, generic `Secret` fields, or connection strings with credentials
- **THEN** the final package replaces those values with placeholders or blank values according to the sanitization policy
- **AND** the source repository files remain unchanged

#### Scenario: Fail when final package still contains unsafe known patterns
- **WHEN** the final package still contains blocked secret patterns after sanitization
- **THEN** `opsxdeploy publish-package` fails with an actionable error message that identifies the remaining unsafe patterns
