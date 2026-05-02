# SCRUM-165 Observabilidad ValidationPipeline

## Logs agregados
- `StorageValidationPipeline`: inicio/fin, requestId, alias, usuarioId, gabinete, tipo, cantidadErrores, duracionMs.
- `PreindexValidator`: warning preindex no encontrado.
- `StoragePreindexReader`: info de preindex encontrado (solo nombre de archivo, no ruta completa).
- `GabineteRequiredFieldsValidator`: warnings por campos obligatorios vacios y mismatch.
- `StorageOptionsResolver`/`StorageGabineteMetadataProvider`: info de resolucion.

## Campos clave
- `requestId`
- `usuarioId`
- `alias`
- `nombreGabinete`
- `tipoAlmacenamiento`
- `cantidadErrores`
- `fase=Validation`

## No se loguea
- Contenido de documentos
- Fulltext
- Valores sensibles de indexacion
- Rutas fisicas completas
