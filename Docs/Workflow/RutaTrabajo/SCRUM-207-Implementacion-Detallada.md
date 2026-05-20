# SCRUM-207 - Implementación Detallada

## 1. Archivos implementados

## 1.1 API
- `DocuArchi.Api/Controllers/Radicacion/Tramite/SolicitaGabineteRadicadoWorkflowController.cs`
- `DocuArchi.Api/Program.cs` (registro DI service/repository)

## 1.2 Services
- `MiApp.Services/Service/Workflow/RutaTrabajo/SolicitaGabineteRadicadoWorkflowService.cs`
- `MiApp.Services/Service/Mapping/Workflow/SolicitaGabineteRadicadoWorkflowMapping.cs`
- `MiApp.Services/Service/Mapping/AutoMapperProfile.cs` (registro mapping)

## 1.3 Repository
- `MiApp.Repository/Repositorio/Workflow/RutaTrabajo/SolicitaGabineteRadicadoWorkflowRepository.cs`

## 1.4 DTO / Modelo
- `MiApp.DTOs/DTOs/Workflow/RutaTrabajo/RadicadoGabineteWorkflowDto.cs`
- `MiApp.Models/Models/Workflow/RutaTrabajo/RadicadoGabineteWorkflow.cs`

## 1.5 Tests
- `tests/TramiteDiasVencimiento.Tests/SolicitaGabineteRadicadoWorkflowControllerContractTests.cs`
- `tests/TramiteDiasVencimiento.Tests/SolicitaGabineteRadicadoWorkflowServiceTests.cs`
- `tests/TramiteDiasVencimiento.Tests/SolicitaGabineteRadicadoWorkflowRepositoryTests.cs`

## 2. Contrato técnico implementado

## 2.1 Endpoint por radicado
- Método: `GET`
- Ruta: `/api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`
- Respuesta: `AppResponses<RadicadoGabineteWorkflowDto>`

## 2.2 Endpoint por tarea workflow
- Método: `GET`
- Ruta: `/api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`
- Respuesta: `AppResponses<RadicadoGabineteWorkflowDto>`

## 2.3 Claims usados
- `defaulaliaswf`: obligatorio para workflow.
- `defaulalias`: opcional por compatibilidad; no define el alias final de consulta de gabinete.

## 3. Lógica interna por capa

## 3.1 Controller
1. Valida claims (`defaulaliaswf`, opcional `defaulalias`).
2. Llama service por radicado o por id de tarea.
3. Retorna `BadRequest` en errores controlados (`success=false`).
4. Envueltos en `try/catch`.

## 3.2 Service
1. Valida input (`consecutivoRadicado`, `idTareaWorkflow`).
2. Resuelve ruta activa vía `SolicitaEstructuraRutaWorkflowAsync`.
3. Valida `Nombre_Ruta` con regex segura.
4. Resuelve alias de gabinete con alias workflow (`defaulaliaswf`).
5. Invoca repositorio y mapea modelo a DTO.
6. Normaliza fallback de salida (`EstadoExistenciaRadicado=NO`).

## 3.3 Repository
1. Construye tabla dinámica segura: `dat_adic_tar{Nombre_Ruta}`.
2. Consulta por `RADICADO` o por `INICIO_TAREAS_WORKFLOW_ID_TAREA`.
3. Si hay `ID_GABINETE > 0`, consulta `configuracion_gabinete`.
4. Si no hay fila, retorna `success=true`, `message=YES`, `EstadoExistenciaRadicado=NO`.
5. Si `EstadoExistenciaRadicado=YES` y `NombreGabinete` queda vacío, retorna `success=false` con error de validación (`NombreGabinete requerido`).
6. Todos los métodos operativos envueltos en `try/catch`.

## 4. Seguridad aplicada
1. Regex anti-inyección para nombre de ruta.
2. QueryOptions parametrizado; no SQL manual de valores.
3. No se recibe alias ni nombre de ruta desde frontend.
4. Error handling controlado con `AppResponses`.

## 5. Compatibilidad y no-regresión
1. `SolicitaExistenciaRadicadoRutaWorkflow` no fue modificado.
2. Endpoints nuevos bajo rutas nuevas (`/radicados/.../gabinete`, `/tareas/.../gabinete`).
