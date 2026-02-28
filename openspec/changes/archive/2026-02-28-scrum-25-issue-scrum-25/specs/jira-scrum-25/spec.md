## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-25.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-25

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Manual repository selection for opsxj:new
`opsxj:new` MUST allow explicit multi-repo selection through `-SelectRepos`.

#### Scenario: User chooses impacted repositories manually
- **WHEN** `opsxj:new <ISSUE-KEY> -SelectRepos` is executed
- **THEN** the command shows an interactive selection using the repo catalog
- **AND** selected repositories are used to build the impact matrix for `sync.md`

### Requirement: Auto-detection fallback to manual selection
`opsxj:new` MUST fallback to manual selection when repository auto-detection has no confident match.

#### Scenario: No repository can be auto-detected
- **WHEN** issue summary/description do not match repository keywords
- **THEN** the command informs that auto-detection failed
- **AND** prompts manual repo selection before continuing

### Requirement: Sync matrix initialization with explicit states
Generated `sync.md` MUST initialize repository rows with explicit operational states to avoid ambiguity.

#### Scenario: sync.md is generated from selected repos
- **WHEN** change artifacts are created for a Jira issue
- **THEN** selected repos are marked with `Impacta? = yes`, `opsxj:new = done`, `PR = pending`, `opsxj:archive = pending`, `Estado = todo`
- **AND** non-selected repos are marked as `Impacta? = no` with `n/a` operational fields
