# SCRUM-61 Diagramas

## Caso de Uso (texto)
- Actor: Frontend Radicacion
- Caso: Consultar campos relacionados entre plantilla y ruta workflow.
- Flujo:
  1. Frontend llama endpoint con `idPlantillaRadicado` y `idRuta`.
  2. Controller valida claim `defaulalias`.
  3. Service coordina consulta.
  4. Repository ejecuta query parametrizada.
  5. Retorna `AppResponses<List<RelacionCamposRutaWorklflowDto>>`.

## Diagrama de Clases (texto)
- `RelacionCamposRutaWorklflowController` -> `IRelacionCamposRutaWorklflowService`
- `RelacionCamposRutaWorklflowService` -> `IRelacionCamposRutaWorklflowRepository`
- `RelacionCamposRutaWorklflowRepository` -> `IDapperCrudEngine`
- `RelacionCamposRutaWorklflow` -> `RelacionCamposRutaWorklflowDto` (AutoMapper)
