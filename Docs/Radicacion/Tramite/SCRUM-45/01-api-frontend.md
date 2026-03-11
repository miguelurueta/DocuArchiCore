# DTO frontend - ValidaDimensionCampos

## Servicio

- Nombre: `IValidaDimensionCamposService`
- Metodo: `ValidaDimensionCamposAsync`
- Firma:
  - `RegistrarRadicacionEntranteRequestDto request`
  - `string defaultDbAlias`
  - `IReadOnlyCollection<DetallePlantillaRadicado> detallePlantilla`

## Request DTO principal

`RegistrarRadicacionEntranteRequestDto` incluye:
- Campos fijos (`IdPlantilla`, `ASUNTO`, `ANEXOS_COR`, `FECHALIMITERESPUESTA`, `numeroFolios`, etc.).
- Objetos tipados (`Remitente`, `Destinatario`, `Tipo_tramite`, `TipoRadicado`, `TipoPlantillaRadicado`).
- Campos dinamicos `Campos[]` (`NombreCampo`, `Valor`).

## Regla de validacion

- El servicio consulta dimensiones maximas desde:
  - `information_schema.columns` de la tabla referenciada por `system_plantilla_radicado.Nombre_Plantilla_Radicado`.
  - `tam_campo` de `DetallePlantillaRadicado` para campos dinamicos.
- Si `valor.Length > maxLength`, genera `ValidationError` con tipo `MaxLength`.

## Response DTO

`AppResponses<List<ValidationError>?>`

Casos:
- `success=false`, `message="Validacion fallida"`, `data=[...]` cuando hay errores de longitud.
- `success=true`, `message="Sin resultados"`, `data=null` cuando no hay estructura para validar.
- `success=true`, `message="OK"`, `data=[]` cuando valida correctamente.
- `success=false`, `message="Error validando dimension de campos"` ante excepcion controlada.
