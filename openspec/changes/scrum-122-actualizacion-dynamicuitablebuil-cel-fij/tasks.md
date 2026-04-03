## 1. Discovery

- [x] 1.1 Revisar el issue Jira SCRUM-122 y confirmar alcance del contrato backend para `UiColumnDto`.
- [x] 1.2 Confirmar repos impactados y rutas destino (`MiApp.DTOs`, `MiApp.Services`, `DocuArchiCore/tests`).
- [x] 1.3 Validar productores reales de `DynamicUiTableDto.Columns`, especialmente `workflowInboxgestion`.

## 2. Specs

- [x] 2.1 Completar `specs/jira-scrum-122/spec.md` con requisitos finales del contrato `Pinned/LockPinned`.
- [x] 2.2 Mantener referencia explícita a `openspec/context/OPSXJ_BACKEND_RULES.md` en artifacts del change.
- [x] 2.3 Verificar escenarios testables por requisito.

## 3. Application

- [x] 3.1 Extender `UiColumnDto` con `Pinned` y `LockPinned`.
- [x] 3.2 Normalizar `Pinned` en `DynamicUiTableBuilder` para aceptar solo `left/right` y preservar compatibilidad.
- [x] 3.3 Ajustar `WorkflowDynamicUiColumnBuilder` para fijar la columna `acciones` de `workflowInboxgestion`.

## 4. Test

- [x] 4.1 Implementar pruebas de contrato para `Pinned/LockPinned` y para el builder de workflow.
- [x] 4.2 Ejecutar `dotnet test` filtrado a `DynamicUiTableServiceTests` y documentar evidencia.
- [ ] 4.3 Validar el change con OpenSpec y dejarlo listo para publish/archive.
