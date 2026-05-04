# SCRUM-177 Cobertura Legacy Metadata Campos Obligatorios

## Tabla de cobertura VB -> .NET

| Legacy VB | Componente nuevo | Cobertura |
|---|---|---|
| `Consulta_Campos_Obligatorio(_Nombre_Gabienete, Matri_Campos_Obli)` | `StorageGabineteMetadataRepository.GetFieldsAsync` + `StorageGabineteMetadataProvider.GetMetadataAsync` | Cumple |
| `If Matri_Campos_Obli Is Nothing Then ...` | `StorageRequiredFieldsValidator` (`Metadata nula` / metadata vacia) | Cumple |
| `If UBound(Matri_Campos_Obli) <> UBound(_Matri_Datos) Then ...` | `StorageRequiredFieldsValidator` (`Cantidad de campos no coincide con metadata`) | Cumple |
| Validacion por posicion/nombre en matriz | `StorageRequiredFieldsValidator` (comparacion posicional por `FieldName` vs `NombreCampo`) | Cumple |
| `If Matri_Tempo(0)=1 And _Matri_Datos(z)="" Then ...` | `StorageRequiredFieldsValidator` (`Campo obligatorio vacio`) | Cumple |
| Error funcional en validacion de metadata | `GabineteRequiredFieldsValidator` mapea a `GAB_FIELDS_*` | Cumple |

## Diferencias controladas frente al legacy
- Mensajeria de error ahora se codifica por `StorageError.Code` para pipeline (`GAB_FIELDS_*`), manteniendo semantica equivalente.
- La validacion se ejecuta en `ValidationPipeline` antes de fase DB transaccional, con trazabilidad por `requestId`.

## Reglas no migradas en este corte
- Ninguna regla funcional de metadata/obligatorios pendiente dentro del alcance de Prompt 11.
- Queda pendiente solo evidencia de integracion DB automatizada en CI (no funcional, sino de entorno de ejecucion).
