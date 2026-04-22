# SCRUMCORE-144 — Integración Frontend

## Endpoint

`GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf={idTareaWf}`

## Contrato

Tipo:
- `AppResponses<List<RaRespuestaRadicado>>`

Fuente canónica:
- `meta.status`

Estados:
- `success`: renderizar `GestionRespuesta`
- `empty`: bloqueo definitivo (no renderizar detalle luego)
- `pending`: skeleton/loading estable (sin bloqueo definitivo)

## Fallback legacy (obligatorio)

Si `meta.status` no existe:
- `success && data.length > 0` => `success`
- `success && data.length == 0` => `empty`

## Ejemplos

### Con datos

```json
{
  "success": true,
  "message": "YES",
  "data": [{ "IdTareaWf": 934 }],
  "meta": { "status": "success" },
  "errors": []
}
```

### Vacío definitivo

```json
{
  "success": true,
  "message": "Sin resultados",
  "data": [],
  "meta": { "status": "empty" },
  "errors": []
}
```
