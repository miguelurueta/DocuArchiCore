# SCRUM-74 Diagramas

## Secuencia

```text
consumidor interno
  -> ISolicitaListaEstructuraConfiguracionPlantillaRadicacionRepository
  -> QueryOptions(Table=ra_rad_config_plantilla_radicacion, Filter=system_plantilla_radicado_id_Plantilla)
  -> IDapperCrudEngine.GetAllAsync
  -> AppResponses<List<RaRadConfigPlantillaRadicacion>?>
```

## Casos

```text
1. idPlantilla valido con filas -> OK + lista
2. idPlantilla valido sin filas -> Sin resultados
3. alias invalido o error de motor -> error controlado
```
