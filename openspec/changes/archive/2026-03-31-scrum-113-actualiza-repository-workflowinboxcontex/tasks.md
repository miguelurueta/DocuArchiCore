## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-113 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino antes de crear Controllers/DTOs/Models/funciones.
- [x] 1.3 Confirmar que no se requiere nuevo modelo ni nuevos repositories.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-113/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Actualizar `WorkflowInboxContextResolverService` para usar `ResolveAsync(int idUsuarioGestion)` y claims `defaulalias` / `defaulaliaswf` mediante `ICurrentUserService`.
- [x] 3.2 Mantener DI existente sin crear nuevos repositories ni duplicar contratos.
- [x] 3.3 Integrar cambios de aplicacion y verificar compilacion local en `MiApp.Services`.

## 4. Test

- [x] 4.1 Actualizar unit tests del resolvedor para validar aliases por claim, firma simplificada y fallas por dependencia.
- [x] 4.2 Ejecutar dotnet test (o skipped explicito si Docker no disponible).
- [ ] 4.3 Validar y archivar con OpenSpec.
