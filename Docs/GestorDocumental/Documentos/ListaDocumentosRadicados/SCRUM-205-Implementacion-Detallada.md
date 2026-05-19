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

