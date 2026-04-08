## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-134 y confirmar alcance.
- [x] 1.2 Confirmar repos impactados y rutas destino.
- [x] 1.3 Confirmar que no se requiere nuevo controller, DTO, modelo, repository ni estructura de tabla.

## 2. Specs

- [x] 2.1 Completar specs/jira-scrum-134/spec.md con requisitos finales.
- [x] 2.2 Incluir referencia explicita a openspec/context/OPSXJ_BACKEND_RULES.md.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Ajustar WorkflowInboxQueryPolicy para usar `ESCAPE '!'`.
- [x] 3.2 Ajustar EscapeLikeLiteral para escapar `!`, `%`, `_` y comillas simples sin regla SQL Server-only de corchetes.
- [x] 3.3 Verificar que rows, count, export y autocomplete usan la misma politica de escape.

## 4. Test

- [x] 4.1 Actualizar Unit tests de WorkflowInboxQueryBuilder.
- [x] 4.2 Ejecutar build/test focal.
- [x] 4.3 Validar OpenSpec y documentar evidencia.
