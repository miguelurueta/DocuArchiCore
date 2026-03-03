# SCRUM-34 - Diagramas de Analisis (Reubicado en SCRUM-35)

> Documento movido desde `Docs/Radicacion/Tramite` a `Docs/UI/MuiTable` como parte del ticket SCRUM-35.

## Caso de uso

```mermaid
flowchart LR
    U[Frontend] --> C[DynamicUiTableController]
    C --> CV[ValidateClaim defaulalias]
    C --> S[IDynamicUiTableService]
    S --> H[IDynamicUiTableHandler por TableId]
    S --> B[IDynamicUiTableBuilder]
    B --> R[IUiTableConfigRepository]
    S --> O[AppResponses object]
```

## Diagrama de clases

```mermaid
classDiagram
    class DynamicUiTableController {
      +Query(DynamicUiTableQueryRequestDto)
      +Action(ExecuteUiActionRequestDto)
      +QueryByModulo(modulo, DynamicUiTableQueryRequestDto)
      +ActionByModulo(modulo, ExecuteUiActionRequestDto)
    }

    class IDynamicUiTableService {
      +QueryAsync(DynamicUiTableQueryRequestDto)
      +ExecuteActionAsync(ExecuteUiActionRequestDto)
    }
    class DynamicUiTableService
    class IDynamicUiTableHandler {
      +TableId
      +GetRowsAsync(...)
      +GetActions(...)
      +GetCellActions(...)
      +ExecuteActionAsync(...)
      +GetFixedColumns()
    }
    class DefaultDynamicUiTableHandler
    class IDynamicUiTableBuilder
    class DynamicUiTableBuilder
    class IUiTableConfigRepository
    class DynamicUiTableDto
    class DynamicUiRowsOnlyDto

    DynamicUiTableController --> IDynamicUiTableService
    DynamicUiTableService ..|> IDynamicUiTableService
    DynamicUiTableService --> IDynamicUiTableBuilder
    DynamicUiTableService --> IDynamicUiTableHandler
    DefaultDynamicUiTableHandler ..|> IDynamicUiTableHandler
    DynamicUiTableBuilder ..|> IDynamicUiTableBuilder
    DynamicUiTableBuilder --> IUiTableConfigRepository
    IDynamicUiTableBuilder --> DynamicUiTableDto
    IDynamicUiTableBuilder --> DynamicUiRowsOnlyDto
```

## Diagrama de secuencia (query)

```mermaid
sequenceDiagram
    participant U as Frontend
    participant C as DynamicUiTableController
    participant S as DynamicUiTableService
    participant H as Handler(TableId)
    participant B as DynamicUiTableBuilder
    participant R as UiTableConfigRepository

    U->>C: POST /api/ui/dynamic-table/query (Bearer token + body)
    C->>C: ValidateClaim("defaulalias")
    C->>C: ExtractClaims(role/permiso/permission)
    C->>S: QueryAsync(req)
    S->>H: GetRowsAsync(req)
    H-->>S: rows + total
    alt IncludeConfig=true
      S->>B: BuildAsync(input)
      opt UseColumnConfigFromDb=true
        B->>R: GetColumnsAsync(defaultDbAlias, tableId)
        R-->>B: UiColumnDto[]
      end
      B-->>S: DynamicUiTableDto
    else IncludeConfig=false
      S->>B: BuildRowsOnlyAsync(input)
      B-->>S: DynamicUiRowsOnlyDto
    end
    S-->>C: AppResponses<object>
    C-->>U: 200/400 + payload
```

## Diagrama de estado

```mermaid
stateDiagram-v2
    [*] --> ValidandoClaims
    ValidandoClaims --> ValidandoRequest: claim defaulalias OK
    ValidandoClaims --> ErrorControlado: claim invalido
    ValidandoRequest --> ResolviendoHandler
    ValidandoRequest --> ErrorControlado: TableId/Page/PageSize invalidos
    ResolviendoHandler --> ConsultandoRows
    ResolviendoHandler --> ErrorControlado: TableId sin handler
    ConsultandoRows --> SinResultados: rows vacio
    ConsultandoRows --> ConstruyendoPayload: rows con datos
    ConstruyendoPayload --> Exitoso
    ConstruyendoPayload --> ErrorControlado: excepcion en builder/handler
    SinResultados --> [*]
    Exitoso --> [*]
    ErrorControlado --> [*]
```
