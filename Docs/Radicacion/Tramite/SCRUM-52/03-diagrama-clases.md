# Diagrama de Clases - SCRUM-52

```mermaid
classDiagram
    class AliasCamposRadicacionEntrante {
      +ResolverAlias(nombreCampo, detallePlantilla) string
      +ConstruirMensajeValorInvalido(aliasCampo) string
    }

    class ValidaCamposObligatoriosService
    class ValidaDimensionCamposService
    class ValidaCamposDinamicosUnicosRadicacionService
    class ValidaTipoCamposService

    ValidaCamposObligatoriosService --> AliasCamposRadicacionEntrante
    ValidaDimensionCamposService --> AliasCamposRadicacionEntrante
    ValidaCamposDinamicosUnicosRadicacionService --> AliasCamposRadicacionEntrante
    ValidaTipoCamposService --> AliasCamposRadicacionEntrante
```
