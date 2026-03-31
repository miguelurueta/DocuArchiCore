## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-110 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Solicitar estructura de tabla si se requiere nuevo modelo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-110/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Implementar `WorkflowInboxDynamicTableRequestDto`, `IWorkflowInboxQueryBuilder`, `WorkflowInboxQueryBuilder` y `WorkflowInboxQueryPolicy` con `QueryOptions` reales y sanitizacion controlada.
- [x] 3.2 Registrar `IWorkflowInboxQueryBuilder` en `Program.cs` y alinear el alcance real del ticket a DTOs + Repository + DI.
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local.

## 4. Test

- [x] 4.1 Implementar Unit/Integration/Contract tests y documentar evidencia.
- [x] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible).
- [ ] 4.3 Validar y archivar con OpenSpec.
