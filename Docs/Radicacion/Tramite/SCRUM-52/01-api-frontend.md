# API Frontend - Alias Constantes y Mensaje Funcional

## Endpoint

- `POST /api/radicacion/registrar-entrante`

## Cambio funcional

Se actualiza el formato de mensaje de validación:

- Antes: `Error en campo '<Alias>': ...`
- Ahora: `<Alias>: valor inválido.`

## Alias constantes relevantes (SCRUM-52)

- `TipoRadicado` -> `Tipo de Radicado`
- `IdtipoRadicado` -> `Tipo de Radicado`
- `Remitente_Cor` -> `Solicitante`
- `Remit_Dest_Interno_id_Remit_Dest_Int` -> `Solicitante`
- `Destinatario_Cor` -> `Responsable del trámite`
- `Destinatario_Externo_id_Dest_Ext` -> `Responsable del trámite`

## Ejemplo de respuesta

```json
{
  "success": false,
  "message": "Validacion fallida",
  "data": [
    {
      "field": "FECHALIMITERESPUESTA",
      "message": "Fecha Límite Respuesta: valor inválido.",
      "type": "Required",
      "attemptedValue": ""
    }
  ],
  "errors": [
    {
      "field": "FECHALIMITERESPUESTA",
      "message": "Fecha Límite Respuesta: valor inválido.",
      "type": "Required",
      "attemptedValue": ""
    }
  ]
}
```
