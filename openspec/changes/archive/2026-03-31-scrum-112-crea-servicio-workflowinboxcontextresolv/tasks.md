## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-112 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Confirmar que no se requiere nuevo modelo ni repositorio nuevo.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-112/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Crear `WorkflowInboxResolvedContextDto`, extender request base con `IdUsuarioGestion` y agregar `WorkflowInboxContextResolverService` con `AppResponses` + `try/catch`.
- [x] 3.2 Registrar `IWorkflowInboxContextResolverService` en `DocuArchi.Api` sin tocar el controller final todavia.
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local en `MiApp.DTOs`, `MiApp.Services` y checkout limpio de `DocuArchi.Api`.

## 4. Test

- [x] 4.1 Implementar unit tests del resolvedor cubriendo validaciones, fallas por repositorio, contexto incompleto y excepcion controlada.
- [x] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible).
- [ ] 4.3 Validar y archivar con OpenSpec.
