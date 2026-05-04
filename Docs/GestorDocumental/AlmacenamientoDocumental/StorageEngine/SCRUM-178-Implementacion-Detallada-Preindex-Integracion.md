# SCRUM-178 — Implementación Detallada Preindex Integración

## Archivos creados
- `MiApp.Models/Models/GestorDocumental/AlmacenamientoDocumental/StoragePreindexFile.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Preindex/IStoragePreindexResolver.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Preindex/StoragePreindexResolver.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Preindex/IStoragePreindexIntegrator.cs`
- `MiApp.Services/Service/GestorDocumental/AlmacenamientoDocumental/Preindex/StoragePreindexIntegrator.cs`

## Archivos modificados
- `MiApp.Models/.../StorageContext.cs`
  - Nuevo `EffectiveCamposIndexacion`.
- `MiApp.Services/.../Preindex/IStoragePreindexReader.cs`
  - Firma migrada a `ReadAsync(StoragePreindexFile file)`.
- `MiApp.Services/.../Preindex/StoragePreindexReader.cs`
  - Enfoque solo-lectura del archivo ya resuelto.
- `MiApp.Services/.../Validation/PreindexValidator.cs`
  - Orquesta `resolver + reader + integrator`.
  - Nuevos errores: `PREINDEX_FIELDS_EMPTY`, `PREINDEX_FIELDS_MISMATCH`.
- `MiApp.Services/.../Validation/GabineteRequiredFieldsValidator.cs`
  - Usa `EffectiveCamposIndexacion` cuando existe.
- `MiApp.Services/.../Transaction/StorageTransactionCoordinator.cs`
  - Inserción gabinete toma campos efectivos.
- `MiApp.Services/.../Builders/StorageXmlBuilder.cs`
  - XML toma campos efectivos.
- `MiApp.Services/.../Workflow/WorkflowStorageLogBuilder.cs`
  - Log toma campos efectivos.
- `DocuArchi.Api/Program.cs`
  - DI de resolver e integrador.

## Flujo final
1. `PreindexValidator` aplica solo en `BatchPreindex`.
2. Resuelve archivo preindex permitido.
3. Lee valores desde `.txt` / `.xmls`.
4. Integra por posición con `CamposIndexacion`:
   - Si valor manual existe, se conserva.
   - Si está vacío, se llena con preindex.
5. Guarda resultado en `context.EffectiveCamposIndexacion`.
6. Resto del pipeline y persistencia consume campos efectivos.

## Compatibilidad legacy
- Mantiene regla de obligatoriedad de preindex en modo batch.
- Mantiene integración posicional de valores.
- Agrega fallback de naming legacy en resolución de candidato preindex.
