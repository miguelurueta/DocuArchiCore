# SCRUM-62 - Integracion Frontend

## Endpoint

- Metodo: `GET`
- URL: `/api/workflow/ruta-trabajo/solicita-existencia-radicado`

## Parametros

- `consecutivoRadicado` (query string, requerido)
- `nombreRuta` (query string, requerido)
- `defaulalias` (claim requerido, resuelto por backend)

## Response DTO

```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2600010000001",
    "IdTareaWorkflow": 12345,
    "EstadoExistenciaRadicado": "YES"
  },
  "errors": []
}
```

## Caso sin registro

```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2600010000001",
    "IdTareaWorkflow": 0,
    "EstadoExistenciaRadicado": "NO"
  },
  "errors": []
}
```
