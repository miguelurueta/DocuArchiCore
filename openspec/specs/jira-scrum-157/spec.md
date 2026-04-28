## ADDED Requirements

### Requirement: Traceability to Jira issue
The system MUST keep traceability between this OpenSpec change and Jira issue SCRUM-157.

#### Scenario: Change references Jira issue
- **WHEN** a reviewer opens the change artifacts
- **THEN** proposal and design identify Jira issue SCRUM-157

### Requirement: Backend update rules baseline
Backend update requests MUST follow repository, architecture and testing constraints defined in openspec/context/OPSXJ_BACKEND_RULES.md.

#### Scenario: Missing implementation constraints
- **WHEN** proposal/design/tasks are reviewed
- **THEN** they explicitly include route confirmation, interface policy, DI registration, AppResponses/try-catch and test requirements

### Requirement: Reubicar controller, service y repository (sin cambio funcional)
Los componentes del flujo `SolicitaEstructuraRespuestaIdTarea` MUST reubicarse a las nuevas carpetas definidas por el ticket, manteniendo contrato y comportamiento.

#### Scenario: Controller reubicado sin cambiar endpoint
- **GIVEN** el endpoint público existente del controller
- **WHEN** se reubica el archivo a `Controllers/GestionCorrespondencia/GestionRespuesta/`
- **THEN** el atributo `[Route("api/GestionCorrespondencia")]` y el `[HttpGet("solicita-estructura-respuesta-id-tarea")]` se mantienen idénticos
- **AND** no cambian firmas públicas ni shape de `AppResponses<T>`

#### Scenario: Service y repository reubicados con namespaces actualizados
- **WHEN** se reubican los archivos a `MiApp.Services/Service/GestionCorrespondencia/GestionRespuesta/` y `MiApp.Repository/Repositorio/GestionCorrespondencia/GestionRespuesta/`
- **THEN** se actualizan los `namespace`/`using` para compilar
- **AND** los nombres de clases e interfaces se mantienen (solo cambia ubicación física + namespace)

#### Scenario: DI actualizado tras cambio de namespaces
- **WHEN** se compila `DocuArchiCore.sln`
- **THEN** `Program.cs` resuelve correctamente `IServiceSolicitaEstructuraRespuesta`, `ServiceSolicitaEstructuraRespuesta`, `ISolicitaEstructuraRespuestaIdTareaRepository` y `SolicitaEstructuraRespuestaIdTareaRepository`

#### Scenario: Pruebas focales del flujo pasan
- **WHEN** se ejecuta `dotnet test` con filtro para `SolicitaEstructuraRespuestaIdTarea` y `ServiceSolicitaEstructuraRespuesta`
- **THEN** las pruebas del flujo pasan sin necesidad de infraestructura externa

