# SCRUM-207 - Integracion Frontend

## Objetivo

Exponer dos endpoints para resolver metadatos de gabinete de un documento workflow sin que frontend envíe `nombreRuta`.

El backend resuelve internamente:

1. Ruta workflow activa (`Nombre_Ruta`).
2. Tabla dinámica `dat_adic_tar{Nombre_Ruta}`.
3. `ID_GABINETE` y `Nombre_Gabinete`.

## Endpoints

1. `GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`
2. `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`

## Claims Requeridos

1. `defaulaliaswf` (obligatorio).
2. `defaulalias` (opcional para consulta de `configuracion_gabinete`; si no existe se usa fallback a `defaulaliaswf`).

## Contrato de Respuesta

```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 98765,
    "IdGabinete": 12,
    "NombreGabinete": "CORRESPO",
    "EstadoExistenciaRadicado": "YES"
  },
  "meta": {
    "Total": 0,
    "Page": 0,
    "PageSize": 0,
    "Status": "",
    "RetryAfterMs": null
  },
  "errors": []
}
```

`EstadoExistenciaRadicado`:

1. `YES` cuando existe fila en `dat_adic_tar{ruta}`.
2. `NO` cuando no existe fila.

## Ejemplos Para Swagger

### 1) Por Radicado

Request:

```http
GET /api/workflow/ruta-trabajo/radicados/2500466700035/gabinete
Authorization: Bearer {token}
```

Response éxito (encontrado):

```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 98765,
    "IdGabinete": 12,
    "NombreGabinete": "CORRESPO",
    "EstadoExistenciaRadicado": "YES"
  },
  "meta": {
    "Total": 0,
    "Page": 0,
    "PageSize": 0,
    "Status": "",
    "RetryAfterMs": null
  },
  "errors": []
}
```

Response éxito (no encontrado):

```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 0,
    "IdGabinete": 0,
    "NombreGabinete": "",
    "EstadoExistenciaRadicado": "NO"
  },
  "meta": {
    "Total": 0,
    "Page": 0,
    "PageSize": 0,
    "Status": "",
    "RetryAfterMs": null
  },
  "errors": []
}
```

### 2) Por IdTareaWorkflow

Request:

```http
GET /api/workflow/ruta-trabajo/tareas/98765/gabinete
Authorization: Bearer {token}
```

Response éxito (encontrado):

```json
{
  "success": true,
  "message": "YES",
  "data": {
    "Radicado": "2500466700035",
    "IdTareaWorkflow": 98765,
    "IdGabinete": 12,
    "NombreGabinete": "CORRESPO",
    "EstadoExistenciaRadicado": "YES"
  },
  "meta": {
    "Total": 0,
    "Page": 0,
    "PageSize": 0,
    "Status": "",
    "RetryAfterMs": null
  },
  "errors": []
}
```

Response error validación (`idTareaWorkflow <= 0`):

```json
{
  "success": false,
  "message": "IdTareaWorkflow requerido",
  "data": {
    "Radicado": "",
    "IdTareaWorkflow": 0,
    "IdGabinete": 0,
    "NombreGabinete": "",
    "EstadoExistenciaRadicado": "NO"
  },
  "meta": {
    "Total": 0,
    "Page": 0,
    "PageSize": 0,
    "Status": "",
    "RetryAfterMs": null
  },
  "errors": [
    {
      "Type": "Validation",
      "Field": "idTareaWorkflow",
      "Message": "IdTareaWorkflow requerido"
    }
  ]
}
```

## Comportamiento Esperado En Frontend

1. Si `success=true` y `EstadoExistenciaRadicado=YES`:
   - usar `NombreGabinete`, `IdGabinete`, `IdTareaWorkflow`.
2. Si `success=true` y `EstadoExistenciaRadicado=NO`:
   - tratar como "sin registro en ruta workflow".
3. Si `success=false`:
   - mostrar mensaje técnico controlado (`message`) o detalle de `errors`.

