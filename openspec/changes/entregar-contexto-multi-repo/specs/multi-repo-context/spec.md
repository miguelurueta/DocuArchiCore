## ADDED Requirements

### Requirement: Consolidated multi-repo context document
OpenSpec changes that impact more than one repository MUST reference a consolidated context document located at `openspec/context/multi-repo-context.md`.

#### Scenario: Cross-repo change references context
- **WHEN** a proposed change modifies components in two or more repositories
- **THEN** proposal and design artifacts include an explicit reference to `openspec/context/multi-repo-context.md`

### Requirement: Context document contains repository inventory
The consolidated context document MUST include, for each detected repository, branch, latest commit, remotes, and build artifacts.

#### Scenario: Context document completeness
- **WHEN** the context document is generated or updated
- **THEN** each repository entry contains path, branch, latest commit, remotes, and detected build artifacts (`.sln`, `.csproj`, `package.json`)