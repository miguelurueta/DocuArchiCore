# SCRUM-98 - Integracion Backend

## Resumen

- `RegistrarRadicacionEntranteService` integra `RegistrarTareaWorkflowAsync` despues de consultar la existencia de tarea workflow.
- El registro de tarea se ejecuta solo cuando `IdTareaWorkflow == 0`.
- `RegistroTareaWorkflowResultDto` se usa como variable local interna del flujo y no se expone como respuesta publica independiente.

## Impacto Real

- `MiApp.Services`: integra el repositorio `IRegistroRadicadoTareaWorkflowRepository` dentro de `RegistrarRadicacionEntranteAsync`.
- `DocuArchiCore`: actualiza pruebas, spec, tasks y evidencia tecnica.
- `DocuArchi.Api`, `MiApp.DTOs`, `MiApp.Repository`, `MiApp.Models`: sin cambios funcionales adicionales porque el repositorio, DTO y DI ya existian.

## Flujo Ajustado

1. El servicio registra la radicacion con `_registrarRepository.RegistrarRadicacionEntranteAsync(...)`.
2. Si el registro fue exitoso, consulta `ConsultarExistenciaRadicadoRutaWorkflowAsync(...)`.
3. Si esa consulta retorna error tecnico o `success = false`, el flujo responde con error controlado.
4. `localRegistroTareaWorkflowResultDto` queda declarado fuera de la condicion `IdTareaWorkflow == 0`.
5. Si `IdTareaWorkflow == 0` y existen ruta, usuario, grupo y actividad inicial validos, el servicio ejecuta `RegistrarTareaWorkflowAsync(...)`.
6. Si el registro de tarea workflow falla o retorna un resultado invalido, el flujo se interrumpe con error controlado.
7. Si ya existe una tarea workflow, el flujo continua sin crear una nueva.

## Parametros Funcionales

- `idRuta`: `RutaWorkflow.id_Ruta`
- `nombreRuta`: `RutaWorkflow.Nombre_Ruta`
- `idGabinete`: `TipoDocEntrante.codigo_gabinete_workflow`
- `idImagen`: `null`
- `idActividadWorkflow`: `GruposWorkflow.id_Actividad`
- `idUsuarioWorkflow`: `UsuarioWorkflow.idU_suario`
- `idFlujoTrabajo`: `requestCanonico.RE_flujo_trabajo?.id_tipo_flujo_workflow`
- `idActiovidadFujoTrabajo`: `actividadInicioFlujo.IdActividadFlujoTrabajo`
- `idUsuarioWorkflowFlujoTrabajo`: `actividadInicioFlujo.IdUsuarioWorkflowFlujoTrabajo`
- `estadoActividaModuloRad`: `0`
- `estadoModuloRadicado`: `TipoDocEntrante.activo_modulo_respuesta`
- `estadoRecuperacionFlujoTrabajo`: `0`
- `relaciones`: `workflowValidation.Relaciones`
- `defaultDbAlias`: claim `defaulaliaswf`

## Cobertura Esperada

- Ejecuta `RegistrarTareaWorkflowAsync` cuando `IdTareaWorkflow == 0`.
- Omite el registro cuando `IdTareaWorkflow != 0`.
- Retorna error controlado cuando el registro de tarea workflow falla.

## Verificacion

- `dotnet test .tmp\\scrum95-harness\\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1`
