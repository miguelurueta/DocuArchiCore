## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-114 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Confirmar reutilizacion de infraestructura existente sin introducir modelos nuevos.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-114/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Extender DTOs y resolvedor de contexto para usar alias workflow e id de ruta como fuente de verdad.
- [x] 3.2 Implementar servicio/repositorio de bandeja workflow y endpoint API con DI.
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local en DTOs, Services y Api.

## 4. Test

- [x] 4.1 Implementar pruebas del resolvedor, query builder, repository y servicio de bandeja workflow.
- [x] 4.2 Ejecutar dotnet build/dotnet test y documentar evidencia de validacion.
- [ ] 4.3 Validar y archivar con OpenSpec.
