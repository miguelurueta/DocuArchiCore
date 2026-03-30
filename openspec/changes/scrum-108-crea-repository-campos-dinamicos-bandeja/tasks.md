## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-108 y confirmar alcance repository-only.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de implementar.
- [x] 1.3 Validar la tabla/modelo existente `configuracion_listado_ruta` y los DTOs creados en SCRUM-107.

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-108/spec.md` con requisitos finales.
- [x] 2.2 Incluir referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.
- [x] 2.3 Verificar escenarios testables por requisito.
- [x] 2.4 Ajustar el cambio para documentar que el mapeo se resuelve en repository, sin dependencia a `AutoMapperProfile`.

## 3. Application

- [x] 3.1 Implementar `IWorkflowRouteColumnConfigRepository` y `WorkflowRouteColumnConfigRepository` en `MiApp.Repository`.
- [x] 3.2 Implementar `WorkflowRouteColumnConfigValidator` y mapeo seguro a `WorkflowDynamicColumnDefinitionDto`.
- [x] 3.3 Registrar la interfaz en `DocuArchi.Api/Program.cs`.
- [x] 3.4 Actualizar trazabilidad de repos impactados en `sync.md`.

## 4. Test

- [x] 4.1 Implementar pruebas unitarias para validación, filtros por modo, normalización y errores controlados.
- [x] 4.2 Dejar evidencia de integración pendiente o placeholder explícito si Docker/Testcontainers no está disponible.
- [x] 4.3 Ejecutar `dotnet test`, `dotnet build` y `openspec validate`.
