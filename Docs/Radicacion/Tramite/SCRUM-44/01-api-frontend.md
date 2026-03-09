# DTO frontend - ValidaCamposObligatorios

## Servicio

- Nombre: `IValidaCamposObligatoriosService`
- Método: `ValidaCamposObligatoriosAsync`
- Firma:
  - `RegistrarRadicacionEntranteRequestDto request`
  - `string defaultDbAlias`
  - `IReadOnlyCollection<DetallePlantillaRadicado> detallePlantilla`

## Request DTO principal

`RegistrarRadicacionEntranteRequestDto` combina:
- Campos fijos (ej. `IdPlantilla`, `ASUNTO`, `ANEXOS_COR`, `FECHALIMITERESPUESTA`, `numeroFolios`).
- Objetos tipados (`Remitente`, `Destinatario`, `Tipo_tramite`, `TipoRadicado`, etc.).
- Lista dinámica `Campos[]` (`NombreCampo`, `Valor`).

## Response DTO

`AppResponses<List<ValidationError>?>`

Casos:
- `success=false`, `message="Validacion fallida"`, `data=[...]` cuando hay faltantes.
- `success=true`, `message="Sin resultados"`, `data=null` cuando no hay registros dinámicos.
- `success=true`, `message="OK"`, `data=[]` cuando valida correctamente.
- `success=false`, `message="Error validando campos obligatorios"` ante excepción controlada.

## ValidationError

- `Field`
- `Message`
- `Type`
- `AttemptedValue`
