# SCRUM-68 Diagramas

## Caso de uso

```text
Usuario autenticado
    |
    v
SolicitaEstructuraRutaWorkflowController
    |
    v
SolicitaEstructuraRutaWorkflowService
    |
    v
SolicitaEstructuraRutaWorkflowRepository
    |
    v
rutas_workflow (Estado_Ruta = 1)
```

## Diagrama de clases

```text
SolicitaEstructuraRutaWorkflowController
  -> ISolicitaEstructuraRutaWorkflowService

SolicitaEstructuraRutaWorkflowService
  -> ISolicitaEstructuraRutaWorkflowRepository
  -> IMapper

SolicitaEstructuraRutaWorkflowRepository
  -> IDapperCrudEngine

RutasWorkflow
SolicitaEstructuraRutaWorkflowDto
```

## Diagrama de secuencia

```text
Cliente -> Controller: GET /api/workflow/ruta-trabajo/solicita-estructura-ruta
Controller -> ClaimValidationService: ValidateClaim("defaulalias")
Controller -> Service: SolicitaEstructuraRutaWorkflowAsync(defaultDbAlias)
Service -> Repository: SolicitaEstructuraRutaWorkflowAsync(defaultDbAlias)
Repository -> DapperCrudEngine: GetAllAsync(QueryOptions)
DapperCrudEngine -> rutas_workflow: SELECT ... WHERE Estado_Ruta = 1
rutas_workflow --> DapperCrudEngine: filas activas
DapperCrudEngine --> Repository: QueryResult<RutasWorkflow>
Repository --> Service: AppResponses<List<RutasWorkflow>?>
Service --> Controller: AppResponses<List<SolicitaEstructuraRutaWorkflowDto>?>
Controller --> Cliente: 200 OK / 400 BadRequest
```

## Estado

```text
Inicio
  -> Validar claim
  -> Consultar rutas activas
  -> Sin resultados
  -> Retornar data null

Inicio
  -> Validar claim
  -> Consultar rutas activas
  -> Con resultados
  -> Retornar lista DTO

Inicio
  -> Error de validacion o excepcion
  -> Retornar error en AppResponses
```
