# DTO frontend - ValidaCamposDinamicosUnicosRadicacion

## Servicio

- Nombre: `IValidaCamposDinamicosUnicosRadicacionService`
- Metodo: `ValidaCamposDinamicosUnicosRadicacionAsync`
- Firma:
  - `RegistrarRadicacionEntranteRequestDto request`
  - `string defaultDbAlias`
  - `IReadOnlyCollection<DetallePlantillaRadicado> detallePlantilla`

## Request DTO principal

`RegistrarRadicacionEntranteRequestDto`:
- Usa `Campos[]` para transportar campos dinamicos (`NombreCampo`, `Valor`).
- La validacion de unicidad se aplica solo sobre campos dinamicos configurados como `UNICO`.

## Regla funcional

- Se identifican campos dinamicos unicos cuando:
  - `Comportamiento_Campo` contiene `UNICO`, o
  - `TagSesion` contiene `UNICO`.
- Se consulta la tabla plantilla asociada (`system_plantilla_radicado.Nombre_Plantilla_Radicado`).
- Si el valor ya existe para el campo, retorna error de validacion de tipo `Unique`.

## Response DTO

`AppResponses<List<ValidationError>?>`

Casos:
- `success=false`, `message="Validacion fallida"`, `data=[...]` cuando hay duplicados.
- `success=true`, `message="Sin resultados"`, `data=null` cuando no hay duplicados o no hay campos unicos a evaluar.
- `success=false`, `message="Error validando campos dinamicos unicos"` ante excepcion controlada.
