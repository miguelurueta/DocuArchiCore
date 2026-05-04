# SCRUM-179 — Observabilidad Naming Legacy

## Puntos observables

### Planificación física
- Componente: `StoragePlanBuilder`
- Señales:
  - error por `IdTipoDocumento` faltante.
  - error por extensión no resoluble.

### Escritura de XML físico
- Componente: `StorageXmlWriter`
- Señales:
  - conflicto por XML existente.
  - confirmación de nombre XML `FXL########.xml`.

### Fase física completa
- Componente: `StoragePhysicalPhaseExecutor`
- Señales:
  - `inicio fase fisica requestId=...`
  - `fase fisica completada requestId=... idAlmacen=...`
  - `error fase fisica requestId=...` + compensación.

## Errores funcionales esperados
- `IdTipoDocumento requerido para naming legacy`
- `No se pudo determinar extensión para tipo X`
- `XML destino ya existe: FXL########.xml`

## Recomendación operativa
- Monitorear tasa de fallos por resolución de extensión para detectar inconsistencias en `DA_EXTENSION`.
- Incluir `requestId`, `idAlmacen`, `nombreGabinete` y `tipoDocumento` en dashboards de trazas.

