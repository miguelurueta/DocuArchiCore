## ADDED Requirements

### Requirement: Validación de tipo de campos de radicación
El backend MUST validar que el valor recibido en campos fijos y dinámicos sea compatible con el tipo de dato de la tabla de plantilla.

#### Scenario: Campo con tipo compatible
- **GIVEN** tipos de columna obtenidos de la tabla de plantilla
- **WHEN** los valores recibidos cumplen el tipo esperado
- **THEN** la validación de tipo retorna `success=true`, `message="OK"` y `data=[]`

#### Scenario: Campo con tipo incompatible
- **GIVEN** un campo cuyo valor no cumple el tipo de dato esperado
- **WHEN** se ejecuta `ValidaTipoCamposService`
- **THEN** retorna `success=false` con `ValidationError` usando `ValidationHelper`

### Requirement: Lectura de tipos desde tabla de plantilla
El backend MUST consultar tipos de columnas desde `information_schema.columns` de la tabla referenciada por `system_plantilla_radicado`.

#### Scenario: Plantilla inexistente o sin columnas aplicables
- **WHEN** no existe tabla de plantilla o no hay columnas
- **THEN** retorna `success=true`, `data=null`, `message="Sin resultados"`

### Requirement: Integración en orquestador de validación
La validación de tipo MUST integrarse en `ValidaCamposRadicacionService`.

#### Scenario: Orquestación completa
- **WHEN** se ejecuta `ValidaCamposRadicacionAsync`
- **THEN** consolida resultados de obligatorios, dimensión, únicos y tipo de campo

### Requirement: Cumplimiento de reglas backend
La implementación MUST respetar `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Revisión de arquitectura y pruebas
- **WHEN** se revisa el cambio
- **THEN** existen registros DI en `Program.cs`, pruebas unitarias e integración de `ValidaTipoCampos`
