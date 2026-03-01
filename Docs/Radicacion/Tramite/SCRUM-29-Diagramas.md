# SCRUM-29 - Diagramas de Analisis

## Caso de uso

```mermaid
flowchart LR
    U[Usuario autenticado] --> C[TramiteController]
    C --> S[FechaLimiteRespuestaService]
    S --> P[SystemPlantillaRadicadoR]
    S --> D[TotalDiasVencimientoTramiteRepository]
    S --> F[ListaDiasFeriadosTramiteRepository]
    S --> R[AppResponses<FechaLimiteRespuestaDto>]
```

## Diagrama de clases

```mermaid
classDiagram
    class TramiteController {
      +SolicitaFechaLimiteRespuesta(int idTipoTramite)
    }
    class IFechaLimiteRespuestaService {
      +SolicitaFechaLimiteRespuesta(int idTipoTramite, string defaultDbAlias)
    }
    class FechaLimiteRespuestaService
    class ISystemPlantillaRadicadoR
    class ITotalDiasVencimientoTramiteRepository
    class IListaDiasFeriadosTramiteRepository
    class FechaLimiteRespuestaDto {
      +int IdTipoTramite
      +int IdPlantilla
      +int DiasVencimiento
      +string FechaLimiteRespuesta
    }
    TramiteController --> IFechaLimiteRespuestaService
    FechaLimiteRespuestaService ..|> IFechaLimiteRespuestaService
    FechaLimiteRespuestaService --> ISystemPlantillaRadicadoR
    FechaLimiteRespuestaService --> ITotalDiasVencimientoTramiteRepository
    FechaLimiteRespuestaService --> IListaDiasFeriadosTramiteRepository
    FechaLimiteRespuestaService --> FechaLimiteRespuestaDto
```

## Diagrama de secuencia

```mermaid
sequenceDiagram
    participant U as Usuario/API Client
    participant C as TramiteController
    participant S as FechaLimiteRespuestaService
    participant P as PlantillaRepository
    participant D as DiasVencimientoRepository
    participant F as FeriadosRepository

    U->>C: GET /api/tramite/tramites/solicitaFechaLimiteRespuesta?idTipoTramite=...
    C->>S: SolicitaFechaLimiteRespuesta(idTipoTramite, defaultDbAlias)
    S->>P: SolicitaEstructuraPlantillaRadicacionDefault(alias)
    P-->>S: id_Plantilla
    S->>D: SolicitaTotalDiasVencimientoTramite(idPlantilla, idTipoTramite, alias)
    D-->>S: numero_dias_vence
    S->>F: SolicitaListaDiasFeriados(alias)
    F-->>S: lista de fechas
    S-->>C: AppResponses<FechaLimiteRespuestaDto>
    C-->>U: 200/400 + payload
```

## Diagrama de estado

```mermaid
stateDiagram-v2
    [*] --> ValidandoAlias
    ValidandoAlias --> SinResultados: alias invalido / plantilla no encontrada
    ValidandoAlias --> ConsultandoPlantilla: alias valido
    ConsultandoPlantilla --> ConsultandoDias: plantilla encontrada
    ConsultandoPlantilla --> SinResultados: plantilla no encontrada
    ConsultandoDias --> SinResultados: dias <= 0
    ConsultandoDias --> ConsultandoFeriados: dias > 0
    ConsultandoFeriados --> CalculandoFecha
    CalculandoFecha --> Exitoso
    ValidandoAlias --> ErrorControlado: excepcion
    ConsultandoPlantilla --> ErrorControlado: excepcion
    ConsultandoDias --> ErrorControlado: excepcion
    ConsultandoFeriados --> ErrorControlado: excepcion
    ErrorControlado --> [*]
    SinResultados --> [*]
    Exitoso --> [*]
```
