## ADDED Requirements

### Requirement: PR creation inside opsxj:new flow
El sistema SHALL crear un Pull Request de GitHub dentro del flujo `opsxj:new` cuando la generacion de `proposal.md` termine correctamente.

#### Scenario: Proposal generation triggers PR creation
- **WHEN** el usuario ejecuta `opsxj:new` con un `issueKey` valido y la propuesta se guarda en `openspec/changes/<change-name>/proposal.md`
- **THEN** el sistema inicia el paso de creacion de PR en GitHub sin requerir un comando adicional

### Requirement: PR title must use Jira ticket subject
El sistema SHALL usar el `summary` del ticket Jira como base del titulo del Pull Request.

#### Scenario: PR title derived from Jira summary
- **WHEN** el ticket Jira devuelve `summary`
- **THEN** el titulo del PR se construye con ese asunto del ticket

### Requirement: Clear GitHub failure messaging
El sistema SHALL retornar mensajes de error claros cuando falle autenticacion, conexion, push de rama o creacion de PR en GitHub.

#### Scenario: GitHub authentication fails
- **WHEN** `gh auth` no esta disponible o el usuario no esta autenticado
- **THEN** el comando falla con mensaje explicito indicando como autenticar y no reporta PR creado

### Requirement: Idempotent PR behavior for reruns
El sistema SHALL evitar PR duplicados para el mismo `issueKey` cuando `opsxj:new` se ejecuta nuevamente sobre la misma rama de cambio.

#### Scenario: Existing PR found for change branch
- **WHEN** ya existe un PR abierto asociado a la rama del cambio
- **THEN** el sistema reutiliza/actualiza referencia del PR existente y no crea uno nuevo
