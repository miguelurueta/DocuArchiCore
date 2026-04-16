# SCRUM-137 — Integración Frontend

## Objetivo

Consultar la estructura de respuestas asociadas a una tarea de workflow, filtrando únicamente por `ID_TAREA_WF`.

## Endpoint

- Método: `GET`
- Ruta: `/api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea`

### Querystring

- `idTareaWf` (requerido, `number/long`)

Ejemplo:

`GET /api/gestor-documental/solicita-estructura-respuesta-id-tarea?idTareaWf=12345`
`GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf=12345`

## Seguridad / Claims

- Claim requerido: `defaulalias`
- Nota: el frontend **no** envía este claim manualmente; viaja en el mecanismo de autenticación/sesión vigente (token/cookie/etc).

## Respuesta

Tipo:

- `AppResponses<List<RaRespuestaRadicado>>`

### Semántica

- `success = true` y `data` con elementos: consulta exitosa con resultados.
- `success = true` y `data = []`: consulta exitosa sin resultados (`message = "Sin resultados"`).
- `success = false`: error controlado (validación o fallo controlado en capa service/repository).

### Ejemplo con datos

```json
{
  "success": true,
  "message": "YES",
  "data": [
    {
      "idRespuestaRadicado": 1,
      "idTareaWf": 12345,
      "radicado": "2025-0001",
      "asunto": "Respuesta generada"
    }
  ],
  "errors": []
}
```

### Ejemplo sin datos

```json
{
  "success": true,
  "message": "Sin resultados",
  "data": [],
  "errors": []
}
```

### Ejemplo de error

```json
{
  "success": false,
  "message": "IdTareaWf requerido",
  "data": [],
  "errors": [
    {
      "type": "Validation",
      "field": "idTareaWf",
      "message": "IdTareaWf requerido"
    }
  ]
}
```

## Guía de consumo (UI)

- Consumir como consulta puntual por `idTareaWf`.
- Tratar `data` como **lista** (puede ser vacía).
- Manejar explícitamente el estado vacío (`data = []`) sin asumir que siempre hay registros.
- No usar `message` para lógica; usar `success` + `data.length`.
