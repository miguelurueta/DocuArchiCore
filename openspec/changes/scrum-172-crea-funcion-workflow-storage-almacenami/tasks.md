## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-172 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de implementar.
- [x] 1.3 Confirmar estructura de tabla objetivo (`logdocuarchi`) en `Docs/DataModel/StorageEngine-Tables.md`.

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-172/spec.md` con requisitos finales de la fase workflow log.
- [x] 2.2 Incluir referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [ ] 3.1 Implementar `WorkflowStorageLogModel` en `MiApp.Models` y contratos `IWorkflowStorageLogRepository` / `IWorkflowStorageLogBuilder` en capas correspondientes.
- [ ] 3.2 Implementar `WorkflowStorageLogRepository` con `DapperCrudEngine + QueryOptions` e integrar `WorkflowStorageLogBuilder`.
- [ ] 3.3 Integrar fase workflow en `StorageTransactionCoordinator` (`IdTareaWorkflow > 0`) y rollback total en fallo.
- [ ] 3.4 Registrar DI en `DocuArchi.Api/Program.cs` para repositorio y builder.
- [ ] 3.5 Verificar compilacion local en repos impactados (`MiApp.Models`, `MiApp.Repository`, `MiApp.Services`, `DocuArchi.Api`).

## 4. Test

- [ ] 4.1 Implementar pruebas unitarias de `WorkflowStorageLogBuilder` y `WorkflowStorageLogRepository`.
- [ ] 4.2 Ajustar pruebas de `StorageTransactionCoordinator` para rama workflow (skip/insert/rollback) y cobertura de validaciones.
- [ ] 4.3 Ejecutar `dotnet test` y registrar evidencia (o bloqueo explicito de entorno).
- [x] 4.4 Completar documentacion tecnica SCRUM-172 (arquitectura, implementacion, pruebas, observabilidad, cobertura legacy, metadata).
- [ ] 4.5 Ejecutar `opsxj:orchestrate:publish` y `opsxj:orchestrate:archive` tras cierre multi-repo.
