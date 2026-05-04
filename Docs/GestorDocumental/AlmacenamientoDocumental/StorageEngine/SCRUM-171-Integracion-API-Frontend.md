# SCRUM-171 Integración API Frontend

## Endpoint oficial
- Método: `POST`
- Ruta: `/api/gestor-documental/almacenamiento`
- Request: `AlmacenarDocumentoRequest`
- Response: `AppResponses<AlmacenarDocumentoResponse>`

## Claims requeridos
- `defaulalias`
- `usuarioid`

## Feature flag
- `FeatureFlags:StorageEngineV2 = true`: ejecuta Storage Engine.
- `FeatureFlags:StorageEngineV2 = false`: retorna `BadRequest` con `meta.status = "feature_disabled"`.

## Contrato request detallado
`AlmacenarDocumentoRequest`

- `NombreGabinete` (`string`, requerido): nombre lógico de gabinete destino.
- `RutaTemporalId` (`string`, requerido): id de carpeta temporal base.
- `NombreDocumento` (`string`, requerido): nombre destino del documento.
- `RequestId` (`string`, requerido): correlación/idempotencia lógica.
- `Documentos` (`DocumentoEntradaDto[]`, requerido): lista de documentos de entrada.
- `CamposIndexacion` (`CampoIndexacionDto[]`, opcional): campos de indexación.
- `Inventario` (`InventarioDocumentalDto`, opcional).
- `Trd` (`TrdStorageDto`, opcional).
- `Expediente` (`ExpedienteStorageDto`, opcional).
- `Workflow` (`WorkflowStorageDto`, opcional).
- `FullText` (`string`, opcional).
- `NumeroPaginasDeclaradas` (`int`, requerido).
- `TipoAlmacenamiento` (`int`, requerido, default `2`).
- `EvaluarCamposObligatorios` (`bool`, requerido).

`DocumentoEntradaDto`
- `IdDocumento` (`string`, requerido)
- `ArchivoTemporalId` (`string`, requerido)
- `NombreOriginal` (`string`, opcional)
- `Extension` (`string`, opcional)
- `NumeroPaginas` (`int`, requerido)

`CampoIndexacionDto`
- `NombreCampo` (`string`, requerido)
- `Valor` (`string`, opcional)
- `EsObligatorio` (`bool`, requerido)

`InventarioDocumentalDto`
- `IdUsuarioGestion` (`int`)
- `IdEmpresa` (`int`)
- `Radicado` (`string`, opcional)

`TrdStorageDto`
- `IdArea`, `IdSerie`, `IdSubSerie`, `IdTipoDocumento` (`int?`)
- `NombreArea`, `NombreSerie`, `NombreSubSerie`, `NombreTipoDocumento` (`string?`)

`ExpedienteStorageDto`
- `IdExpediente`, `IdTipoExpediente`, `IdUnidadConservacion`, `IdClaseDocumento` (`int?`)

`WorkflowStorageDto`
- `IdTareaWorkflow` (`long?`)
- `IdRutaWorkflow` (`int?`)

## Contrato response detallado
`AppResponses<AlmacenarDocumentoResponse>`

- `success` (`bool`): éxito/fallo funcional.
- `message` (`string`): mensaje de negocio.
- `data` (`AlmacenarDocumentoResponse | null`):
  - `IdAlmacen` (`long`)
  - `IdRegistroProduccionDocumental` (`long?`)
  - `NombreArchivoFinal` (`string`)
  - `RequestId` (`string`)
- `meta` (`AppMeta | null`):
  - `Total`, `Page`, `PageSize` (`int`)
  - `Status` (`string?`): ej. `feature_disabled`, `validation`, `error`
  - `RetryAfterMs` (`int?`)
- `errors` (`AppError[] | object[] | null`):
  - `Type` (`string`)
  - `Field` (`string?`)
  - `Message` (`string`)

