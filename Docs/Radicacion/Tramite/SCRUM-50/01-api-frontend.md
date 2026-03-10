# API Frontend - Alias en Mensajes de Validacion

## Endpoint

- `POST /api/radicacion/registrar-entrante`

## Objetivo funcional

Los mensajes de error de validacion ahora incluyen alias descriptivo del campo:

- Campos fijos: alias centralizado en backend.
- Campos dinamicos: alias tomado de `DetallePlantillaRadicado.Alias_Campo`.

## DTO de entrada relevante

`RegistrarRadicacionEntranteRequestDto`:

- Fijos: `Asunto`, `FECHALIMITERESPUESTA`, `Anexos_Cor`, `TipoRadicado`, etc.
- Dinamicos: `Campos[]` (`NombreCampo`, `Valor`).
- Contexto dinamico (backend): `DetallePlantillaRadicado[]`.

## Alias fijos soportados

- `TipoRadicado`, `IdtipoRadicado`, `IdTipoRadicado` -> `Tipo de Radicado`
- `Anexos_Cor` -> `Anexos del Radicado`
- `FECHALIMITERESPUESTA` -> `Fecha Límite Respuesta`
- `Asunto` -> `Asunto`
- `id_tipo_flujo_workflow` -> `Flujo trámite`
- `Remitente_Cor`, `Remit_Dest_Interno_id_Remit_Dest_Int` -> `Solicitante`
- `Destinatario_Cor`, `Destinatario_Externo_id_Dest_Ext` -> `Responsable del trámite`
- `Numero_Folios` -> `Número Folios`

## Formato de mensajes

- Obligatorio: `Error en campo '<Alias>': campo obligatorio.`
- MaxLength: `Error en campo '<Alias>': longitud maxima permitida: N.`
- Unico: `Error en campo '<Alias>': el valor ya se encuentra registrado.`

## Ejemplo de respuesta

```json
{
  "success": false,
  "message": "Validacion fallida",
  "data": [
    {
      "field": "FECHALIMITERESPUESTA",
      "message": "Error en campo 'Fecha Límite Respuesta': campo obligatorio.",
      "type": "Required",
      "attemptedValue": ""
    }
  ],
  "errors": [
    {
      "field": "FECHALIMITERESPUESTA",
      "message": "Error en campo 'Fecha Límite Respuesta': campo obligatorio.",
      "type": "Required",
      "attemptedValue": ""
    }
  ]
}
```
