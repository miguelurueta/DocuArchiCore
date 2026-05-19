# SCRUM-205 Integracion Frontend ListaDocumentosRadicados

## 1. Objetivo Frontend
Consumir la lista de documentos relacionados (query) y ejecutar acciones por fila (action) sin cambiar la estructura base de AppTable.

## 2. Seguridad
- Header requerido: `Authorization: Bearer {jwt}`.
- Claims requeridos:
  - `defaulalias`
  - `usuarioid`

## 3. Endpoints
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`

## 4. Parametros de Query (lista documentos relacionados)
### 4.1 Campos obligatorios
| Campo | Tipo | Requerido | Valores | Default sugerido |
|---|---|---|---|---|
| `ViewMode` | string | Si | `hierarchical`, `flatDocuments` | `flatDocuments` |
| `Page` | int | Si | `>= 1` | `1` |
| `PageSize` | int | Si | `1..200` | `25` |
| `SortDir` | string | Si | `ASC`, `DESC` | `ASC` |

### 4.2 Campos opcionales recomendados
| Campo | Tipo | Requerido | Uso |
|---|---|---|---|
| `ColumnMode` | int | No | Modo de columnas dinamicas. |
| `EstadoTramite` | string | No | Filtro funcional por estado. |
| `SearchType` | int | No | Tipo de busqueda. |
| `Search` | string | No | Texto de busqueda libre. |
| `SortField` | string | No | Campo para ordenamiento. |
| `StructuredFilters` | array | No | Filtros avanzados. |
| `IncludeConfig` | bool | No | Incluir config de tabla. |
| `EnablePagination` | bool/null | No | Activa/desactiva paginacion del componente. Si no se envía, backend usa `true` (compatibilidad). |
| `EnableColumnFilters` | bool/null | No | Activa/desactiva busqueda/filtros por columnas. Si no se envía, backend usa `true` (compatibilidad). |
| `ParentRowId` | string/null | Condicional | Requerido para cargar hijos en `hierarchical`. |
| `ParentNodeType` | string/null | Condicional | Tipo del nodo padre en `hierarchical`. |
| `Level` | int | No | Nivel de arbol consultado. |

### 4.3 Regla clave por modo de vista
- `flatDocuments`:
  - enviar `ViewMode: "flatDocuments"`.
  - `ParentRowId` y `ParentNodeType` deben ir `null`.
- `hierarchical`:
  - para raiz usar `ParentRowId: null`, `Level: 1`.
  - para expandir nodo usar `ParentRowId` con el `RowId` del padre y `Level` del siguiente nivel.

### 4.4 Payload minimo recomendado para primera carga
```json
{
  "ViewMode": "flatDocuments",
  "Page": 1,
  "PageSize": 25,
  "SortDir": "ASC",
  "Search": "",
  "StructuredFilters": [],
  "IncludeConfig": true,
  "EnablePagination": false,
  "EnableColumnFilters": false,
  "ParentRowId": null,
  "ParentNodeType": null,
  "Level": 1
}
```

### 4.5 Ejemplo hierarchical (expandir nodo)
```json
{
  "ColumnMode": 2,
  "EstadoTramite": "",
  "SearchType": 1,
  "Search": "",
  "SortField": "",
  "SortDir": "ASC",
  "Page": 1,
  "PageSize": 25,
  "StructuredFilters": [],
  "IncludeConfig": true,
  "ViewMode": "hierarchical",
  "ParentRowId": "node-100",
  "ParentNodeType": "expediente",
  "Level": 2
}
```

## 5. Campos que debe leer el frontend en Query Response
Comportamiento de compatibilidad:
- Si `EnablePagination` no se envía: backend asume `true`.
- Si `EnableColumnFilters` no se envía: backend asume `true`.
- Para desactivar explícitamente:
  - `EnablePagination=false`
  - `EnableColumnFilters=false`

### 5.1 Success
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Rows": [
      {
        "RowId": "doc-15416",
        "Values": {
          "ID": 15416,
          "DBT": 778,
          "PAG": 4,
          "TIPODOCUMENTO": "Factura",
          "ESTADO_FIRMA_DIGITAL": "PENDIENTE"
        },
        "Meta": {
          "NodeType": "documento",
          "ParentId": null,
          "HasChildren": false,
          "CanAddChild": false,
          "CanDelete": true,
          "DocumentId": 15416,
          "NombreGabinete": "WF_DOCS"
        }
      }
    ]
  },
  "meta": { "Status": "success" },
  "errors": []
}
```

