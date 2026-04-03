## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-116 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de cambiar WorkflowInboxRepository y WorkflowInboxService.
- [x] 1.3 Confirmar que no se requieren cambios en API, DTOs, Repository base ni Models.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-116/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Cambiar el contrato de WorkflowInboxRepository para retornar filas normalizadas tipadas.
- [x] 3.2 Ajustar WorkflowInboxService para consumir diccionarios y eliminar el casting dinamico.
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local del slice workflow inbox.

## 4. Test

- [x] 4.1 Actualizar pruebas del repository y service para validar filas normalizadas e id derivado.
- [x] 4.2 Ejecutar dotnet build/dotnet test del slice workflow inbox y documentar evidencia.
- [ ] 4.3 Validar y archivar con OpenSpec.
