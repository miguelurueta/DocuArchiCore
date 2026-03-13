## ADDED Requirements

### Requirement: Explicit non-interactive execution mode
The system SHALL support an explicit non-interactive execution mode for `opsxj:new`, `opsxj:doctor`, and `opsxj:archive` through the `-NonInteractive` parameter.

#### Scenario: New command runs in non-interactive mode
- **WHEN** the user runs `opsxj:new SCRUM-64 -NonInteractive`
- **THEN** the system executes the flow without requesting interactive input
- **AND** uses preauthorized configuration for Jira and GitHub access

### Requirement: Token-based GitHub auth in non-interactive mode
The system SHALL require `GITHUB_TOKEN` when `-NonInteractive` is active.

#### Scenario: Missing GitHub token
- **WHEN** the user runs `opsxj:new SCRUM-64 -NonInteractive`
- **AND** `GITHUB_TOKEN` is not configured
- **THEN** the system fails before creating artifacts or attempting interactive GitHub authentication

### Requirement: Backward compatibility with legacy mode
The system SHALL preserve current behavior when `-NonInteractive` is not specified.

#### Scenario: Legacy execution remains unchanged
- **WHEN** the user runs `opsxj:new SCRUM-64`
- **THEN** the system preserves the existing legacy behavior

### Requirement: Execution audit mode
The system SHALL record the execution mode in `opsxj` audit logs.

#### Scenario: Audit includes execution mode
- **WHEN** any `opsxj` command is executed
- **THEN** the audit entry includes whether the mode was `legacy` or `noninteractive`
