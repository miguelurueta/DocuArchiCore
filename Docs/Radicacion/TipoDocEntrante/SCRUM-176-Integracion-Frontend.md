# SCRUM-176 - Integracion Frontend

## Endpoint
- Metodo: `GET`
- Ruta: `/api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}`

## Seguridad
- Requiere claim `defaulalias` en sesión/token.

## Request
- Parametro de ruta: `idTipoDocEntrante` (int).

## Response
- Tipo: `AppResponses<TipoDocEntrante>`

## Ejemplo success
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "id_Tipo_Doc_Entrante": 302,
    "Descripcion_Doc": "TRAMITE",
    "util_envio_correo_certificado": 1,
    "util_firma_digital_protocolo_respuesta": 0,
    "util_agrega_digital_protocolo_respuesta": 1
  }
}
```

## Ejemplo error
```json
{
  "success": false,
  "message": "Id de tipo documento entrante requerido",
  "data": null
}
```
