# SCRUM-205 Integracion Frontend ListaDocumentosRadicados

## 1. Objetivo Frontend
Consumir listado dinamico de documentos radicados y ejecutar acciones por fila sin cambios estructurales de AppTable existente.

## 2. Seguridad
- Header requerido: `Authorization: Bearer {jwt}`.
- Claims requeridos:
  - `defaulalias`
  - `usuarioid`

## 3. Endpoints
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`

## 4. Query Request
### 4.1 Ejemplo hierarchical
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

### 4.2 Ejemplo flatDocuments
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
  "ViewMode": "flatDocuments",
  "ParentRowId": null,
  "ParentNodeType": null,
  "Level": 1
}
```

## 5. Query Response
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

### 5.2 Error controlado
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

## 6. Action Request
### 6.1 ver_documento
```json
{
  "TableId": "workflowInboxgestion",
  "ViewMode": "flatDocuments",
  "ActionId": "ver_documento",
  "RowId": "doc-15416",
  "ParentRowId": null,
  "NodeType": "documento",
  "Payload": {
    "DocumentId": 15416,
    "NombreGabinete": "WF_DOCS"
  }
}
```

### 6.2 agregar_item
```json
{
  "TableId": "workflowInboxgestion",
  "ViewMode": "hierarchical",
  "ActionId": "agregar_item",
  "RowId": "node-100",
  "ParentRowId": "node-100",
  "NodeType": "expediente",
  "Payload": {
    "DocumentId": 0,
    "NombreGabinete": "WF_DOCS"
  }
}
```

### 6.3 eliminar_item
```json
{
  "TableId": "workflowInboxgestion",
  "ViewMode": "flatDocuments",
  "ActionId": "eliminar_item",
  "RowId": "doc-15416",
  "ParentRowId": null,
  "NodeType": "documento",
  "Payload": {
    "DocumentId": 15416,
    "NombreGabinete": "WF_DOCS"
  }
}
```

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

### 7.2 Success agregar/eliminar
```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Operation": "added",
    "AffectedRowId": "doc-20001",
    "ParentRowId": "node-100",
    "RequiresReloadNode": true,
    "Row": {},
    "DocumentResolveRequest": null
  },
  "meta": { "Status": "success" },
  "errors": []
}
```

## 8. Errores esperados
- `400`: claims invalidos o payload invalido.
- `200 success=false`: accion no soportada o regla funcional incumplida.
- `500`: error tecnico controlado por `AppResponses`.

