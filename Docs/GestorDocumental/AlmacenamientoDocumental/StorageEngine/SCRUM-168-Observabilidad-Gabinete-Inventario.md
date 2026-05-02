# SCRUM-168 Observabilidad Gabinete Inventario

## Logs requeridos por fase
- Inicio de insercion en gabinete.
- Exito de insercion en gabinete.
- Inicio de insercion inventario documental.
- Exito de insercion inventario documental.
- Error de validacion de identificadores dinamicos.
- Error transaccional con rollback.

## Campos minimos recomendados
- `requestId`
- `alias`
- `usuarioId`
- `nombreGabinete`
- `idAlmacen`
- `idInventario`
- `cantidadColumnasDinamicas`
- `numeroFolios`
- `fase`
- `duracionMs`

## Metricas recomendadas
- `storage_gabinete_insert_success_count`
- `storage_gabinete_insert_error_count`
- `storage_inventory_insert_success_count`
- `storage_inventory_insert_error_count`
- `storage_dynamic_columns_count`
- `storage_fulltext_truncated_count`

## Datos que no se deben loguear
- contenido `FullText`.
- valores sensibles de indexacion.
- SQL completo con parametros de usuario.
- rutas fisicas de almacenamiento.

## Troubleshooting
- `NombreGabinete invalido`:
  - revisar regex permitida y metadata de gabinete.
- `Columna dinamica invalida/duplicada`:
  - validar nombres contra patron y deduplicacion case-insensitive.
- `Fallo insert inventario`:
  - confirmar rollback de la transaccion y estado de locks.
- `IdRegistroProduccionDocumental null en respuesta`:
  - verificar que el retorno de `InventarioDocumentalRepository.InsertAsync` se asigne en `StorageTransactionResult`.
