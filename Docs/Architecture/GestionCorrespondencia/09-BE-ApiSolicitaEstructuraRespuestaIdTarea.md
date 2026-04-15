# PROMPT ARQUITECTÓNICO — Ticket 09 BE
# Crear API + Service: SolicitaEstructuraRespuestaIdTarea (ra_respuesta_radicado por ID_TAREA_WF)

Rol esperado:
Arquitecto de software senior backend (.NET, C#, ASP.NET Core, controllers, services, repositorios, Dapper, seguridad por claims, contratos).

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Exponer un endpoint API que permita consultar la estructura de respuesta asociada a una tarea de workflow, filtrando únicamente por `ID_TAREA_WF`, utilizando el repositorio `SolicitaEstructuraRespuestaIdTarea...` a través de un servicio intermedio.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
CONTEXTO EXISTENTE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Patrón de Controller con claims + AppResponses<T>:
DocuArchi.Api/Controllers/Radicacion/Tramite/SolicitaEstructuraRutaWorkflowController.cs

Patrón de Controller con request/claims:
DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs

Patrón de Service:
MiApp.Services/Service/Workflow/RutaTrabajo/SolicitaEstructuraRutaWorkflowService.cs

Repositorio/modelo del ticket previo:
Docs/Architecture/GestionCorrespondencia/tuket-solicita-estructura-respuesta-id-trea.md

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
UBICACIÓN ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:
DocuArchi.Api/Controllers/GestorDocumental/SolicitaEstructuraRespuestaIdTareaController.cs

Service:
MiApp.Services/Service/GestorDocumental/ServiceSolicitaEstructuraRespuesta.cs

Repositorio:
MiApp.Repository/Repositorio/GestionCorrespondencia/

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
RUTA OFICIAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Base controller:
[Route("api/gestor-documental")]

Acción:
GET /api/gestor-documental/solicita-estructura-respuesta-id-tarea?idTareaWf={ID_TAREA_WF}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
CONTRATO OFICIAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Parámetro:
- idTareaWf (querystring, requerido)

Response:
- AppResponses<List<RaRespuestaRadicado>>

REGLA:
- sin resultados → success = true, data = [], message = "Sin resultados"

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
REGLAS DE SEGURIDAD / CLAIMS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Validar obligatoriamente:
- defaulalias

Usar:
- IClaimValidationService

Regla:
- si claim inválido o ausente, retornar BadRequest(validation.Response)

No validar usuarioid salvo requerimiento explícito del negocio.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
REGLAS DE IMPLEMENTACIÓN — CONTROLLER
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

1. Inyectar:
   - IClaimValidationService
   - IServiceSolicitaEstructuraRespuesta

2. Validar claim defaulalias

3. Validar idTareaWf:
   - requerido
   - > 0

4. Invocar service:
   - SolicitaEstructuraRespuestaIdTareaAsync(idTareaWf, defaultDbAlias)

5. Si el resultado es inválido según el estándar actual del proyecto:
   - retornar BadRequest(result)

6. Si el resultado es exitoso:
   - retornar Ok(result)

REGLA:
- el controller no consulta repositorio directamente
- el controller no contiene lógica de negocio

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
REGLAS DE IMPLEMENTACIÓN — SERVICE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Nombre:
ServiceSolicitaEstructuraRespuesta

Interface:
IServiceSolicitaEstructuraRespuesta

Método:
Task<AppResponses<List<RaRespuestaRadicado>>> SolicitaEstructuraRespuestaIdTareaAsync(
    long idTareaWf,
    string defaultDbAlias
);

Reglas:

- encapsular en try/catch
- validar defensivamente defaultDbAlias
- llamar al repositorio correspondiente
- propagar message / errors según estándar
- no transformar a DTO adicional en esta fase
- no meter lógica de controller en el service

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
REPOSITORIO A UTILIZAR
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Interface:
ISolicitaEstructuraRespuestaIdTareaRepository

Método:
SolicitaEstructuraRespuestaIdTareaAsync(...)

Regla crítica:
- filtrar únicamente por ID_TAREA_WF

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PRUEBAS OBLIGATORIAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:
- defaulalias faltante → 400
- idTareaWf inválido → 400
- success con resultados → 200
- success sin resultados → 200 con success = true y data = []
- error controlado del service/repository → comportamiento consistente con el estándar actual

Service:
- defaultDbAlias inválido → error controlado
- success con lista
- success sin resultados → data = []
- excepción del repositorio → error controlado

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
ENTREGABLES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- DocuArchi.Api/Controllers/GestorDocumental/SolicitaEstructuraRespuestaIdTareaController.cs
- MiApp.Services/Service/GestorDocumental/ServiceSolicitaEstructuraRespuesta.cs
- interface IServiceSolicitaEstructuraRespuesta
- registro en DI de service y repository
- pruebas relacionadas

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
CRITERIOS DE ACEPTACIÓN
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- existe endpoint GET oficial por ID_TAREA_WF
- usa claim defaulalias
- usa service intermedio
- usa repository existente
- response mantiene AppResponses<List<RaRespuestaRadicado>>
- sin resultados retorna data = []
- no se rompe el patrón arquitectónico actual