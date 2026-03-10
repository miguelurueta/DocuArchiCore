# Diagrama de Clases

```mermaid
classDiagram
    class IRegistrarRadicacionEntranteService {
      +RegistrarRadicacionEntranteAsync(...)
    }

    class RegistrarRadicacionEntranteService {
      -IValidaCamposRadicacionService validaCamposRadicacion
      +RegistrarRadicacionEntranteAsync(...)
    }

    class IValidaCamposRadicacionService {
      +ValidaCamposRadicacionAsync(defaultDbAlias, request, detallePlantilla)
    }

    class ValidaCamposRadicacionService {
      -IValidaCamposObligatoriosService obligatorios
      -IValidaDimensionCamposService dimension
      -IValidaCamposDinamicosUnicosRadicacionService unicos
      +ValidaCamposRadicacionAsync(...)
    }

    class AppResponses~List~ValidationError~~

    IRegistrarRadicacionEntranteService <|.. RegistrarRadicacionEntranteService
    IValidaCamposRadicacionService <|.. ValidaCamposRadicacionService
    RegistrarRadicacionEntranteService --> IValidaCamposRadicacionService
    ValidaCamposRadicacionService --> AppResponses~List~ValidationError~~
```
