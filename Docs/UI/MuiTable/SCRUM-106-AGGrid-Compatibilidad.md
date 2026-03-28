# SCRUM-106 - Compatibilidad AG Grid para DynamicUiTableBuilder

## Objetivo

Hacer explícita la compatibilidad del payload `DynamicUiTableDto` con AG Grid sin romper el consumo actual desde MUI DataGrid.

## Ajustes implementados

- Se agregó `UiColumnDto.Field` como alias explícito para `AgGridColDef.field`.
- Se agregó `UiColumnDto.AgGridFilterType` como alias explícito para `AgGridColDef.filter`.
- `DynamicUiTableBuilder` ahora normaliza:
  - `Field`
  - `DataIndex`
  - `Title`
  - `FilterType`
  - `AgGridFilterType`
- La columna automática de acciones también queda normalizada para AG Grid.

## Reglas de derivación

- `Field`:
  - usa `ColumnName` cuando está disponible
  - cae a `Key` si `ColumnName` no viene informado
- `AgGridFilterType`:
  - `text` -> `agTextColumnFilter`
  - `select` -> `agSetColumnFilter`
  - `date` / `datetime` -> `agDateColumnFilter`
  - `none` o columna no filtrable -> `none`

## Compatibilidad preservada

- MUI sigue consumiendo:
  - `Key`
  - `HeaderName`
  - `Rows`
  - `Pagination`
  - `Sorting`
- Ant Design sigue consumiendo:
  - `DataIndex`
  - `Title`
  - `FilterType`

## Evidencia

- Pruebas actualizadas en `tests/TramiteDiasVencimiento.Tests/DynamicUiTableServiceTests.cs`
- Cobertura validada sobre:
  - aliases AG Grid
  - aliases Ant Design
  - columna automática de acciones
  - filtros derivados para texto, fecha y selección
