# SCRUM-202 Integración Frontend Reemplazo PDF

## 1. Objetivo Frontend
Permitir que el frontend suba un PDF modificado por chunks y luego ejecute el reemplazo físico del documento en gabinete.

## 2. Seguridad
- Header obligatorio en todas las APIs: `Authorization: Bearer {jwt}`
- Claims mínimos en JWT:
- `defaulalias`
- `usuarioid`

## 3. APIs del módulo Reemplazo PDF
- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`
- `PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- `GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- `DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`
- `POST /api/gestor-documental/documentos/reemplazopdf`

## 4. Contratos por API
### 4.1 Init upload temporal
`POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`

Request:
```json
{
  "NombreOriginal": "DIG00015416.PDF",
  "TamanoBytes": 251004,
  "Extension": ".PDF",
  "HashSha256Esperado": "opcionalesha256",
  "NumeroChunks": 3
}
```

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "RutaTemporalId": "usr_141_18be1f9f81524358bdf6a78e7f25f2dc",
    "ArchivoTemporalId": "af_0d22cb08fb6b4f16b3916f6759089f49.pdf",
    "ChunkSizeBytes": 1048576,
    "Estado": "IN_PROGRESS"
  },
  "errors": []
}
```

### 4.2 Upload chunk
`PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`

Headers obligatorios:
- `Content-Type: application/octet-stream`
- `Content-Length: {bytesChunk}`
- `X-Total-Chunks: {totalChunks}`

Body:
- Binario puro del chunk (no JSON, no Base64).

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "chunkIndex": 0
  },
  "errors": []
}
```

### 4.3 Status upload temporal
`GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Estado": "IN_PROGRESS",
    "ChunksRecibidos": 2,
    "ChunksPendientes": 1,
    "TamanoRecibidoBytes": 2097152
  },
  "errors": []
}
```

### 4.4 Complete upload temporal
`POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Estado": "COMPLETED"
  },
  "errors": []
}
```

### 4.5 Cancel upload temporal
`DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Estado": "CANCELLED"
  },
  "errors": []
}
```

### 4.6 Reemplazo final PDF
`POST /api/gestor-documental/documentos/reemplazopdf`

Request:
```json
{
  "NombreGabinete": "contabil",
  "IdDocumento": 15416,
  "RutaTemporalId": "usr_141_18be1f9f81524358bdf6a78e7f25f2dc",
  "ArchivoTemporalId": "af_0d22cb08fb6b4f16b3916f6759089f49.pdf",
  "Motivo": "Actualizacion de firma embebida en PDF",
  "DescOp": "REEMPLAZO_PDF_STORAGE_ENGINE",
  "ModuloRegistro": "DOCUARCHI",
  "Radicado": "2600466700019",
  "IdTareaWorkflow": 12873,
  "IdRutaWorkflow": 45,
  "TipologiaDocumental": "FACTURA"
}
```

Response OK:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "IdDocumento": 15416,
    "NombreGabinete": "contabil",
    "RutaArchivoFinal": "D:/imagenes/discos/CONTABIL7/00093/DIG00015416.PDF",
    "RutaRespaldo": "D:/temp/storage-temp/replacement-versions/contabil/15416/20260516091530/DIG00015416.PDF",
    "TamanoAnteriorBytes": 248112,
    "TamanoNuevoBytes": 251004,
    "HashAnteriorSha256": "...",
    "HashNuevoSha256": "...",
    "RequestId": "f5db29fb18b94b27878db78f6743aa52"
  },
  "meta": {
    "Status": "success"
  },
  "errors": []
}
```

## 5. Paso a paso de consumo frontend
1. Seleccionar archivo PDF modificado en front.
2. Calcular `TamanoBytes` y `NumeroChunks` según tamaño y chunk-size que decida frontend.
3. Llamar `init` y guardar `RutaTemporalId` + `ArchivoTemporalId`.
4. Dividir el archivo en chunks binarios.
5. Enviar cada chunk con `PUT .../chunk/{chunkIndex}` y header `X-Total-Chunks`.
6. Al terminar chunks, llamar `complete`.
7. Validar que `Estado` final sea `COMPLETED` (con respuesta de `complete` o `status`).
8. Llamar endpoint final `POST /reemplazopdf` enviando IDs temporales y datos funcionales.
9. Mostrar al usuario `RutaArchivoFinal` y `RequestId`.
10. Si el usuario cancela antes de terminar, invocar `DELETE` de cancelación.

## 6. Campos de auditoría `logdocuarchi`
Campos recomendados cuando aplica flujo/radicación:
- `DescOp`
- `ModuloRegistro`
- `Radicado`
- `IdTareaWorkflow`
- `IdRutaWorkflow`
- `TipologiaDocumental`

Defaults backend si no se envían:
- `DescOp`: `REEMPLAZO_PDF_STORAGE_ENGINE`
- `ModuloRegistro`: `DOCUARCHI`
- `IdTareaWorkflow`: `0`
- `IdRutaWorkflow`: `0`
- `TipologiaDocumental`: se toma de `TIPODOCUMENTO` del gabinete

## 7. Errores esperados
- `400 Validation`: archivo temporal no existe, estado no `COMPLETED`, extensión no PDF, documento de gabinete no existe.
- `500 Error`: fallo no controlado en reemplazo o auditoría.

## 8. Checklist de depuración frontend
- JWT vigente con claims `defaulalias` y `usuarioid`.
- `RutaTemporalId` y `ArchivoTemporalId` del mismo flujo `init`.
- Chunks enviados con `application/octet-stream` y `X-Total-Chunks`.
- `complete` ejecutado antes del `POST /reemplazopdf`.
- Guardar `RequestId` de error o éxito para soporte.
