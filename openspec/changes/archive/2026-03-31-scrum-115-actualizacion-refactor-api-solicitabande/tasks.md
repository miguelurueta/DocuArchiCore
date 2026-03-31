## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-115 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de cambiar controller, DTOs y services.
- [x] 1.3 Confirmar que no se requieren cambios en MiApp.Repository ni MiApp.Models.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-115/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Crear WorkflowInboxApiRequestDto y completar contratos internos de inbox workflow.
- [x] 3.2 Ajustar controller y service para construir el request interno enriquecido en backend.
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local del slice workflow inbox.

## 4. Test

- [x] 4.1 Ajustar pruebas de query builder, repository y service, y agregar pruebas del controller/API request.
- [x] 4.2 Ejecutar dotnet build/dotnet test del slice workflow inbox y documentar evidencia.
- [ ] 4.3 Validar y archivar con OpenSpec.
