# jira-scrum-198 Specification

## Purpose
TBD - created by archiving change scrum-198-implementacion-correcion-dbnull-almacena. Update Purpose after archive.
## Requirements
### Requirement: Null semantics in transactional inventory insert
Storage Engine transactional inventory inserts MUST use C# `null` for optional fields and MUST NOT send `DBNull.Value` through repository parameter payloads.

#### Scenario: Optional series fields omitted
- **GIVEN** an inventory insert with `SERIE_DOCUMENTO` and `SUBSERIE_DOCUMENTO` not provided
- **WHEN** the repository builds insert parameters
- **THEN** those fields are sent as `null`
- **AND** the insert does not fail with `System.DBNull cannot be used as a parameter value`

#### Scenario: Optional numeric metadata omitted
- **GIVEN** an inventory insert with `ID_SERIE_DOCUMENTO` or `ID_SUBSERIE_DOCUMENTO` not provided
- **WHEN** the transactional insert executes
- **THEN** the values are persisted as SQL `NULL` where schema allows it
- **AND** no runtime parameter-mapping exception is thrown

### Requirement: Dapper engine preserves null without manual DBNull injection
`InsertBeginTrandAsync` MUST accept null-valued payload members without requiring callers to use `DBNull.Value`.

#### Scenario: Repository sends null payload to Dapper engine
- **GIVEN** a transactional insert payload that includes null optional values
- **WHEN** `InsertBeginTrandAsync` executes the insert
- **THEN** the insert command binds parameters correctly with null semantics
- **AND** no adapter-side conversion to `System.DBNull` is required in service/repository layers

### Requirement: Error traceability for transactional inserts
When a transactional insert fails, the reported error MUST identify the transactional operation context.

#### Scenario: Transactional insert fails in persistence layer
- **GIVEN** an exception during `InsertBeginTrandAsync`
- **WHEN** the error is wrapped in `QueryResult`
- **THEN** the message indicates transactional insert context
- **AND** preserves original exception detail for diagnostics

