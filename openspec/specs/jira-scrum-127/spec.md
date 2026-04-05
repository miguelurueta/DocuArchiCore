# jira-scrum-127 Specification

## Purpose
Definir la politica operativa de uso de agentes Codex dentro del flujo `opsxj` del backend, dejando la guia visible en documentacion y en la salida del script sin intentar controlar tecnicamente la seleccion del modelo.

## Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-127.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-127

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: opsxj MUST document Codex agent operating policy
The system MUST include an explicit operating policy for Codex agents in the backend repository.

#### Scenario: Reviewer inspects tooling documentation
- **WHEN** the reviewer opens backend operational docs
- **THEN** there is a dedicated Codex agent strategy document
- **AND** it differentiates subagente mini vs agente principal
- **AND** it states that PowerShell does not control model selection

### Requirement: opsxj README MUST reference the agent policy
The system MUST reference the Codex agent policy from `Tools/jira-open/README.md`.

#### Scenario: Operator reads jira-open README
- **WHEN** an operator reviews `Tools/jira-open/README.md`
- **THEN** the README links to the Codex agent strategy
- **AND** summarizes the recommended use of mini vs principal

### Requirement: opsxj MUST print operational hints per command
The system MUST print visible operational guidance for Codex agent usage according to the executed command.

#### Scenario: Running doctor
- **WHEN** `opsxj.ps1 doctor` runs
- **THEN** the output includes a Codex agent hint block
- **AND** it recommends mini for analysis/report review
- **AND** it recommends principal for execution and final decisions

#### Scenario: Running orchestrated commands
- **WHEN** commands such as `orchestrate:new`, `orchestrate:publish`, `archive`, or `orchestrate:archive` run
- **THEN** the output includes guidance that keeps Jira/Git/GitHub and multi-repo closure in the principal agent
