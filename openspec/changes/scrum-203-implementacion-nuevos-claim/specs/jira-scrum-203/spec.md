## ADDED Requirements

### Requirement: Emitir nuevos claims de relación en JWT
El sistema MUST emitir en el JWT de autenticación los claims `IdUsuarioWorkflow`, `IdUsuarioWorkflowExt`, `IdUsuarioRadicador` e `IdUsuarioDa` con base en datos de `remit_dest_interno`.

#### Scenario: Login normal emite claims nuevos
- **GIVEN** un usuario de gestión autenticado con relaciones válidas en `remit_dest_interno`
- **WHEN** el sistema genera el token JWT por login normal
- **THEN** el JWT contiene los 4 claims nuevos con sus valores correspondientes

#### Scenario: Relaciones sin valor emiten fallback
- **GIVEN** una o más relaciones nulas/no definidas en origen
- **WHEN** el sistema genera el JWT
- **THEN** los claims nuevos se emiten con valor `0`

### Requirement: Paridad entre login normal y 2FA
El sistema MUST emitir el mismo contrato de claims en JWT final tanto para login normal como para login con segundo factor.

#### Scenario: Token final en 2FA mantiene claims nuevos
- **GIVEN** un flujo de autenticación con OTP válido
- **WHEN** se emite el JWT final tras verificar el segundo factor
- **THEN** el JWT contiene los 4 claims nuevos y también los claims legacy existentes

### Requirement: Compatibilidad con claims legacy
El sistema MUST mantener intactos los claims legacy actuales (`usuarioid`, `uid`, `defaulalias`, `defaulaliaswf`, `perm`).

#### Scenario: No regresión de claims existentes
- **WHEN** se emite un JWT después del cambio
- **THEN** los claims legacy permanecen disponibles con su naming actual

### Requirement: Trazabilidad técnica para frontend
El sistema MUST documentar en artefactos técnicos el mapeo exacto claim-origen y ejemplos de payload JWT para integración frontend.

#### Scenario: Contrato técnico actualizado
- **WHEN** frontend revisa documentación del ticket
- **THEN** encuentra tabla de homologación de claims, tipo de dato y regla de fallback
