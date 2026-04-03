# jira-scrum-116 Specification

## Purpose
TBD - created by archiving change scrum-116-actualizacion-normalizacion-salida-workf. Update Purpose after archive.
## Requirements
### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-116.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-116

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Workflow inbox repository returns normalized rows
The system MUST normalize workflow inbox query rows inside `WorkflowInboxRepository` and return typed dictionaries instead of `ExpandoObject`.

#### Scenario: Repository returns typed normalized rows
- **WHEN** `WorkflowInboxRepository.GetInboxAsync` completes successfully
- **THEN** the result type is `AppResponses<List<Dictionary<string, object?>>>`
- **AND** each row preserves original values and nulls without type coercion

#### Scenario: Repository derives id from id_tarea
- **WHEN** a normalized row does not include `id`
- **AND** it includes `id_tarea`
- **THEN** the repository adds `id` with the same value as `id_tarea`

#### Scenario: Repository avoids synthetic id when id_tarea is missing
- **WHEN** a normalized row does not include `id`
- **AND** it does not include `id_tarea`
- **THEN** the repository does not add `id`

### Requirement: Workflow inbox service consumes normalized rows without dynamic casting
The system MUST consume normalized dictionaries in `WorkflowInboxService` without depending on `ExpandoObject`.

#### Scenario: Service builds DynamicUiTableDto from normalized rows
- **WHEN** `WorkflowInboxService` receives normalized inbox rows from the repository
- **THEN** it passes dictionary-based rows to `DynamicUiTableBuilder`
- **AND** it preserves the existing functional result of the workflow inbox table

