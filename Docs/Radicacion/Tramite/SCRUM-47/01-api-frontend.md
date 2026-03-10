# DTO frontend - ValidaCamposRadicacion

## Servicio

- Nombre: `IValidaCamposRadicacionService`
- Metodo: `ValidaCamposRadicacionAsync`
- Firma:
  - `string defaultDbAlias`
  - `RegistrarRadicacionEntranteRequestDto request`
  - `IReadOnlyCollection<DetallePlantillaRadicado> detallePlantilla`

## Objetivo

- Orquestar en una sola llamada las validaciones:
  - `ValidaCamposObligatoriosService`
  - `ValidaDimensionCamposService`
  - `ValidaCamposDinamicosUnicosRadicacionService`
- Retornar errores consolidados como `List<ValidationError>` envuelto en `AppResponses`.

## DTO de entrada

`RegistrarRadicacionEntranteRequestDto`:
- Incluye campos fijos y dinamicos en `Campos[]` (`NombreCampo`, `Valor`).
- Debe incluir `IdPlantilla`, `Remitente`, `Destinatario`, `TipoRadicado`, `TipoPlantillaRadicado`.

## Respuesta

`AppResponses<List<ValidationError>?>`

Escenarios:
- `success=true`, `message="OK"`, `data=[]`: validacion satisfactoria.
- `success=true`, `message="Sin resultados"`, `data=null`: no aplica validacion con resultados.
- `success=false`, `message="Validacion fallida"`, `data=[ValidationError...]`: errores de negocio.
- `success=false`, `message="Error validando campos de radicacion"`: excepcion controlada.

## Ejemplo de error

```json
{
  "success": false,
  "message": "Validacion fallida",
  "data": [
    {
      "field": "CampoIdentificador",
      "message": "El valor ya se encuentra registrado",
      "type": "Unique",
      "attemptedValue": "NIT-EXISTENTE"
    }
  ]
}
```
