# SCRUM-205 Observabilidad ListaDocumentosRadicados

## 1. Objetivo
Definir trazabilidad operativa para query/action de `ListaDocumentosRadicados`, con foco en diagnostico de errores y comportamiento por modo de vista.

## 2. Catalogo de Logs
### 2.1 Information
- Inicio de `query` con `ViewMode`, `Page`, `PageSize`.
- Fin de `query` con `rowCount` y `duracionMs`.
- Inicio de `action` con `ActionId` y `RowId`.
- Fin de `action` con `Operation` y `duracionMs`.
- `ver_documento` con `DocumentId` y `NombreGabinete` (sin datos sensibles).

### 2.2 Warning
- `ActionId` no soportada.
- `ParentRowId` invalido para `hierarchical`.
- Sin resultados en consulta (`message=Sin resultados`).
- Payload incompleto para accion.

### 2.3 Error
- Excepcion en repository/DB.
- Excepcion de mapping en service.
- Excepcion no controlada en controller.

## 3. Campos Minimos de Trazabilidad
- `requestId`
- `usuarioid`
- `alias`
- `ViewMode`
- `ActionId`
- `rowCount`
- `duracionMs`

## 4. Metricas Sugeridas
- Latencia `p50/p95/p99` de `query`.
- Latencia `p50/p95/p99` de `action`.
- Tasa de error por endpoint.
- Tasa de `ActionId` no soportada.
- Distribucion de uso `hierarchical` vs `flatDocuments`.

## 5. Troubleshooting
### 5.1 `query` sin datos
1. Validar claims.
2. Validar filtros de request.
3. Verificar `defaultDbAlias`.
4. Verificar criterios legacy de enlace/radicado.

### 5.2 `action` invalida
1. Revisar `ActionId`.
2. Revisar `NodeType` y `RowId`.
3. Revisar payload requerido para esa accion.

### 5.3 Error DB
1. Revisar `QueryOptions` construidos.
2. Revisar parametrizacion.
3. Revisar conectividad alias DB.

