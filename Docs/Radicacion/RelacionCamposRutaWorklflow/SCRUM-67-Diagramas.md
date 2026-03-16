# SCRUM-67 Diagramas

## Caso de uso
- Actor: flujo interno de construccion/registro de tarea workflow.
- Caso: validar los datos ya asignados a la relacion plantilla-ruta antes de continuar.
- Flujo:
  1. El backend obtiene la lista `RelacionCamposRutaWorklflow` con `DatoCampoPlantilla` asignado.
  2. Invoca `IValidaDatosRadicacionTareaWorkflowService`.
  3. El servicio recorre la lista y valida reglas en memoria.
  4. Retorna `AppResponses<List<ValidationError>?>`.
  5. El caller decide continuar o detener el flujo.

## Diagrama de clases
- `IValidaDatosRadicacionTareaWorkflowService` -> `ValidaDatosRadicacionTareaWorkflowService`
- `ValidaDatosRadicacionTareaWorkflowService` -> `RelacionCamposRutaWorklflow`
- `ValidaDatosRadicacionTareaWorkflowService` -> `ValidationError`
- `ValidaDatosRadicacionTareaWorkflowService` -> `AppResponses<T>`

## Diagrama de secuencia
1. Caller envia `relaciones`.
2. Servicio valida lista nula o vacia.
3. Servicio evalua `Required` declarado por metadata.
4. Servicio evalua `MaxLength`.
5. Servicio evalua compatibilidad de tipo.
6. Servicio retorna resultado consolidado.

## Diagrama de estados
- `Lista nula` -> `Validation error`
- `Sin relaciones` -> `Sin resultados`
- `Relaciones validas` -> `Validando`
- `Validando` -> `Validacion exitosa`
- `Validando` -> `Validacion fallida`
