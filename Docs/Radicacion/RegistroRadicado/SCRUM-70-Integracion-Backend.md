# SCRUM-70 Integracion backend

## Objetivo

Integrar la prevalidacion workflow antes del registro definitivo del radicado usando un servicio dedicado.

## Punto de integracion

- Servicio principal: `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
- Servicio nuevo: `MiApp.Services/Service/Radicacion/RelacionCamposRutaWorklflow/ValidaPreRegistroWorkflowService.cs`
- Metodo impactado: `RegistrarRadicacionEntranteAsync`
- Cadena integrada:
  - `IValidaPreRegistroWorkflowService`
  - `ISolicitaEstructuraRutaWorkflowService`
  - `IRelacionCamposRutaWorklflowService`
  - `IAsingacionValoresDatosRadicadoRutaWorklflow`
  - `IValidaDatosRadicacionTareaWorkflowService`

## Condicion funcional

El flujo workflow solo se ejecuta cuando `util_tipo_modulo_envio == 2`.

Si la condicion no se cumple:
- no se consulta `defaulaliaswf`
- no se consulta estructura workflow
- no se ejecuta asignacion ni validacion workflow
- el registro sigue por el flujo normal actual

## Flujo aplicado

1. `RegistrarRadicacionEntranteAsync` obtiene y valida datos base de radicacion.
2. Cuando `util_tipo_modulo_envio == 2`, delega en `IValidaPreRegistroWorkflowService`.
3. Ese servicio consulta el claim `defaulaliaswf` con `ICurrentUserService`.
4. Con ese alias consume `SolicitaEstructuraRutaWorkflow(defaultDbAliasWorkflow)`.
5. Toma la primera ruta activa y construye `nombreRuta = "dat_adic_tar" + Nombre_Ruta`.
6. Consulta la relacion plantilla-ruta con `SolicitaCamposRelacionRutaPlantillaAsync`.
7. Asigna los valores del request a la estructura workflow con `AsignaDatosRadicacionAsync`.
8. Ejecuta `ValidaDatosRadicacionTareaWorkflowAsync`.
9. Devuelve `NombreRuta` y `RelacionCamposRutaWorklflowDto` con los datos ya asignados y validados.
10. Solo si todo lo anterior es exitoso, continua el registro definitivo del radicado.

## Punto exacto de corte

El proceso se detiene antes de `_registrarRepository.RegistrarRadicacionEntranteAsync(...)` cuando ocurre cualquiera de estos casos:

- falta el claim `defaulaliaswf`
- no existe ruta workflow activa
- no existe relacion plantilla-ruta
- falla la asignacion de datos workflow
- falla la validacion de datos workflow

En todos esos casos se retorna `AppResponses<T>` controlado y el radicado no se registra.

## Archivos relacionados

- `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
- `MiApp.Services/Service/Radicacion/RelacionCamposRutaWorklflow/ValidaPreRegistroWorkflowService.cs`
- `MiApp.Services/Service/Workflow/RutaTrabajo/SolicitaEstructuraRutaWorkflowService.cs`
- `MiApp.Services/Service/Radicacion/Tramite/RelacionCamposRutaWorklflowService.cs`
- `MiApp.Services/Service/Radicacion/RelacionCamposRutaWorklflow/AsingacionValoresDatosRadicadoRutaWorklflow.cs`
- `MiApp.Services/Service/Radicacion/RelacionCamposRutaWorklflow/ValidaDatosRadicacionTareaWorkflowService.cs`
- `MiApp.DTOs/DTOs/Radicacion/RelacionCamposRutaWorklflow/ValidaPreRegistroWorkflowResultDto.cs`
- `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/RegistrarRadicacionEntranteServiceTests.cs`

## Evidencia de validacion

- Pruebas focalizadas: `dotnet test --filter FullyQualifiedName~RegistrarRadicacionEntranteServiceTests`
- Resultado: `8/8` pruebas OK
- Escenarios cubiertos en esta bateria:
  - flujo valido
  - alias vacio
  - propagacion de alias y usuario radicador
  - falla de parametros backend
  - configuracion inexistente
  - falla de configuracion
  - bypass workflow cuando `util_tipo_modulo_envio != 2`
  - bloqueo cuando falta claim `defaulaliaswf`
