# SCRUM-173 - Integracion Frontend

## Endpoint
- Metodo: `GET`
- Ruta: `/api/gestion-correspondencia/tipos-respuesta`
- Auth: requerido (`[Authorize]`)
- Claim requerido: `defaulalias`

## Request
- Sin body
- Sin query params

## Response (contrato)
`AppResponses<List<ResponseDropdownDto>>`

## Ejemplo success
```json
{
  "success": true,
  "message": "YES",
  "data": [
    { "id": 1, "descripcion": "Positiva" },
    { "id": 2, "descripcion": "Negativa" }
  ],
  "meta": { "status": "success" },
  "errors": []
}
```

## Ejemplo empty
```json
{
  "success": true,
  "message": "Sin resultados",
  "data": [],
  "meta": { "status": "empty" },
  "errors": []
}
```

## Ejemplo error
```json
{
  "success": false,
  "message": "Error consultando tipos de respuesta",
  "data": [],
  "meta": { "status": "error" },
  "errors": [
    { "type": "Exception", "field": "defaultDbAlias", "message": "..." }
  ]
}
```
