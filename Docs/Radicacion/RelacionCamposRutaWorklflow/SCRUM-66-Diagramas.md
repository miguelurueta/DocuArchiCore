# SCRUM-66 Diagramas

## Caso de uso
- Actor: flujo interno de radicacion/workflow.
- Caso: asignar datos de radicacion sobre la estructura de relacion plantilla-ruta.
- Flujo:
  1. El backend obtiene la lista `RelacionCamposRutaWorklflow`.
  2. Invoca `IAsingacionValoresDatosRadicadoRutaWorklflow`.
  3. El servicio construye un mapa desde `RegistrarRadicacionEntranteRequestDto`.
  4. Asigna `DatoCampoPlantilla` por coincidencia de `NombreCampoPlantilla`.
  5. Retorna `AppResponses<List<RelacionCamposRutaWorklflow>>`.

## Diagrama de clases
- `IAsingacionValoresDatosRadicadoRutaWorklflow` -> `AsingacionValoresDatosRadicadoRutaWorklflow`
- `AsingacionValoresDatosRadicadoRutaWorklflow` -> `CamposRadicacionRequestMap`
- `CamposRadicacionRequestMap` -> `RegistrarRadicacionEntranteRequestDto`
- `AsingacionValoresDatosRadicadoRutaWorklflow` -> `RelacionCamposRutaWorklflow`

## Diagrama de secuencia
1. Caller envia `relaciones` + `request`.
2. Servicio valida request.
3. Servicio genera mapa de campos.
4. Servicio recorre cada relacion.
5. Servicio asigna `DatoCampoPlantilla`.
6. Servicio retorna estructura transformada.

## Diagrama de estados
- `Sin relaciones` -> retorno `Sin resultados`
- `Request invalido` -> retorno `Validation error`
- `Relaciones validas` -> `Transformando`
- `Transformando` -> `OK`
