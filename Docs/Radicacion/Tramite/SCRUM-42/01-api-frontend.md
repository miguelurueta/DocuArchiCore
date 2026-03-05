# API y DTOs para Frontend

## Base URL

- `https://<host>/api/radicacion`

## Endpoint 1: Registrar entrante

- Metodo: `POST`
- Ruta: `/registrar-entrante`
- Headers:
  - `Authorization: Bearer <token>`
  - `Content-Type: application/json`

Request DTO:

```json
{
  "idPlantilla": 100,
  "tipoRadicacion": "ENTRANTE",
  "asunto": "Asunto del radicado",
  "remitente": "Nombre remitente",
  "idExpediente": 123,
  "esRelacionado": true,
  "radicadosRelacionados": [1101, 1102],
  "campos": [
    {
      "idDetallePlantillaRadicado": 10,
      "nombreCampo": "Ciudad",
      "valor": "Bogota"
    }
  ]
}
```

Response DTO (`AppResponses<RegistrarRadicacionEntranteResponseDto>`):

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "consecutivoRadicado": "RAD-20260305112233",
    "estadoAsignacion": "Registrado",
    "alertas": [],
    "metadataOperativa": {
      "dbAlias": "DA",
      "idUsuarioRadicador": 55,
      "q09Ejecutado": false
    }
  },
  "errors": []
}
```

## Endpoint 2: Validar entrante

- Metodo: `POST`
- Ruta: `/validar-entrante`

Request DTO:

```json
{
  "idPlantilla": 100,
  "tipoRadicacion": "ENTRANTE",
  "asunto": "Asunto del radicado",
  "remitente": "Nombre remitente"
}
```

Response DTO (`AppResponses<ValidarRadicacionEntranteResponseDto>`):

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "esValido": true,
    "alertas": []
  },
  "errors": []
}
```

## Endpoint 3: Flujo inicial

- Metodo: `GET`
- Ruta: `/flujo-inicial?idTipoTramite=5`

Response DTO (`AppResponses<FlujoInicialDto>`):

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "idTipoTramite": 5,
    "codigoFlujo": "FLUJO-5",
    "actividadInicial": "Radicar"
  },
  "errors": []
}
```

## Notas de integracion frontend

- El backend obtiene `defaultDbAlias` y `usuarioid` desde claims del token.
- En errores de validacion, `success` llega en `false` y `errors` contiene detalle por campo.
- Todos los contratos devuelven `AppResponses<T>`.
