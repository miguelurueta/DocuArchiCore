## ADDED Requirements

### Requirement: OpenSpec context expansion for SCRUM-23
El cambio SHALL ampliar `openspec/config.yaml` para definir politicas de contexto multi-repo y limites de alcance para SCRUM-23.

#### Scenario: Context rules are defined in config
- **WHEN** se revisa `openspec/config.yaml`
- **THEN** existen reglas para referencia obligatoria de `openspec/context/multi-repo-context.md` en cambios cross-repo

#### Scenario: SCRUM-23 scope boundary is explicit
- **WHEN** se revisan los artefactos del cambio SCRUM-23
- **THEN** se indica que el alcance de este cambio es context-only y no incluye implementacion de codigo en repos ejecutores

### Requirement: Coordinator-driven multi-repo tracking
El cambio SHALL usar `DocuArchiCore` como coordinador de OpenSpec para mantener trazabilidad del estado multi-repo de SCRUM-23.

#### Scenario: Multi-repo progress is updated in coordinator
- **WHEN** se actualiza estado o PR de un repo ejecutor
- **THEN** el coordinador registra enlaces y estado en `sync.md`
