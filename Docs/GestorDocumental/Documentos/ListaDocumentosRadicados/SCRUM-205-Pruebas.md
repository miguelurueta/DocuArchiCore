# SCRUM-205 Pruebas ListaDocumentosRadicados

## 1. Estrategia
Cobertura por capas `Controller -> Service -> Repository` y validacion de contrato `AppResponses<T>`.

## 2. Unit Tests
### 2.1 Controller
- Claim `defaulalias` ausente -> error controlado.
- Claim `usuarioid` invalido -> error controlado.
- `query` success -> retorno `AppResponses<object>`.
- `action` success -> retorno `AppResponses<ListaDocumentosRadicadosTreeMutationResultDto>`.

### 2.2 Service Query
- `ViewMode=hierarchical` nivel raiz.
- `ViewMode=hierarchical` expansion hijos por `ParentRowId`.
- `ViewMode=flatDocuments` salida plana con metadata fija.
- `IncludeConfig=true` retorna estructura de tabla.
- `IncludeConfig=false` retorna solo filas.

### 2.3 Service Action
- `ver_documento` retorna `Operation=view` + `DocumentResolveRequest`.
- `agregar_item` retorna `Operation=added`.
- `eliminar_item` retorna `Operation=deleted`.
- `ActionId` invalido retorna `success=false`.

## 3. Integration Tests
- Repository con `DapperCrudEngine + QueryOptions` sobre MySQL de pruebas.
- Validar mapping legacy de `ID`, `DBT`, `PAG`, `TIPODOCUMENTO`, `ESTADO_FIRMA_DIGITAL`.
- Validar SQL parametrizado sin concatenacion.

## 4. Contract Tests
- Shape de `AppResponses<T>`:
  - `success`
  - `message`
  - `data`
  - `meta`
  - `errors`
- Validar nodos `Meta` obligatorios por fila.

## 5. Regresion
- No degradar:
  - `POST /api/workflowInboxgestion/inboxgestion`
  - `POST /api/workflowInboxgestion/inboxgestion/autocomplete`
  - `POST /api/AppTable/export`

## 6. Ejecucion
Comandos objetivo:
```powershell
dotnet test DocuArchiCore.sln -c Debug
```

Si Docker no esta disponible para integracion:
- Marcar casos integration como `Skipped` con mensaje explicito.

## 7. Evidencia
- Adjuntar resultados de `dotnet test`.
- Registrar cantidad de pruebas por suite y estado final.
- Vincular evidencia a PR y a `SCRUM-205-Metadata.md`.

### 7.1 Ejecucion real (2026-05-19)
Comando (focalizado SCRUM-205):
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~ListaDocumentosRadicado"
```
Resultado:
- Total: 7
- Superadas: 7
- Fallidas: 0
- Omitidas: 0

Comando (suite completa del proyecto de pruebas):
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj
```
Resultado:
- Total: 606
- Superadas: 593
- Fallidas: 3
- Omitidas: 10

Detalle de fallas observadas en suite completa (fuera del alcance SCRUM-205):
- `ServiceSincronizaEditorDocumentImagesTests.SincronizaAsync_CuandoRepoOk_RetornaOk`
- `InitialContentEditorControllerTests.GetInitialContent_CuandoClaimInvalido_RetornaBadRequest`
- `InitialContentEditorControllerTests.GetInitialContent_CuandoServiceOk_RetornaOk`

Notas:
- Se observaron pruebas de integracion marcadas como `SKIP` por dependencia de entorno Docker/Testcontainers.
- En restauracion aparece advertencia `NU1900` por acceso al indice de vulnerabilidades de NuGet (`https://api.nuget.org/v3/index.json`).

### 7.2 Cobertura adicional repository + regression (2026-05-19)
Comando (suite focalizada con repository/contract/regression de SCRUM-205):
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~ListaDocumentosRadicado|FullyQualifiedName~ListaDocumentosRadicados"
```
Resultado:
- Total: 15
- Superadas: 14
- Fallidas: 0
- Omitidas: 1

Detalle de la omitida:
- `ListaDocumentosRadicadosRepositoryIntegrationTests.SolicitaDocumentosRelacionadosAsync_Integracion_MySqlTestcontainers_Pendiente`
  - Motivo: requiere Docker/Testcontainers para validar tabla legacy real de gabinete.

Pruebas agregadas para este bloque:
- `ListaDocumentosRadicadosRepositoryTests`
- `ListaDocumentosRadicadosRepositoryIntegrationTests`
- `ListaDocumentosRadicadosContractRegressionTests`
