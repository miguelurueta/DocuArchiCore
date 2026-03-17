# SCRUM-72 Diagramas

## Caso de uso

```text
Componente interno workflow
    |
    v
SolicitaEstructuraConfiguracionListadoRutaRepository
    |
    v
configuracion_listado_ruta (Rutas_Workflow_id_Ruta = idRuta)
```

## Diagrama de clases

```text
ComponenteInterno
  -> ISolicitaEstructuraConfiguracionListadoRutaRepository

SolicitaEstructuraConfiguracionListadoRutaRepository
  -> IDapperCrudEngine

ConfiguracionListadoRuta
AppResponses<List<ConfiguracionListadoRuta>?>
```

## Diagrama de secuencia

```text
ComponenteInterno -> Repository: SolicitaEstructuraConfiguracionListadoRutaAsync(idRuta, defaultDbAlias)
Repository -> Repository: validar idRuta y defaultDbAlias
Repository -> DapperCrudEngine: GetAllAsync(QueryOptions)
DapperCrudEngine -> configuracion_listado_ruta: SELECT ... WHERE Rutas_Workflow_id_Ruta = @idRuta
configuracion_listado_ruta --> DapperCrudEngine: filas
DapperCrudEngine --> Repository: QueryResult<ConfiguracionListadoRuta>
Repository --> ComponenteInterno: AppResponses<List<ConfiguracionListadoRuta>?>
```

## Estado

```text
Inicio
  -> Validar parametros
  -> Error de validacion
  -> Retornar success=false

Inicio
  -> Validar parametros
  -> Consultar configuracion_listado_ruta
  -> Sin resultados
  -> Retornar success=true data=null

Inicio
  -> Validar parametros
  -> Consultar configuracion_listado_ruta
  -> Con resultados
  -> Retornar success=true data=list

Inicio
  -> Excepcion
  -> Retornar success=false
```
