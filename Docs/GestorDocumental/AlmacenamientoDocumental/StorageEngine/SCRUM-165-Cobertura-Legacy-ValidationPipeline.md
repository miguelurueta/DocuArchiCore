# SCRUM-165 Cobertura Legacy ValidationPipeline

## Mapeo Legacy -> Nuevo componente
- Preindex legacy (`.xmls/.txt`) -> `IStoragePreindexReader` + `PreindexValidator`
- Campos obligatorios legacy -> `IStorageGabineteMetadataProvider` + `GabineteRequiredFieldsValidator`
- Verifica inventario documental -> `IStorageOptionsResolver` + `StorageOptionsValidator`
- Verifica TRD -> `TrdRulesValidator`
- Verifica unidad/expediente -> `ExpedienteUnidadRulesValidator`

## Reglas no migradas en esta fase
- Estado real de expediente/unidad en DB: pendiente fases posteriores.
- Persistencia/operacion transaccional: fuera de Prompt 4.
- Generacion XML y copia física: fuera de alcance.
