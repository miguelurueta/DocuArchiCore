## ADDED Requirements

### Requirement: Mensaje específico para error MaxLength
La validación de radicación entrante MUST construir mensaje específico para errores de longitud máxima.

#### Scenario: Campo supera longitud permitida
- **WHEN** un campo excede la longitud permitida en validación de dimensión
- **THEN** el mensaje debe ser `Campo <Alias>: supera la longitud máxima permitida.`

### Requirement: Alcance del cambio sin afectar otros tipos
El ajuste MUST aplicar solo al tipo de error `MaxLength`.

#### Scenario: Otros tipos de error se mantienen
- **WHEN** ocurre error `Required`, `Unique` o `InvalidType`
- **THEN** el mensaje conserva su formato definido en SCRUM-53

### Requirement: Cumplimiento de reglas backend
La implementación MUST respetar `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Revisión de pruebas y documentación
- **WHEN** se revisa el cambio
- **THEN** existe al menos una prueba unitaria actualizada para `MaxLength` y documentación frontend ajustada
