# SCRUM-61 Migracion Legacy

## Funcion Legacy
- Archivo: `Gestion/Class_ra_relacion_ruta_plantilla.vb`
- Funcion: `solicita_campos_relacion_ruta_plantilla(id_plantilla_radicado, id_ruta, ByRef estructura[])`

## Query Legacy
```sql
SELECT
  rcr.NOMBRE_CAMPO_PLANTILLA,
  rcr.TIPO_CAMPO_PLANTILLA,
  rcr.DIMENSION_CAMPO_PLANTILLA,
  rcr.NOMBRE_CAMPO_RUTA,
  rcr.TIPO_CAMPO_RUTA,
  rcr.DIMENSION_CAMPO_RUTA
FROM ra_relacion_ruta_plantilla rrr
INNER JOIN ra_campos_relacionados_ruta_plantilla rcr
  ON rcr.RA_RELACION_RUTA_PLANTILLA_ID_RELACION_RUTA_PLANTILLA = rrr.ID_RELACION_RUTA_PLANTILLA
WHERE system_plantilla_radicado_id_Plantilla = @idPlantillaRadicado
  AND ID_RUTA = @idRuta
```

## Reglas Migradas
- Parametros requeridos: `idPlantillaRadicado`, `idRuta`, `defaultDbAlias`.
- Consulta parametrizada con `QueryOptions` + `DapperCrudEngine`.
- Respuesta estandar `AppResponses<List<RelacionCamposRutaWorklflowDto>>`.
- Sin resultados: `success=true`, `message=\"Sin resultados\"`, `data=[]`.
