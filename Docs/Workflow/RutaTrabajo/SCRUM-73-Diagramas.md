# SCRUM-73 Diagramas

## Flujo transaccional

```text
RegistrarTareaWorkflowAsync
  -> validar parametros
  -> abrir conexion + begin transaction
  -> Q01 validar nombreRuta contra rutas_workflow
  -> Q02 insert INICIO_TAREAS_WORKFLOW
  -> Q03 insert DAT_ADIC_TAR{NombreRuta}
  -> Q04 insert ESTADOS_TAREA_WORKFLOW
  -> commit
```

## Flujo de error

```text
si falla cualquier paso:
  -> rollback
  -> AppResponses<RegistroTareaWorkflowResultDto> success = false
  -> error.Field = Q0x
```
