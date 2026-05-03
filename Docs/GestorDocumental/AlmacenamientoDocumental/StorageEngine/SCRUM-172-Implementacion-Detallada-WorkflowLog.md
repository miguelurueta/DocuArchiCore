# SCRUM-172 Implementacion Detallada Workflow Log

## Repos impactados
- `MiApp.Models`
- `MiApp.Repository`
- `MiApp.Services`
- `DocuArchi.Api`
- `DocuArchiCore` (OpenSpec + documentacion + pruebas coordinador)

## Archivos objetivo
### MiApp.Models
- `Models/GestorDocumental/AlmacenamientoDocumental/WorkflowStorageLogModel.cs`

### MiApp.Repository
- `Repositorio/GestorDocumental/AlmacenamientoDocumental/Workflow/IWorkflowStorageLogRepository.cs`
- `Repositorio/GestorDocumental/AlmacenamientoDocumental/Workflow/WorkflowStorageLogRepository.cs`

### MiApp.Services
- `Service/GestorDocumental/AlmacenamientoDocumental/Workflow/IWorkflowStorageLogBuilder.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/Workflow/WorkflowStorageLogBuilder.cs`
- `Service/GestorDocumental/AlmacenamientoDocumental/Transaction/StorageTransactionCoordinator.cs` (integracion de fase workflow)

### DocuArchi.Api
- `Program.cs` (registro DI)

## Detalle tecnico esperado
- Insert en `logdocuarchi` solo con `IdTareaWorkflow > 0`.
- Builder mapea:
  - `IdAlmacen` desde reserva de identidad.
  - `UsuarioOperacion` y `UsuarioPropietario` desde `context.Usuario`.
  - `Radicado` desde `context.Command.Inventario.Radicado` (si aplica).
  - `Campos` desde indexacion (formato controlado).
  - `RutaDocumento` en formato logico/relativo.
- Repository valida precondiciones y ejecuta insert transaccional via `DapperCrudEngine`.
- Si `rows != 1`, lanzar `StorageTransactionException`.

## Estado actual de implementacion
- OpenSpec y documentacion tecnica central: actualizado en `DocuArchiCore`.
- Implementacion funcional satelite (`MiApp.Models`, `MiApp.Repository`, `MiApp.Services`, `DocuArchi.Api`): pendiente de cierre y publicacion de PRs en esta iteracion.

