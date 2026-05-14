# SCRUM-171 Integración API Frontend

## Endpoints oficiales (vigente)
Base:
- `/api/gestor-documental/almacenamiento`

Operaciones:
- `POST /upload-temporal/init`
- `PUT /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- `GET /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
- `POST /upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- `DELETE /upload-temporal/{rutaTemporalId}/{archivoTemporalId}`
- `POST /` (almacenamiento final)

## Seguridad y claims
- Todos los endpoints están protegidos con JWT Bearer (`[Authorize]`).
- Claims requeridos para almacenamiento final:
  - `defaulalias`
  - `usuarioid`

## Feature flag
- `FeatureFlags:StorageEngineV2 = true`: ejecuta Storage Engine.
- `FeatureFlags:StorageEngineV2 = false`: retorna `BadRequest` con `meta.status = "feature_disabled"`.

## Contrato upload temporal
### 1) Inicializar sesión
`POST /api/gestor-documental/almacenamiento/upload-temporal/init`

Request:
```json
{
  "NombreOriginal": "plantuml_diagram.pdf",
  "TamanoBytes": 245678,
  "Extension": ".pdf",
  "HashSha256Esperado": null,
  "NumeroChunks": 1
}
```

Response éxito:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "RutaTemporalId": "usr_141_abc123",
    "ArchivoTemporalId": "af_1234567890.pdf",
    "ChunkSizeBytes": 10485760,
    "Estado": "IN_PROGRESS"
  },
  "errors": []
}
```

### 2) Subir chunk binario
`PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`

Headers requeridos:
- `Content-Length` > 0
- `X-Total-Chunks` > 0

Content-Type:
- `application/octet-stream`

Body:
- bytes crudos del chunk (no JSON, no multipart).

### 3) Consultar estado
`GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`

Response:
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Estado": "IN_PROGRESS",
    "ChunksRecibidos": [0],
    "ChunksPendientes": [],
    "TamanoRecibidoBytes": 245678
  },
  "errors": []
}
```

### 4) Completar upload
`POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`

### 5) Cancelar upload
`DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`

## Contrato almacenamiento final
### `POST /api/gestor-documental/almacenamiento`
Request: `AlmacenarDocumentoRequest`

Campos:
- `NombreGabinete` (`string`, requerido)
- `RutaTemporalId` (`string`, requerido)
- `NombreDocumento` (`string`, requerido)
- `RequestId` (`string`, requerido)
- `Documentos` (`DocumentoEntradaDto[]`, requerido)
- `CamposIndexacion` (`CampoIndexacionDto[]`, opcional)
- `Inventario` (`InventarioDocumentalDto`, opcional)
- `Trd` (`TrdStorageDto`, opcional)
- `Expediente` (`ExpedienteStorageDto`, opcional)
- `Workflow` (`WorkflowStorageDto`, opcional)
- `FullText` (`string`, opcional)
- `NumeroPaginasDeclaradas` (`int`, requerido)

`DocumentoEntradaDto`:
- `IdDocumento` (`string`, requerido)
- `ArchivoTemporalId` (`string`, requerido)
- `NombreOriginal` (`string`, opcional)
- `Extension` (`string`, opcional)
- `NumeroPaginas` (`int`, requerido)

`CampoIndexacionDto`:
- `NombreCampo` (`string`, requerido)
- `Valor` (`string`, opcional)
- `EsObligatorio` (`bool`, requerido)

Request mínimo ejemplo:
```json
{
  "NombreGabinete": "CONTABIL",
  "RutaTemporalId": "usr_141_abc123",
  "NombreDocumento": "plantuml_diagram.pdf",
  "RequestId": "rq-196-001",
  "Documentos": [
    {
      "IdDocumento": "DOC-1",
      "ArchivoTemporalId": "af_1234567890.pdf",
      "NombreOriginal": "plantuml_diagram.pdf",
      "Extension": ".pdf",
      "NumeroPaginas": 1
    }
  ],
  "CamposIndexacion": [],
  "NumeroPaginasDeclaradas": 1
}
```

## Contrato response detallado
`AppResponses<AlmacenarDocumentoResponse>`

- `success` (`bool`)
- `message` (`string`)
- `data` (`AlmacenarDocumentoResponse | null`)
- `meta` (`AppMeta | null`)
- `errors` (`object[] | null`)

`AlmacenarDocumentoResponse`:
- `IdAlmacen` (`long`)
- `IdRegistroProduccionDocumental` (`long?`)
- `NombreArchivoFinal` (`string`)
- `RequestId` (`string`)

## Errores y observaciones operativas
- `401` + `www-authenticate: Bearer`: token ausente/inválido.
- En `init`, si faltan campos requeridos (`NombreOriginal`, `Extension`) retorna `400` por deserialización.
- En flujo final, si claim `usuarioid` no existe o es inválido retorna error de validación/seguridad.
- Errores funcionales de upload y validación viajan en `AppResponses` con `meta.status = "validation"`.

## Resumen Frontend
- El binario solo viaja por endpoint de chunk (`octet-stream`).
- `POST /almacenamiento` recibe JSON con referencias temporales (`RutaTemporalId`, `ArchivoTemporalId`).
- No enviar body envuelto en `{ "request": { ... } }`; el payload va directo en la raíz.

## Cambios internos recientes (sin cambio de contrato HTTP)
- La clasificación técnica de archivo para almacenamiento usa `DA_EXTENSION` por extensión real del documento.
- El flujo transaccional inserta gabinete dinámico + inventario + índice lógico en una sola transacción cuando aplica.
- En caso de falla física post-commit, se mantiene compensación DB.
