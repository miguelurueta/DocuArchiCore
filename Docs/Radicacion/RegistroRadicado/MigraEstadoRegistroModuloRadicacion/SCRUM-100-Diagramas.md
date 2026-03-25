# SCRUM-100 - Diagramas

## Flujo

```text
Caller
  -> IRaRadEstadosModuloRadicacionR.ActualizaEstadoModuloRadicacio(defaultDbAlias, idRegistroEstado, estado)
    -> RaRadEstadosModuloRadicacionR
      -> IDapperCrudEngine.UpdateDynamicWithValidationAsync(QueryOptions, "ActualizaEstadoModuloRadicacio")
        -> ra_rad_estados_modulo_radicacion
```

## Alcance

- No crea `Service`
- No crea `Controller`
- No crea DTO adicional
- No crea Model adicional
- Solo migra la operacion legacy de actualizacion de estado al repositorio existente
