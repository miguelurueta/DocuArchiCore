# jira-scrum-75 Specification

## Purpose
TBD - created by archiving change scrum-75-actualiza-registro-radicacion-entrante. Update Purpose after archive.
## Requirements
### Requirement: Registrar entrante acepta tipoModuloRadicacion
El endpoint `POST /api/radicacion/registrar-entrante` MUST aceptar el parametro `tipoModuloRadicacion` por query string con valor por defecto `1`.

#### Scenario: No se envia tipoModuloRadicacion
- **WHEN** el frontend invoca `registrar-entrante` sin query param
- **THEN** el controller usa `tipoModuloRadicacion = 1`
- **AND** propaga ese valor al service de registro

#### Scenario: Se envia tipoModuloRadicacion explicito
- **WHEN** el frontend invoca `registrar-entrante?tipoModuloRadicacion=2`
- **THEN** el controller propaga `2` al service
- **AND** mantiene el contrato AppResponses existente

### Requirement: El service propaga tipoModuloRadicacion al flujo de registro
`RegistrarRadicacionEntranteService` MUST normalizar y propagar `tipoModuloRadicacion` al request canonico y al repository de registro.

#### Scenario: El parametro llega con valor invalido
- **WHEN** `tipoModuloRadicacion <= 0`
- **THEN** el service usa `1` como valor funcional por defecto

#### Scenario: El parametro llega con valor valido
- **WHEN** `tipoModuloRadicacion > 0`
- **THEN** el service asigna ese valor al request canonico
- **AND** lo envía al repository de registro

### Requirement: tipoModuloRadicacion gobierna la prevalidacion workflow
La prevalidacion workflow previa al registro MUST ejecutarse cuando `tipoModuloRadicacion == 2`.

#### Scenario: tipoModuloRadicacion diferente de workflow
- **WHEN** `tipoModuloRadicacion != 2`
- **THEN** el service no invoca `IValidaPreRegistroWorkflowService`
- **AND** el flujo de registro continúa por el camino normal

#### Scenario: tipoModuloRadicacion workflow
- **WHEN** `tipoModuloRadicacion == 2`
- **THEN** el service invoca `IValidaPreRegistroWorkflowService`
- **AND** detiene el registro si la prevalidacion retorna error

### Requirement: El repository usa tipoModuloRadicacion para la politica de registro
`RegistrarRadicacionEntranteRepository` MUST usar `tipoModuloRadicacion` para decidir la ejecucion de pasos condicionados y reportarlo en metadata operativa.

#### Scenario: tipoModuloRadicacion activa estado inicial workflow
- **WHEN** `tipoModuloRadicacion == 2` o `tipoModuloRadicacion == 3`
- **THEN** la policy habilita `Q08`

#### Scenario: Respuesta exitosa incluye trazabilidad
- **WHEN** el registro finaliza correctamente
- **THEN** `MetadataOperativa` incluye `tipoModuloRadicacion`

