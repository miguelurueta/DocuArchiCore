# SCRUM-34 - PR Checklist by Repository

## 1) MiApp.DTOs

- [ ] Crear DTOs `UI/MuiTable`.
- [ ] Validar contrato `DynamicUiTableDto` y `DynamicUiRowsOnlyDto`.
- [ ] Abrir PR y registrar URL en `sync.md`.

## 2) MiApp.Models

- [ ] Confirmar si se requiere modelo para `ui_table_columns`.
- [ ] Crear/actualizar modelo si aplica.
- [ ] Abrir PR y registrar URL en `sync.md`.

## 3) MiApp.Repository

- [ ] Implementar `IUiTableConfigRepository` y `UiTableConfigRepository`.
- [ ] Query parametrizada + `DefaultAlias`.
- [ ] Abrir PR y registrar URL en `sync.md`.

## 4) MiApp.Services

- [ ] Implementar builder reusable + service generico + handler base.
- [ ] Implementar validaciones de claims y sort whitelist.
- [ ] Registrar mappings AutoMapper.
- [ ] Abrir PR y registrar URL en `sync.md`.

## 5) DocuArchi.Api

- [ ] Crear endpoints `/api/ui/dynamic-table/query` y `/api/ui/dynamic-table/action`.
- [ ] Crear wrappers por modulo.
- [ ] Registrar DI en `Program.cs`.
- [ ] Abrir PR y registrar URL en `sync.md`.

## 6) Validation

- [ ] Unit tests.
- [ ] Integration tests o `skipped` justificado.
- [ ] `dotnet test` y `openspec validate` en verde.

## 7) Archive

- [ ] PRs en estado `merged`.
- [ ] Ejecutar `opsxj:archive SCRUM-34`.
