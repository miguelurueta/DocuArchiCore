## ADDED Requirements

### Requirement: Generate proposal from Jira issue
El sistema SHALL generar una propuesta de OpenSpec a partir del `summary` y `description` del issue de Jira indicado por `issueKey`.

#### Scenario: Jira issue fetched and proposal generated
- **WHEN** el usuario ejecuta el generador con un `issueKey` valido
- **THEN** el sistema consulta Jira y usa el `summary` y `description` para construir el contenido de la propuesta

### Requirement: Persist proposal in change folder
El sistema SHALL guardar la propuesta generada en `openspec/changes/<issueKey>/proposal.md`.

#### Scenario: Proposal saved to OpenSpec change path
- **WHEN** la propuesta fue generada correctamente
- **THEN** el sistema crea (si no existe) la carpeta `openspec/changes/<issueKey>/` y guarda `proposal.md`

### Requirement: Clear failure messaging
El sistema SHALL reportar errores claros cuando falle la consulta a Jira o la generacion de la propuesta.

#### Scenario: Jira API request fails
- **WHEN** la consulta a Jira falla por credenciales, red o `issueKey` inexistente
- **THEN** el sistema retorna un error explicito y no escribe un `proposal.md` incompleto

### Requirement: Single command proposal generation
El sistema SHALL permitir ejecutar un solo comando que consulte Jira y genere `openspec/changes/<issueKey>/proposal.md`.

#### Scenario: Single command produces proposal file
- **WHEN** el usuario ejecuta `node scripts/generate-proposal-from-jira.js <issueKey>`
- **THEN** el sistema consulta Jira, genera la propuesta y guarda el archivo en el directorio del cambio
