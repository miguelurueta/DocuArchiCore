# SCRUM-176 - Integracion Frontend

## Endpoint
- Metodo: `GET`
- Ruta: `/api/radicacion/tramite/tipo-doc-entrante/{idTipoDocEntrante}`

## Seguridad
- Requiere claim `defaulalias` en sesión/token.

## Request
- Parametro de ruta: `idTipoDocEntrante` (int).

## Response
- Tipo: `AppResponses<TipoDocEntranteParametroDto>`

## Ejemplo success
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "IdTipoDocEntrante": 302,
    "DescripcionDoc": "TRAMITE",
    "UtilEnvioCorreoCertificado": 1,
    "UtilFirmaDigitalProtocoloRespuesta": 0,
    "UtilAgregaDigitalProtocoloRespuesta": 1
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
