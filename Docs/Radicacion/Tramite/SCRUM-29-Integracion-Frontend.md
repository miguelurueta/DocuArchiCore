# SCRUM-29 - Guia de Integracion Frontend

## Endpoint

- Metodo: `GET`
- URL: `/api/tramite/tramites/solicitaFechaLimiteRespuesta`
- Query params:
- `idTipoTramite` (int, requerido)

## Contrato de respuesta

```json
{
  "success": true,
  "message": "OK",
  "errors": [],
  "data": {
    "IdTipoTramite": 200,
    "IdPlantilla": 100,
    "DiasVencimiento": 5,
    "FechaLimiteRespuesta": "2026-03-05"
  }
}
```

## DTO

`FechaLimiteRespuestaDto`

- `IdTipoTramite`: identificador del tramite solicitado.
- `IdPlantilla`: plantilla default usada para la consulta.
- `DiasVencimiento`: numero de dias habiles base.
- `FechaLimiteRespuesta`: fecha limite calculada en formato `yyyy-MM-dd`.

## Manejo de estados esperados

- `success=true` + `message="OK"`: hay fecha calculada.
- `success=true` + `message="Sin resultados"` + `data=null`: no hay configuracion para calcular.
- `success=false`: error controlado con arreglo `errors`.
