# SCRUM-34 - Multi-Repo Implementation Playbook

## Objective

Implementar el backend reusable `DynamicUiTable` (DTO + builder + service + repository + endpoints) con seguridad por claims y sorting seguro.

## Repo Execution Order

1. `MiApp.DTOs`
2. `MiApp.Models` (si aplica)
3. `MiApp.Repository`
4. `MiApp.Services`
5. `DocuArchi.Api`
6. `DocuArchiCore` (coordinacion docs/tests/evidencia)

## Deliverables by Repository

### MiApp.DTOs

- Crear DTOs en `DTOs/UI/MuiTable`.
- Incluir comentarios XML en clases y propiedades.

### MiApp.Models

- Crear modelo de configuracion UI si no existe mapeo actual para `ui_table_columns`.

### MiApp.Repository

- Crear `IUiTableConfigRepository` y `UiTableConfigRepository`.
- Query parametrizada para columnas por `table_id`.
- Aplicar `QueryOptions.DefaultAlias`.

### MiApp.Services

- Crear `IDynamicUiTableBuilder` + `DynamicUiTableBuilder`.
- Crear `IDynamicUiTableService` + `DynamicUiTableService`.
- Crear `IDynamicUiTableHandler` y un handler de referencia.
- Agregar validaciones de claims y whitelist de sort.
- Registrar mapeos en `AutoMapperProfile`.

### DocuArchi.Api

- Crear endpoints:
  - `POST /api/ui/dynamic-table/query`
  - `POST /api/ui/dynamic-table/action`
- Crear wrappers por modulo `/api/{modulo}/ui/table/*`.
- Registrar DI en `Program.cs`.

### DocuArchiCore

- Agregar docs en `Docs/UI/MuiTable`.
- Agregar evidencia de pruebas y validaciones.

## Validation

- `dotnet build` en repos impactados.
- `dotnet test` para unitarios.
- Integration tests con Docker/Testcontainers o `skipped` justificado.
- `openspec validate scrum-34-crea-componte-dymanic-ui-table`.
