# SCRUM-73 Integracion Backend

## Objetivo

Registrar atomicamente una tarea workflow en:

- `INICIO_TAREAS_WORKFLOW`
- `DAT_ADIC_TAR + NombreRuta`
- `ESTADOS_TAREA_WORKFLOW`

El cambio sigue el patron repository y no expone API ni service nuevos.

## Componente principal

- Repository: [RegistroRadicadoTareaWorkflowRepository.cs](D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Repository/Repositorio/Workflow/RutaTrabajo/RegistroRadicadoTareaWorkflowRepository.cs)
- DTO retorno: [RegistroTareaWorkflowResultDto.cs](D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.DTOs/DTOs/Workflow/RutaTrabajo/RegistroTareaWorkflowResultDto.cs)
- Modelos:
  - [InicioTareasWorkflow.cs](D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Models/Models/Workflow/RutaTrabajo/InicioTareasWorkflow.cs)
  - [EstadosTareaWorkflow.cs](D:/imagenesda/GestorDocumental/DocuArchiCore/MiApp.Models/Models/Workflow/RutaTrabajo/EstadosTareaWorkflow.cs)

## Firma implementada

```csharp
Task<AppResponses<RegistroTareaWorkflowResultDto>> RegistrarTareaWorkflowAsync(
    int idRuta,
    string nombreRuta,
    int idGabinete,
    int? idImagen,
    int idActividadWorkflow,
    int idUsuarioWorkflow,
    int idFlujoTrabajo,
    int idActiovidadFujoTrabajo,
    int idUsuarioWorkflowFlujoTrabajo,
    int estadoActividaModuloRad,
    int estadoModuloRadicado,
    int estadoRecuperacionFlujoTrabajo,
    IReadOnlyCollection<RelacionCamposRutaWorklflowDto> relaciones,
    DateTime fechaIni,
    string defaultDbAlias)
```

## Notas de implementacion

- `nombreRuta` se valida contra `rutas_workflow` antes de concatenarlo en `DAT_ADIC_TAR{NombreRuta}`.
- El proceso corre en una sola transaccion con `commit`/`rollback`.
- El `lastInsertId` de `INICIO_TAREAS_WORKFLOW` se propaga a la tabla dinamica y al estado workflow.
- Los campos dinamicos se toman desde `RelacionCamposRutaWorklflowDto.NombreCampoRuta` y `DatoCampoPlantilla`.

## Resultado

En exito retorna:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "idTareaWorkflow": 77
  }
}
```