## Ejemplo completo request
```json
{
  "NombreGabinete": "GAB_PQRS_2026",
  "RutaTemporalId": "TMP-REQ-00045",
  "NombreDocumento": "pqrs_2026_00045.pdf",
  "RequestId": "REQ-STORAGE-2026-00045",
  "Documentos": [
    {
      "IdDocumento": "DOC-1",
      "ArchivoTemporalId": "tmp-file-001",
      "NombreOriginal": "soporte.pdf",
      "Extension": "pdf",
      "NumeroPaginas": 3
    }
  ],
  "CamposIndexacion": [
    {
      "NombreCampo": "RADICADO",
      "Valor": "2026-00045",
      "EsObligatorio": true
    }
  ],
  "Inventario": {
    "IdUsuarioGestion": 102,
    "IdEmpresa": 1,
    "Radicado": "2026-00045"
  },
  "Trd": {
    "IdArea": 10,
    "IdSerie": 12,
    "IdSubSerie": 19,
    "IdTipoDocumento": 5,
    "NombreArea": "Atencion Ciudadano",
    "NombreSerie": "PQRS",
    "NombreSubSerie": "Respuesta",
    "NombreTipoDocumento": "Comunicacion"
  },
  "Expediente": {
    "IdExpediente": 345,
    "IdTipoExpediente": 2,
    "IdUnidadConservacion": 7,
    "IdClaseDocumento": 4
  },
  "Workflow": {
    "IdTareaWorkflow": 900001,
    "IdRutaWorkflow": 33
  },
  "FullText": "Texto indexable del documento",
  "NumeroPaginasDeclaradas": 3,
  "TipoAlmacenamiento": 2,
  "EvaluarCamposObligatorios": true
}
```

## Ejemplo completo response éxito
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "IdAlmacen": 120045,
    "IdRegistroProduccionDocumental": 778899,
    "NombreArchivoFinal": "000120045.pdf",
    "RequestId": "REQ-STORAGE-2026-00045"
  },
  "meta": null,
  "errors": []
}
```

## Cómo viajan las imágenes Front -> Backend
En `POST /api/gestor-documental/almacenamiento` **no viaja el binario** del archivo.  
El binario viaja antes, en una fase de carga temporal/editor, y en Storage solo viajan referencias.

### Fase 1: subir archivo (binario)
- Método típico: `multipart/form-data`
- Ejemplo existente en la solución: `POST /api/gestor-documental/editor/guardar-imagen`
- Campo form-data: `File`
- Resultado: backend retorna un identificador técnico (`imageUid`/id temporal según flujo usado).

Ejemplo `curl` de subida binaria:
```bash
curl -X POST "https://{host}/api/gestor-documental/editor/guardar-imagen" \
  -H "Authorization: Bearer {token}" \
  -F "File=@C:/tmp/soporte.pdf"
```

Ejemplo de respuesta de subida:
```json
{
  "success": true,
  "message": "YES",
  "data": {
    "imageId": 123,
    "imageUid": "b0c1f720b7af4c7c9c4f73f9b4f1b0aa",
    "fileName": "soporte.pdf",
    "contentType": "application/pdf",
    "fileSize": 53210,
    "storageTypeCode": "db",
    "publicUrl": null
  },
  "meta": null,
  "errors": []
}
```

### Fase 2: registrar almacenamiento (referencias)
Con los IDs temporales ya obtenidos, el frontend llama:
- `POST /api/gestor-documental/almacenamiento`
- `RutaTemporalId`: lote/carpeta temporal
- `Documentos[].ArchivoTemporalId`: ID de cada archivo temporal

Ejemplo end-to-end mínimo:
```json
{
  "NombreGabinete": "GAB_PQRS_2026",
  "RutaTemporalId": "TMP-REQ-00045",
  "NombreDocumento": "pqrs_2026_00045.pdf",
  "RequestId": "REQ-STORAGE-2026-00045",
  "Documentos": [
    {
      "IdDocumento": "DOC-1",
      "ArchivoTemporalId": "tmp-file-001",
      "NombreOriginal": "soporte.pdf",
      "Extension": "pdf",
      "NumeroPaginas": 3
    }
  ],
  "NumeroPaginasDeclaradas": 3,
  "TipoAlmacenamiento": 2,
  "EvaluarCamposObligatorios": true
}
```

Resumen operativo:
- Binario: viaja por `multipart/form-data` en endpoint de carga.
- Storage final: viaja por JSON con referencias (`RutaTemporalId` + `ArchivoTemporalId`).

## Ejemplo response feature deshabilitado
```json
{
  "success": false,
  "message": "StorageEngineV2 deshabilitado y no existe adaptador legacy configurado",
  "data": null,
  "meta": {
    "status": "feature_disabled"
  },
  "errors": [
    {
      "type": "FeatureFlag",
      "field": "StorageEngineV2",
      "message": "StorageEngineV2 = false"
    }
  ]
}
```

## Ejemplo response validación de claims
```json
{
  "success": false,
  "message": "No se encontró el claim 'defaulalias'.",
  "data": null,
  "meta": null,
  "errors": [
    {
      "type": "Claim",
      "field": "defaulalias",
      "message": "El claim 'defaulalias' es requerido para continuar."
    }
  ]
}
```

## Recomendaciones Frontend
- Tratar `feature_disabled` como estado funcional, no como caída técnica.
- No asumir rollback DB en todo error: revisar `meta.status` y mensaje.
- Mostrar siempre `RequestId` al usuario técnico/soporte para trazabilidad.
