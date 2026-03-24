## ADDED Requirements

### Requirement: Orchestrated new must isolate impacted repositories
El sistema SHALL crear o reutilizar un worktree temporal cuando un repo impactado no pueda ser operado de forma segura desde su checkout principal.

#### Scenario: Repository checkout is busy
- **WHEN** `opsxj:orchestrate:new` detecta que un repo impactado esta en otra rama, sucio o con colision de branch
- **THEN** el comando crea o reutiliza un worktree temporal para ese repo
- **AND** continua el flujo sin mezclar el ticket con cambios ajenos

### Requirement: Orchestrated sync must classify repo impact
El sistema SHALL registrar el tipo de impacto real de cada repo impactado y usarlo para decidir si debe abrir PR o solo mantener trazabilidad.

#### Scenario: Repo requires implementation
- **WHEN** un repo requiere un cambio funcional real
- **THEN** `sync.md` lo marca como `implementation_required`
- **AND** el flujo exige branch, commit y PR para ese repo

#### Scenario: Repo only needs traceability
- **WHEN** un repo solo participa como dependencia tecnica o de trazabilidad
- **THEN** `sync.md` lo marca como `traceability_only`
- **AND** el flujo no obliga a crear un PR sin diff funcional

### Requirement: Orchestrated archive must trust merged pull requests
El sistema SHALL permitir `opsxj:orchestrate:archive` cuando el PR de un repo satelite este `MERGED`, aunque la branch remota del cambio ya no exista.

#### Scenario: Merged PR with deleted branch
- **WHEN** el PR satelite esta `MERGED`
- **AND** GitHub ya elimino la branch remota del cambio
- **THEN** `opsxj:orchestrate:archive` considera ese repo listo para archive
- **AND** no bloquea el cierre por ausencia de la branch

### Requirement: Orchestrated archive must clean its own temporary artifacts
El sistema SHALL limpiar residuos temporales creados por el flujo orquestado del issue actual antes y despues del archive.

#### Scenario: Temporary worktree and logs exist
- **WHEN** el issue actual tiene worktrees temporales, logs o artefactos `.tmp` creados por `opsxj`
- **THEN** el comando limpia esos residuos de forma segura
- **AND** no falla el archive por archivos temporales propios del flujo
