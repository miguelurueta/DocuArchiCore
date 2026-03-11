# SCRUM-55 Integracion Frontend - AutoComplete Token Radicado

## API
- Metodo: `POST`
- Ruta: `/api/PlantillaRadicado/solicitaAutoCompleteTokenRadicado`
- Auth: Bearer token con claim `defaulalias`

## DTO de Entrada
`ParameterAutoComplete`

- `TextoBuscado` (`string`): texto a buscar sobre `consecutivo_rad`
- `defaultDbAlias` (`string`): se completa en backend desde claim `defaulalias`
- `tbl_control` (`string`): no requerido para este endpoint
- `name_campo` (`string`): no requerido para este endpoint

## Respuesta
`AppResponses<List<rowTomSelect>>`

- `success` (`bool`)
- `message` (`string`)
- `data` (`List<rowTomSelect>` o `null`)
- `errors` (`object[]`)

`rowTomSelect`:
- `idValue` (`int`)
- `texValue` (`string`)
- `textValueDescritipo` (`string`)

## Ejemplo Request
```json
{
  "TextoBuscado": "260001",
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
      "idValue": 0,
      "texValue": "26000100010100001",
      "textValueDescritipo": ""
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
const response = await fetch("/api/PlantillaRadicado/solicitaAutoCompleteTokenRadicado", {
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
