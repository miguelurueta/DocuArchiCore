## ADDED Requirements

### Requirement: Archive must validate Git integration
El sistema SHALL validar en `opsxj:archive` que la rama del cambio este integrada en la rama base antes de archivar.

#### Scenario: Change branch already merged
- **WHEN** existe la rama local del cambio y es ancestro de la rama base
- **THEN** el comando permite continuar con el archive

### Requirement: Archive must block when change is not integrated
El sistema SHALL bloquear `opsxj:archive` cuando el cambio no esta integrado en Git.

#### Scenario: Change branch not merged into base
- **WHEN** la rama del cambio no es ancestro de la rama base
- **THEN** el comando falla con mensaje explicito y no archiva

### Requirement: Archive transitions Jira issue to done
El sistema SHALL transicionar el ticket Jira asociado a estado terminado cuando el archive se completa exitosamente.

#### Scenario: Done transition available
- **WHEN** el issue tiene una transicion a estado de cierre (Done/Terminado/Closed)
- **THEN** el comando ejecuta la transicion y reporta el estado actualizado

### Requirement: Clear Jira transition failure
El sistema SHALL reportar errores claros cuando no se pueda transicionar el ticket Jira a terminado.

#### Scenario: No matching done transition
- **WHEN** el workflow Jira no expone una transicion de cierre esperada
- **THEN** el comando falla con detalle de estados disponibles
