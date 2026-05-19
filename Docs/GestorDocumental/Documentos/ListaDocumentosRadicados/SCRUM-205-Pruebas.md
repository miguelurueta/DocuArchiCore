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

