## Context

- Jira issue key: SCRUM-34
- Jira summary: CREA-COMPONTE-DYMANIC-UI-TABLE
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-34

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se requiere un backend reusable para construir tablas UI dinamicas consumidas por React/MUI, evitando duplicar logica por entidad y manteniendo control de seguridad (claims + anti-SQLi) desde backend.

## Goals

- Construir un componente `DynamicUiTableBuilder` reusable por `TableId`.
- Exponer endpoints genericos y especificos por modulo para `query` y `action`.
- Soportar columnas por configuracion DB y columnas fijas por codigo.
- Estandarizar acciones `toolbar`, `bulk`, `row`, `cell` con reglas de claims.
- Garantizar respuestas `AppResponses<T>`, validaciones y `try/catch` en funciones publicas.
- Entregar pruebas unitarias/integracion y documentacion tecnica para frontend.

## Non-Goals

- Amarrar la solucion a una entidad unica.
- Implementar logica de negocio de todos los modulos en esta fase inicial.
- Cambiar arquitectura base fuera del patron Controller -> Service -> Repository.

## Target Repositories

- `DocuArchi.Api`
- `MiApp.DTOs`
- `MiApp.Services`
- `MiApp.Repository`
- `MiApp.Models` (condicional segun existencia de modelo config)
- `DocuArchiCore` (coordinacion OpenSpec y docs)

## Key Technical Decisions

- Contrato obligatorio con `AppResponses<T>` y convencion:
  - sin resultados: `success=true`, `data=null`, `message=\"Sin resultados\"`
  - error controlado: `success=false`, `errors` poblado
- Builder reusable en `MiApp.Services/Service/UI/MuiTable` con interfaz en el mismo archivo.
- Resolucion por `TableId` via handlers (`IDynamicUiTableHandler`) para evitar `switch` gigante.
- `sort` protegido por whitelist por `TableId`.
- Claims revalidados en ejecucion de acciones, no solo para visibilidad.
- Repository con queries parametrizadas (`DapperCrudEngine` + `QueryOptions.DefaultAlias`).

## Expected Deliverables

- DTOs `DynamicUiTableDto`, `DynamicUiRowsOnlyDto`, `UiColumnDto`, `UiRowDto`, `UiActionDto`, `UiCellActionDto`, requests y auxiliares.
- Repositorio de configuracion UI (`IUiTableConfigRepository` + implementacion).
- Servicio generico (`IDynamicUiTableService`) + builder + al menos un handler de referencia.
- Endpoints:
  - `POST /api/ui/dynamic-table/query`
  - `POST /api/ui/dynamic-table/action`
  - wrappers por modulo `/api/{modulo}/ui/table/*`
- Registro DI en `Program.cs`.
- Pruebas unitarias/integracion + docs en `Docs/UI/MuiTable`.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-34-crea-componte-dymanic-ui-table.
