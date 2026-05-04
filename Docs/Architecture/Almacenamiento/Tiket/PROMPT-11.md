# PROMPT ARQUITECTONICO - ORCHESTRATOR / ENGINE
# PROMPT 11 - Paridad Legacy de Metadata Gabinete + Campos Obligatorios
# (FASE CRITICA - BLOQUEANTE FUNCIONAL)

## ROL ESPERADO
Arquitecto Master Backend .NET experto en DocuArchi legacy, metadata dinamica por gabinete, Dapper/QueryOptions, validacion de campos obligatorios, integracion preindex y migracion exacta VB -> C#.

## OBJETIVO
Restaurar paridad funcional en:
- Consulta real de metadata del gabinete (DETALLE_GABIENETE)
- Obtencion de campos obligatorios
- Validacion contra preindex
- Alineacion entre metadata y valores
- Integracion de valores al flujo de persistencia

## FUNCIONES LEGACY REFERENCIA
- Consulta_Campos_Obligatorio
- Almacenamiento (bloque de validacion de matrices y obligatorios)

## UBICACION
- MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Metadata/
- MiApp.Repository/Repositorio/GestorDocumental/AlmacenamientoDocumental/GabineteMetadata/

## INSTRUCCION TECNICA
Implementar:
1. IStorageGabineteMetadataRepository + StorageGabineteMetadataRepository (consulta real DB).
2. IStorageGabineteMetadataProvider + StorageGabineteMetadataProvider (ordenado por campo Orden).
3. IStorageRequiredFieldsValidator + StorageRequiredFieldsValidator.
4. Integracion en GabineteRequiredFieldsValidator.
5. Eliminar placeholders actuales de metadata.

## REGLAS CRITICAS
- Orden exacto obligatorio.
- Cantidad de campos debe coincidir.
- Nombre de campo debe coincidir.
- Campos obligatorios no pueden venir vacios.
- No confiar en frontend.

## PRUEBAS
Unitarias e integracion real contra gabinete_detalle.

## CRITERIOS DE ACEPTACION
- Metadata real desde DB.
- Sin placeholders.
- Errores funcionales equivalentes al legado.
