## Why

Se requiere que el backend pueda emitir metadata de columnas fijas (`Pinned/LockPinned`) dentro de `DynamicUiTableDto.Columns` para que AppTable pueda renderizar columnas ancladas desde respuestas dinámicas.

## What Changes

- Extender `UiColumnDto` con `Pinned` y `LockPinned`.
- Normalizar esos valores en `DynamicUiTableBuilder`.
- Ajustar `workflowInboxgestion` para fijar la columna `acciones` a la derecha.
- Cubrir contrato y productor real con pruebas.

## Capabilities

### New Capabilities
- jira-scrum-122: DynamicUiTable backend can emit fixed-column metadata for AppTable.

### Modified Capabilities
- None.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-122
- OpenSpec change path: openspec/changes/scrum-122-actualizacion-dynamicuitablebuil-cel-fij/
- Repos impactados: MiApp.DTOs, MiApp.Services, DocuArchiCore/tests
