# API y DTOs para Frontend (actualizado)

Fecha de corte: `2026-03-08`

## Base URL

- `https://<host>/api/radicacion`

## Endpoint 1: Registrar entrante

- Metodo: `POST`
- Ruta: `/registrar-entrante`
- Content-Type: `application/json`

Nota de seguridad actual:
- El controlador tiene `[Authorize]` comentado.
- Hoy se esta enviando `idUsuarioGestion=141` y `defaultDbAlias="DA"` de forma fija desde API.
- `Authorization: Bearer <token>` no es obligatorio en el estado actual del endpoint.

Request DTO real (`RegistrarRadicacionEntranteRequestDto`):

```json
{
  "idPlantilla": 100,
  "ASUNTO": "Asunto del radicado",
  "remitente": {
    "nombre": "Nombre remitente",
    "id_Dest_Ext": 0
  },
  "destinatario": {
    "destinatario": "Nombre destinatario",
    "id_Remit_Dest_Int": 141
  },
  "tipo_tramite": {
    "descripcion": "Peticion",
    "tipo_doc_entrante": 5
  },
  "RE_flujo_trabajo": {
    "nombreFlujo": "Flujo principal",
    "id_tipo_flujo_workflow": 1
  },
  "tipoRadicado": {
    "tipoRadicacion": "PQR",
    "idTipoRadicado": 2
  },
  "tipoPlantillaRadicado": {
    "tipoPlantillaRadicado": "ENTRANTE",
    "idTipoPlantillaRdicado": 1
  },
  "expedienteRelacionado": {
    "expediente": "EXP-2026-0001",
    "idExpediente": 123
  },
  "radicadoRelacionados": [
    {
      "consecutivoRelacionadohijo": "260001000100001",
      "idregistroradicadohijo": 0,
      "idplantillahijo": 0
    }
  ],
  "ANEXOS_COR": "Soportes PDF",
  "FECHALIMITERESPUESTA": "2026-03-30",
  "numeroFolios": 12,
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
    "consecutivoRadicado": "260001000100001",
    "estadoAsignacion": "Registrado",
    "alertas": [],
    "metadataOperativa": {
      "dbAlias": "DA",
      "idUsuarioRadicador": 55,
      "idUsuarioGestion": 141,
      "moduloRegistro": "RADICACION",
      "q09Ejecutado": true
    }
  },
  "errors": []
}
```

Reglas de ejecucion internas que impactan el registro:
- Si `citaEstructuraTipoDoEntrante.requiere_respuesta == 1`, ejecuta Q06 y Q07.
- Si `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio == 2` o `3`, ejecuta Q08.
- Si `tipoRadicado.tipoRadicacion == "PQR"`, ejecuta Q09.

Errores esperados:
- HTTP `400` con `AppResponses.success=false`.
- Validaciones con `errors[].Type = "Validation"`.
- Fallos transaccionales con `errors[].Type = "Technical"` y codigo `RAD_TXN_Qxx`.

## Endpoint 2: Validar entrante

- Metodo: `POST`
- Ruta: `/validar-entrante`

Request DTO real (`ValidarRadicacionEntranteRequestDto`):

```json
{
  "idPlantilla": 100,
  "tipoRadicacion": "ENTRANTE",
  "asunto": "Asunto del radicado",
  "remitente": {
    "nombre": "Nombre remitente",
    "id_Dest_Ext": 0
  }
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

- Todos los contratos devuelven `AppResponses<T>`.
- `errors` esta tipado como `object[]`; validar por `Type`, `Field` y `Message` al renderizar.
- El backend normaliza parte del request (canonicalizacion), pero frontend debe enviar la estructura completa del DTO para evitar ambiguedades.
