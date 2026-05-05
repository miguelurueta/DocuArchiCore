# SCRUM-183 - Observabilidad Inventario Documental

## Logs esperados
- `requestId`
- `idAlmacen`
- `idRegistroProduccionDocumental`
- `nombreGabinete`
- estado de insercion (aplica/omitido/insertado)

## Datos no logueables
- fulltext completo del documento
- contenido binario
- rutas fisicas completas

## Eventos
- `inventario documental omitido/no aplica`
- `inventario documental build sin insercion`
- `inventario documental insertado`
- errores transaccionales con rollback
