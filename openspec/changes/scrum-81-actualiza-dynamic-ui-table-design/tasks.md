## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-81 y confirmar que el alcance real es compatibilidad contractual/documental MUI + Ant Design.
- [x] 1.2 Confirmar repos impactados y rutas reales (`MiApp.DTOs`, `DocuArchiCore/docs`, `tests`).
- [x] 1.3 Confirmar que no se requieren Controllers/Repositories nuevos ni cambios disruptivos en payload.

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-81/spec.md` con requisitos finales orientados a compatibilidad MUI/AntD.
- [x] 2.2 Mantener referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Ajustar comentarios/metadata del contrato para expresar neutralidad de framework en `DynamicUiTableDtos.cs`.
- [x] 3.2 Actualizar documentacion tecnica y de integracion frontend con mapping MUI + Ant Design.
- [x] 3.3 Mantener compatibilidad con el payload actual sin introducir cambios disruptivos.

## 4. Test

- [x] 4.1 Agregar prueba unitaria/contractual para validar que el payload sigue siendo apto para adapter Ant Design.
- [ ] 4.2 Ejecutar `dotnet test` y registrar evidencia o bloqueo.
- [x] 4.3 Validar OpenSpec del cambio.
