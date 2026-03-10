## ADDED Requirements

### Requirement: Mensaje por tipo de validación
La API de radicación entrante MUST construir mensajes de validación según tipo de error.

#### Scenario: Error requerido
- **WHEN** un campo obligatorio no tiene valor
- **THEN** el mensaje es `Campo <Alias>: requerido.`

#### Scenario: Error por valor existente
- **WHEN** una validación de unicidad detecta valor ya registrado
- **THEN** el mensaje es `Campo <Alias>: valor existente.`

#### Scenario: Error por formato no compatible
- **WHEN** falla validación de tipo o dimensión
- **THEN** el mensaje es `Campo <Alias>: formato no compatible.`

### Requirement: Aplicación en validadores de radicación
El formateo de mensajes MUST aplicarse en validaciones de obligatorio, unicidad, tipo y dimensión.

#### Scenario: Orquestación consolidada
- **WHEN** `ValidaCamposRadicacionService` integra errores de validación
- **THEN** los `ValidationError.Message` usan formato funcional por tipo

### Requirement: Cumplimiento de reglas backend
La implementación MUST respetar `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Revisión de pruebas y arquitectura
- **WHEN** se revisa el cambio
- **THEN** existen pruebas unitarias con mensajes por tipo y documentación técnica actualizada
