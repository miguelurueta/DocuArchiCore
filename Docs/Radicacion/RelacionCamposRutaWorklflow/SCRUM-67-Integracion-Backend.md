# SCRUM-67 Integracion Backend

## Objetivo
- Validar en memoria los datos ya asignados a `RelacionCamposRutaWorklflow` antes de construir o registrar la tarea de workflow.
- Reutilizar el contrato de errores actual del proyecto con `AppResponses<List<ValidationError>?>`.
- No usar controller, repository, Dapper ni consultas a base de datos.

## Servicio
- `IValidaDatosRadicacionTareaWorkflowService`
- Implementacion: `ValidaDatosRadicacionTareaWorkflowService`
- Ubicacion: `MiApp.Services/Service/Radicacion/RelacionCamposRutaWorklflow`

## Contrato
```csharp
Task<AppResponses<List<ValidationError>?>> ValidaDatosRadicacionTareaWorkflowAsync(
    IReadOnlyCollection<RelacionCamposRutaWorklflow>? relaciones)
```

## Reglas soportadas
- `Required` solo cuando la metadata del campo declara la restriccion con marcadores como `required`, `not null`, `obligatorio` o `requerido`.
- `MaxLength` usando el menor valor numerico encontrado entre `DimensionCampoPlantilla` y `DimensionCampoRuta`.
- `InvalidType` usando `TipoCampoPlantilla` y `TipoCampoRuta` cuando describen tipos compatibles conocidos (`int`, `decimal`, `date`, `datetime`, `bool`, `guid`, `email`, `varchar`, etc.).
- Nulos y vacios:
  - Si el campo no esta marcado como requerido, un valor vacio no rompe el flujo.
  - Si esta marcado como requerido, un valor nulo, vacio o en blanco genera error.

## Limitaciones deliberadas
- La estructura `RelacionCamposRutaWorklflow` no expone una bandera formal de obligatoriedad.
- El servicio no inventa reglas de requerido; solo aplica esa validacion cuando la metadata textual la declara explicitamente.
- Si no existe metadata suficiente para validar una regla, el campo se omite en esa regla y el flujo continua.

## Registro DI
- `DocuArchi.Api/Program.cs`

## Ejemplo de consumo interno
```csharp
var result = await _validaDatosRadicacionTareaWorkflowService
    .ValidaDatosRadicacionTareaWorkflowAsync(relacionesAsignadas);
```
