# SCRUM-208 Integracion Frontend

## Endpoint
`GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`

## Request ejemplo
`GET /api/gestor-documental/documentos/12345/firma-electronica?nombreGabinete=CORRESPO`

## Response firmado
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "IdArchivo": 12345,
    "NombreGabinete": "CORRESPO",
    "FirmadoElectronico": true,
    "IdCertificado": 9876
  },
  "meta": { "Status": "success" },
  "errors": []
}
```

## Response no firmado
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "IdArchivo": 12345,
    "NombreGabinete": "CORRESPO",
    "FirmadoElectronico": false,
    "IdCertificado": 0
  },
  "meta": { "Status": "success" },
  "errors": []
}
```
