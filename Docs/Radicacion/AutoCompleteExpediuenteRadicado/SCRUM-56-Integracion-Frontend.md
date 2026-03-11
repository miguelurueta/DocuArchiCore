# SCRUM-56 Integracion Frontend - AutoComplete Token Expediente Radicado

## API
- Metodo: `POST`
- Ruta: `/api/PlantillaRadicado/solicitaAutoCompleteTokenExpedienteRadicado`
- Auth: Bearer token con claim `defaulalias`

## DTO de Entrada
`ParameterAutoComplete`

- `TextoBuscado` (`string`): texto a buscar en campos del expediente
- `defaultDbAlias` (`string`): lo completa backend desde claim `defaulalias`
- `tbl_control` (`string`): no requerido para este endpoint
- `name_campo` (`string`): no requerido para este endpoint

## Campos de busqueda en backend
- `CODIGO_UNICO`
- `ALEAS_EXPEDIENTE`
- `NOMBRE_PERSONA_EXPEDIENTE`
- `IDENTIFICACION_PERSONA_EXPEDIENTE`
- `NOMBRE_RESPONSABLE_EXPEDIENTE`
- `IDENFICACION_RESPONSABLE_EXPEDIENTE`

## Respuesta
`AppResponses<List<rowTomSelect>>`

- `success` (`bool`)
- `message` (`string`)
- `data` (`List<rowTomSelect>` o `null`)
- `errors` (`object[]`)

`rowTomSelect`:
- `idValue` = `ID_EXPEDIENTE`
- `texValue` = `CODIGO_UNICO`
- `textValueDescritipo` = `ALEAS_EXPEDIENTE`

## Ejemplo Request
```json
{
  "TextoBuscado": "EXP",
  "defaultDbAlias": "",
  "tbl_control": "",
  "name_campo": ""
}
```

## Ejemplo Response OK
```json
{
  "success": true,
  "message": "OK",
  "data": [
    {
      "idValue": 1,
      "texValue": "EXP-001-2026",
      "textValueDescritipo": "ALIAS-UNO"
    }
  ],
  "meta": null,
  "errors": []
}
```

## Ejemplo Response Sin Resultados
```json
{
  "success": true,
  "message": "Sin resultados",
  "data": null,
  "meta": null,
  "errors": []
}
```

## Ejemplo Consumo React
```ts
const response = await fetch("/api/PlantillaRadicado/solicitaAutoCompleteTokenExpedienteRadicado", {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`
  },
  body: JSON.stringify({
    TextoBuscado: texto
  })
});

const payload = await response.json();
const opciones = payload?.data ?? [];
```
