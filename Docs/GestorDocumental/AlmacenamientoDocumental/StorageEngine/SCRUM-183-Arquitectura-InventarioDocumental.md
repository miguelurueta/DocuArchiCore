# SCRUM-183 - Arquitectura Inventario Documental

## Objetivo
Restaurar paridad legacy VB en insercion de `registro_producion_documental`, condicionada por opcion real de gabinete (`INVENTARIO_DOCUMENTAL`).

## Componentes
- `InventarioDocumentalBuilder` (Services): compone modelo de insercion completo.
- `InventarioDocumentalRepository` (Repository): persiste columnas legacy dentro de transaccion activa.
- `StorageTransactionCoordinator` (Services): decide ejecucion por `ResolvedOptions` y propaga `IdRegistroProduccionDocumental`.

## Decisiones
- Inventario solo se inserta si `context.ResolvedOptions.AplicaInventarioDocumental == true`.
- Si inventario no aplica, la transaccion continua sin error ni insercion.
- Si inventario aplica y faltan dependencias/datos requeridos, se corta con error transaccional.
- `FullText` se toma de `Command.FullText` o se consolida desde campos efectivos (incluye preindex integrado).
