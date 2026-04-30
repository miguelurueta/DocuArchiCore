# jira-scrum-174 Specification

## Purpose
TBD - created by archiving change scrum-174-actualiza-opsxj-new-orchestrate. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-174.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-174

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Publish/Archive tasks gate
`opsxj:orchestrate:publish`, `opsxj:archive`, and `opsxj:orchestrate:archive` MUST block when `tasks.md` contains pending items (`- [ ]`) unless an explicit override is provided.

#### Scenario: Pending tasks without override
- **GIVEN** `tasks.md` has one or more unchecked tasks
- **WHEN** user runs publish/archive command without override
- **THEN** command fails with policy message about incomplete tasks
- **AND** audit log writes step `tasks_validation` with status `error`

#### Scenario: Pending tasks with override
- **GIVEN** `tasks.md` has unchecked tasks
- **WHEN** user runs publish/archive command with `-ForceIncompleteTasks`
- **THEN** command continues with warning
- **AND** audit log writes step `tasks_validation` with status `warning`

### Requirement: OpenSpec review confirmation gate
Before publish/archive commands run, the workflow MUST require explicit OpenSpec review confirmation through environment confirmation.

#### Scenario: Missing review confirmation
- **GIVEN** OpenSpec artifacts exist but no review confirmation is provided
- **WHEN** user runs publish/archive without override
- **THEN** command fails with policy message requiring review
- **AND** audit log writes step `openspec_review_gate` with status `error`

#### Scenario: Review confirmation provided
- **GIVEN** `OPSXJ_OPENSPEC_REVIEW_CONFIRMED=1`
- **WHEN** user runs publish/archive
- **THEN** gate passes and command proceeds
- **AND** audit log writes step `openspec_review_gate` with status `ok`

