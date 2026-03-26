# SCRUM-102 - Integracion Backend

## Resumen

`RegistrarRadicacionEntranteAsync` ahora actualiza `ra_rad_estados_modulo_radicacion.estado = 1` despues de registrar una tarea workflow.

## Flujo aplicado

1. `RegistrarTareaWorkflowInternaAsync` retorna una tarea valida.
2. El servicio toma `ReturnRegistraRadicacion.IdEstadoRadicado`.
3. Consulta el claim `defaulalias`.
4. Ejecuta `ActualizaEstadoModuloRadicacio(defaultDbAlias, idEstadoRadicado, 1)`.
5. Si la actualizacion falla, el servicio retorna error controlado.

## Repos impactados

- `MiApp.Services`
- `DocuArchiCore` pruebas y documentacion
