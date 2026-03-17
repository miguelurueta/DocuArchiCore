# SCRUM-30 - Diagramas de Analisis

## Caso de uso

```mermaid
flowchart LR
    U[Usuario radicacion] --> C[TramiteController.ApListaRadicadosPendientes]
    C --> S[ServiceListaRadicadosPendientes]
    S --> UG[SolicitaEstructuraIdUsuarioGestion]
    S --> PL[SolicitaEstructuraPlantillaRadicacionDefault]
    S --> RP[SolicitaListaRadicadosPendientes]
    RP --> B[DynamicUiTableBuilder]
    B --> R[AppResponses<DynamicUiTableDto>]
```

## Diagrama de clases

```mermaid
classDiagram
    class TramiteController {
      +ApListaRadicadosPendientes()
    }
    class IListaRadicadosPendientesService {
      +SolicitaListaRadicadosPendientes(int idUsuarioGestion, string defaultDbAlias)
    }
    class ListaRadicadosPendientesService
    class IRemitDestInternoR
    class ISystemPlantillaRadicadoR
    class IListaRadicadosPendientesRepository
    class IDynamicUiTableBuilder
    class DynamicUiTableDto
    class DynamicUiActionRequestDto {
      +string RowIdField
      +Dictionary~string,string~ PayloadFields
    }
    class raradestadosmoduloradicacion

    TramiteController --> IListaRadicadosPendientesService
    ListaRadicadosPendientesService ..|> IListaRadicadosPendientesService
    ListaRadicadosPendientesService --> IRemitDestInternoR
    ListaRadicadosPendientesService --> ISystemPlantillaRadicadoR
    ListaRadicadosPendientesService --> IListaRadicadosPendientesRepository
    ListaRadicadosPendientesService --> IDynamicUiTableBuilder
    IListaRadicadosPendientesRepository --> raradestadosmoduloradicacion
    IDynamicUiTableBuilder --> DynamicUiTableDto
    DynamicUiTableDto --> DynamicUiActionRequestDto
```

## Diagrama de secuencia

```mermaid
sequenceDiagram
    participant U as Frontend
    participant C as TramiteController
    participant S as ListaRadicadosPendientesService
    participant G as UsuarioGestionRepository
    participant P as PlantillaRepository
    participant R as RadicadosPendientesRepository
    participant B as DynamicUiTableBuilder

    U->>C: GET /api/tramite/tramites/apListaRadicadosPendientes (Bearer token)
    C->>C: ValidateClaim("defaulalias")
    C->>C: ValidateClaim("usuarioid")
    C->>S: SolicitaListaRadicadosPendientes(idUsuarioGestion, defaultDbAlias)
    S->>G: SolicitaEstructuraIdUsuarioGestion(idUsuarioGestion, defaultDbAlias)
    G-->>S: Relacion_Id_Usuario_Radicacion
    S->>S: validar Relacion_Id_Usuario_Radicacion > 0
    S->>P: SolicitaEstructuraPlantillaRadicacionDefault(defaultDbAlias)
    P-->>S: idPlantilla
    S->>R: SolicitaListaRadicadosPendientes(idPlantilla, idUsuarioRadicacion, defaultDbAlias)
    R-->>S: lista pendiente
    S->>B: BuildAsync(rows, columns, action request)
    B-->>S: DynamicUiTableDto
    S-->>C: AppResponses<DynamicUiTableDto>
    C-->>U: 200 + payload
```

## Diagrama de estado

```mermaid
stateDiagram-v2
    [*] --> ValidandoClaims
    ValidandoClaims --> ResolviendoUsuarioGestion: claims validos
    ValidandoClaims --> ErrorControlado: claim invalido
    ResolviendoUsuarioGestion --> ResolviendoPlantilla
    ResolviendoUsuarioGestion --> ErrorControlado: Relacion_Id_Usuario_Radicacion <= 0
    ResolviendoUsuarioGestion --> ErrorControlado: excepcion
    ResolviendoPlantilla --> ConsultandoPendientes
    ResolviendoPlantilla --> SinResultados: sin plantilla
    ResolviendoPlantilla --> ErrorControlado: excepcion
    ConsultandoPendientes --> ConstruyendoTabla: hay registros
    ConsultandoPendientes --> SinResultados: lista vacia
    ConsultandoPendientes --> ErrorControlado: excepcion
    ConstruyendoTabla --> Exitoso: DynamicUiTableDto listo
    ConstruyendoTabla --> ErrorControlado: excepcion builder
    Exitoso --> [*]
    SinResultados --> [*]
    ErrorControlado --> [*]
```
