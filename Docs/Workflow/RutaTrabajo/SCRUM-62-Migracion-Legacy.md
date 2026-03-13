# SCRUM-62 - Migracion Legacy

## Funcion legacy origen

- Funcion: `csfc_verifica_existencia_radicado`
- Archivo: `workflow/ClassWorkflow.vb`
- Reglas preservadas:
  - Consulta en tabla dinamica `dat_adic_tar{nombre_ruta}`.
  - Filtro por `RADICADO = consecutivo_radicado`.
  - Si no hay filas: `EstadoExistenciaRadicado = "NO"` y `IdTareaWorkflow = 0`.
  - Si hay fila: `EstadoExistenciaRadicado = "YES"` y se retorna `INICIO_TAREAS_WORKFLOW_ID_TAREA`.
  - En error tecnico: mensaje controlado con prefijo de la funcion legacy.

## Implementacion .NET

- Repository: `MiApp.Repository/Repositorio/Workflow/RutaTrabajo/SolicitaExistenciaRadicadoRutaWorkflowRepository.cs`
- Service: `MiApp.Services/Service/Workflow/RutaTrabajo/SolicitaExistenciaRadicadoRutaWorkflowService.cs`
- Controller API: `DocuArchi.Api/Controllers/Radicacion/Tramite/SolicitaExistenciaRadicadoRutaWorkflowController.cs`
- DTO: `MiApp.DTOs/DTOs/Workflow/RutaTrabajo/SolicitaExistenciaRadicadoRutaWorkflowDto.cs`
- Model: `MiApp.Models/Models/Workflow/RutaTrabajo/SolicitaExistenciaRadicadoRutaWorkflow.cs`

## Consideraciones

- Se usa `QueryOptions` con consulta parametrizada.
- Se valida formato de `nombreRuta` para evitar inyeccion en nombre de tabla dinamica.
- La respuesta usa `AppResponses` y `try/catch` controlado.
