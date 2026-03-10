## ADDED Requirements

### Requirement: Mensajes de validación con alias descriptivo
La API de radicación entrante MUST retornar mensajes de validación usando alias funcionales para campos fijos y dinámicos.

#### Scenario: Campo fijo inválido usa alias funcional
- **GIVEN** una validación fallida sobre un campo fijo de radicación
- **WHEN** el backend construye el `ValidationError`
- **THEN** el `Message` incluye el alias funcional (ej. `Fecha Límite Respuesta`) y no solo el nombre técnico

#### Scenario: Campo dinámico inválido usa Alias_Campo de plantilla
- **GIVEN** un campo dinámico con `Alias_Campo` configurado en `DetallePlantillaRadicado`
- **WHEN** ese campo falla validación
- **THEN** el `Message` incluye el alias de plantilla

### Requirement: Alias aplicados en validaciones de radicación
El backend MUST aplicar alias en todas las validaciones orquestadas por `ValidaCamposRadicacionService`.

#### Scenario: Campo obligatorio faltante
- **WHEN** falla `ValidaCamposObligatoriosService`
- **THEN** el mensaje usa formato `Error en campo '<Alias>': campo obligatorio.`

#### Scenario: Campo excede longitud máxima
- **WHEN** falla `ValidaDimensionCamposService`
- **THEN** el mensaje usa formato `Error en campo '<Alias>': longitud maxima permitida: N.`

#### Scenario: Campo dinámico único duplicado
- **WHEN** falla `ValidaCamposDinamicosUnicosRadicacionService`
- **THEN** el mensaje usa formato `Error en campo '<Alias>': el valor ya se encuentra registrado.`

### Requirement: Cumplimiento de reglas backend
La implementación MUST respetar las reglas de `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Revisión de artefactos y pruebas
- **WHEN** se revisa el cambio
- **THEN** existen pruebas unitarias para alias en obligatorio, longitud y unicidad
