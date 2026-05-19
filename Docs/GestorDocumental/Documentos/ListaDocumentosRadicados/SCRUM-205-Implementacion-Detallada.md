# SCRUM-205 Implementacion Detallada ListaDocumentosRadicados

## 1. Resumen
Este documento describe la implementacion esperada por capa para `ListaDocumentosRadicados`, incluyendo contratos, wiring y reglas de migracion legacy.

## 2. Archivos esperados por repositorio
### 2.1 DocuArchi.Api
- `Controllers/GestorDocumental/Documentos/ListaDocumentosRadicadoController.cs`
- `Program.cs` (registro DI)

### 2.2 MiApp.Services
- `Service/GestorDocumental/Documentos/ListaDocumentosRadicadoService.cs`

### 2.3 MiApp.Repository
- `Repositorio/GestorDocumental/Documentos/ListaDocumentosRadicados/...`

### 2.4 MiApp.DTOs
- `DTOs/GestorDocumental/Documentos/ListaDocumentosRadicados/...`

### 2.5 DocuArchiCore (coordinador)
- `openspec/changes/scrum-205-crea-api-lista-documentos-radicados/*`
- `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/*`

## 3. Firmas objetivo
### 3.1 Service Query
`Task<AppResponses<object>> SolicitaListaDocumentosRadicadosTreeAsync(ListaDocumentosRadicadosTreeQueryRequestDto req, int idUsuarioGestion, string defaultDbAlias)`

### 3.2 Service Action
`Task<AppResponses<ListaDocumentosRadicadosTreeMutationResultDto?>> EjecutaAccionListaDocumentosRadicadosTreeAsync(ListaDocumentosRadicadosTreeActionRequestDto req, int idUsuarioGestion, string defaultDbAlias)`

## 4. DI esperado
- Services en `Program.cs` bajo `// Services (L)` como `Scoped`.
- Repositories en `Program.cs` bajo `// Repositories (R)` como `Scoped`.
- Mantener patron actual de interfaces en mismo archivo cuando aplique.

## 5. AutoMapper
- Registrar mapeos para:
  - resultado repository -> fila dinamica (`Values`/`Meta`)
  - payload `action` -> `ListaDocumentosRadicadosTreeMutationResultDto`
  - metadata para `DocumentResolveRequest` en `ver_documento`

## 6. Estrategia DapperCrudEngine + QueryOptions
- Definir `QueryOptions` con filtros parametrizados por:
  - criterios de radicado/enlace
  - paginacion (`Page`, `PageSize`)
  - orden (`SortField`, `SortDir`) validado
- Evitar query manual y concatenacion SQL.

## 7. Mapping Legacy -> Contrato Nuevo
| Legacy | Values | Meta |
|---|---|---|
| `ID` | `Values.ID` | `Meta.DocumentId` |
| `DBT` | `Values.DBT` | `Meta.DocumentId` (derivado segun regla) |
| `PAG` | `Values.PAG` | `Meta.HasChildren` (regla de negocio) |
| `TIPODOCUMENTO` | `Values.TIPODOCUMENTO` | `Meta.NodeType` (si aplica) |
| `ESTADO_FIRMA_DIGITAL` | `Values.ESTADO_FIRMA_DIGITAL` | `Meta.CanDelete` / acciones disponibles |

Nota: la semantica final de `Meta.*` debe quedar alineada con reglas funcionales reales del modulo.

## 8. Reglas de implementacion por ActionId
- `ver_documento`:
  - no crea preview nuevo
  - retorna `DocumentResolveRequest`
  - `Operation=view`
- `agregar_item`:
  - aplica mutacion controlada
  - `Operation=added`
- `eliminar_item`:
  - aplica mutacion controlada
  - `Operation=deleted`

## 9. Control de errores
- Todo metodo critico con `try/catch`.
- Salida uniforme con `AppResponses<T>`.
- Sin exponer stacktrace en respuesta publica.

## 10. Ejemplo tecnico de consumo (Query)
Endpoint:
`POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`

Payload de ejemplo:
```json
{
  "ColumnMode": 2,
  "EstadoTramite": "",
  "SearchType": 1,
  "Search": "",
  "SortField": "ID",
  "SortDir": "ASC",
  "Page": 1,
  "PageSize": 25,
  "StructuredFilters": [
    {
      "Field": "TIPODOCUMENTO",
      "Operator": "contains",
      "Value": "Factura",
      "ValueFrom": null,
      "ValueTo": null
    }
  ],
  "IncludeConfig": true,
  "ViewMode": "flatDocuments",
  "ParentRowId": null,
  "ParentNodeType": null,
  "Level": 1,
  "TableId": "InboxListaDocumentosRadicado",
  "NombreGabinete": "CORRESPO",
  "CampoRadicado": "",
  "Radicado": "2500466700035",
  "AplicaTrd": 1
}
```

Notas de implementacion para este ejemplo:
- `CampoRadicado` vacio aplica fallback a `ENLASE`.
- `StructuredFilters[].Operator="contains"` no se aplica en repository actual (solo `=`, `eq`, `equals`); el filtro estructurado se ignora y la consulta usa filtros validos restantes.
- `ViewMode="flatDocuments"` mantiene salida plana y usa `Rows` con `Values`/`Meta` segun contrato actual.