### 5.2 Campos minimos de uso UI
- `Rows[].RowId`: id tecnico de fila para acciones.
- `Rows[].Values`: columnas visibles.
- `Rows[].Meta.DocumentId`: id de documento para abrir/eliminar.
- `Rows[].Meta.NombreGabinete`: gabinete para resolver visualizacion.
- `Rows[].Meta.HasChildren`: controlar icono de expandir.
- `Rows[].Meta.CanAddChild` y `Rows[].Meta.CanDelete`: habilitar botones.

### 5.3 Error controlado
```json
{
  "success": false,
  "message": "Request invalido",
  "data": null,
  "meta": { "Status": "validation" },
  "errors": [
    { "errorCode": "CLAIM_INVALID", "errorMessage": "defaulalias requerido" }
  ]
}
```

## 6. Parametros de Action
### 6.1 Campos obligatorios
| Campo | Tipo | Requerido | Valores |
|---|---|---|---|
| `TableId` | string | Si | Id de la tabla en frontend (`InboxListaRadicados`). |
| `ViewMode` | string | Si | `hierarchical`, `flatDocuments`. |
| `ActionId` | string | Si | `ver_documento`, `agregar_item`, `eliminar_item`. |
| `RowId` | string | Si | `RowId` retornado por query. |
| `NodeType` | string | Si | Tipo de nodo (`documento`, `expediente`, etc.). |
| `Payload.IdDocumento` | int | Si | Id numerico del documento (alineado con `/visualizacion/resolve`). |
| `Payload.NombreGabinete` | string | Si | Gabinete del documento. |

### 6.2 Ejemplo ver_documento
```json
{
  "TableId": "InboxListaRadicados",
  "ViewMode": "flatDocuments",
  "ActionId": "ver_documento",
  "RowId": "doc-15416",
  "ParentRowId": null,
  "NodeType": "documento",
  "Payload": {
    "IdDocumento": 15416,
    "NombreGabinete": "WF_DOCS"
  }
}
```
Compatibilidad: tambien se acepta `Payload.DocumentId` para clientes legacy.

## 7. Action Response
### 7.1 Success ver_documento
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Operation": "view",
    "AffectedRowId": "doc-15416",
    "ParentRowId": null,
    "RequiresReloadNode": false,
    "Row": null,
    "DocumentResolveRequest": {
      "NombreGabinete": "WF_DOCS",
      "IdDocumento": 15416
    }
  },
  "meta": { "Status": "success" },
  "errors": []
}
```

### 7.2 Integracion con descarga/visualizacion (sin API->API)
`action/ver_documento` solo devuelve el contrato de resolucion (`DocumentResolveRequest`). La llamada real de visualizacion debe hacerla el frontend directamente a:

- `POST /api/gestor-documental/documentos/visualizacion/resolve`

Request esperado:
```json
{
  "NombreGabinete": "WF_DOCS",
  "IdDocumento": 15416
}
```

Nota de diseno: no se debe implementar consumo de API desde API para este flujo. Si en el futuro se elimina la API generica de `action`, el frontend puede usar directamente los datos de fila (`ID` y `NOMBRE_GABINETE`) para construir la solicitud a `visualizacion/resolve`.

## 8. Errores esperados
- `400`: claims invalidos o payload invalido.
- `200 success=false`: accion no soportada o regla funcional incumplida.
- `500`: error tecnico controlado por `AppResponses`.

## 9. Checklist rapido de consumo (frontend)
1. En query enviar siempre `ViewMode`, `Page`, `PageSize`, `SortDir`.
2. Para lista plana usar `ViewMode=flatDocuments` y `ParentRowId=null`.
3. Guardar en estado de fila: `RowId`, `Meta.DocumentId`, `Meta.NombreGabinete`, `Meta.NodeType`.
4. Para `ver_documento`, invocar desde frontend `POST /api/gestor-documental/documentos/visualizacion/resolve` con `IdDocumento` y `NombreGabinete` de la fila.
5. Si `success=false`, mostrar `errors[0].errorMessage`.
