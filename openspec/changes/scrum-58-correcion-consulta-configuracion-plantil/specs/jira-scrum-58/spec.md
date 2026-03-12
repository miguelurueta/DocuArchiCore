## ADDED Requirements

### Requirement: Consulta configuracion plantilla alineada a estructura real
La funcion `SolicitaConfiguracionPlantillaAsync` MUST reflejar la estructura real de `ra_rad_config_plantilla_radicacion` en el entorno configurado.

#### Scenario: Consulta con estructura real
- **WHEN** se consulta por `idPlantilla`, `tipoRadicacionPlantilla` y `defaultDbAlias`
- **THEN** el modelo y DTO exponen `id_rad_config_plantilla_radicacion`, `Descripcion_tipo_radicacion`, `util_notificacion_remitente`, `util_notificacion_destinatario`, `util_valida_restriccion_radicacion`
- **AND** la consulta sigue siendo parametrizada y envuelta en `AppResponses`

#### Scenario: Sin resultados
- **WHEN** no hay registros para los filtros
- **THEN** retorna `success=true`, `message="Sin resultados"` y `data=null`

#### Scenario: Error controlado
- **WHEN** ocurre una excepcion en Repository o Service
- **THEN** retorna `success=false` con detalle en `errors` y sin romper contrato

### Requirement: Reglas backend OPSXJ
Las actualizaciones backend MUST cumplir `openspec/context/OPSXJ_BACKEND_RULES.md`.

#### Scenario: Cumplimiento de arquitectura
- **WHEN** se revisa el cambio
- **THEN** mantiene patron Controller -> Service -> Repository, DI en `Program.cs`, y pruebas unitarias/integracion/contract
