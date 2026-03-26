# SCRUM-103 - Integracion Backend

## Resumen

`ActualizaEstadoModuloRadicacio` ahora actualiza `ra_rad_estados_modulo_radicacion.estado` e `id_tarea_workflow` en la misma operacion. `RegistrarRadicacionEntranteAsync` pasa `registroTareaWorkflowResult.data.idTareaWorkflow` al repositorio despues de registrar la tarea workflow.

## Flujo aplicado

1. `RegistrarTareaWorkflowInternaAsync` retorna una tarea workflow valida.
2. El servicio conserva `registroTareaWorkflowResult.data.idTareaWorkflow`.
3. El servicio toma `ReturnRegistraRadicacion.IdEstadoRadicado`.
4. Consulta el claim `defaulalias`.
5. Ejecuta `ActualizaEstadoModuloRadicacio(defaultDbAlias, idEstadoRadicado, 1, idTareaWorkflow)`.
6. Si falta `idTareaWorkflow` o la actualizacion falla, el flujo retorna error controlado.

## Repos impactados

- `MiApp.Services`
- `MiApp.Repository`
- `DocuArchiCore` pruebas y documentacion
