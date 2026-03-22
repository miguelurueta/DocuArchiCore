## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-82 y confirmar que el alcance es adaptación funcional mínima para Ant Design.
- [x] 1.2 Confirmar repos impactados y rutas reales (`MiApp.DTOs`, `MiApp.Services`, `DocuArchiCore/docs`, `tests`).
- [x] 1.3 Confirmar que no se requieren controllers o repositories nuevos para este ticket.

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-82/spec.md` con requisitos finales de aliases AntD, filtros y compatibilidad backward compatible.
- [x] 2.2 Incluir referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Ajustar `UiColumnDto` con aliases/backing metadata para Ant Design (`DataIndex`, `Title`, `Filterable`, `FilterType`, `FilterOptions`).
- [x] 3.2 Normalizar aliases y filtros desde `DynamicUiTableBuilder` sin romper el payload actual.
- [x] 3.3 Actualizar documentación técnica y frontend con el comportamiento final implementado.

## 4. Test

- [x] 4.1 Implementar pruebas unitarias/contractuales del builder para aliases AntD y metadata de filtros.
- [ ] 4.2 Ejecutar `dotnet test` y registrar evidencia o bloqueo.
- [x] 4.3 Validar OpenSpec del cambio.
