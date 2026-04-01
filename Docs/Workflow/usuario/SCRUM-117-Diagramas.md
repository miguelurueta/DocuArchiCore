# SCRUM-117 - Diagramas

## Caso de uso

- Actor interno consume configuracion de lista workflow por usuario.
- El repository consulta `configuracion_usuario`.
- El consumidor recibe `AppResponses<configuracionUsuarioDTO?>`.

## Diagrama de clases

- `ISolicitaConfiguracionListaUsuarioWorkflowRepository`
- `SolicitaConfiguracionListaUsuarioWorkflowRepository`
- `configuracionUsuarioDTO`
- `IDapperCrudEngine`
- `QueryOptions`

## Diagrama de secuencia

1. Consumidor invoca repository con `idUsuarioWorkflow` y `defaultDbAlias`.
2. Repository construye `QueryOptions`.
3. `IDapperCrudEngine` consulta `configuracion_usuario`.
4. Repository retorna:
   - registro encontrado
   - `Sin resultados`
   - error controlado

## Estado

No aplica diagrama de estados adicional porque la funcionalidad es una consulta puntual sin transicion de workflow.
