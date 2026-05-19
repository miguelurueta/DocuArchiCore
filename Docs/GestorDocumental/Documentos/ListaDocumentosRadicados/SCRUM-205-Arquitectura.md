# SCRUM-205 Arquitectura ListaDocumentosRadicados

## 1. Objetivo
Definir la arquitectura backend para listar documentos radicados en modo arbol (`hierarchical`) y modo plano (`flatDocuments`), con acciones por fila y migracion controlada de comportamiento legacy.

## 2. Alcance
- Endpoint `query`: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`.
- Endpoint `action`: `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`.
- Contrato de salida con `AppResponses<T>` y objetos dinamicos de tabla.
- Integracion de accion `ver_documento` con endpoint existente `visualizacion/resolve`.

Fuera de alcance:
- Cambios de frontend.
- Nuevos endpoints de preview documental.
- Cambios estructurales de esquema DB no necesarios para la migracion.

## 3. Diagrama de Capas
```text
HTTP Request
   |
   v
ListaDocumentosRadicadoController
   |
   v
ListaDocumentosRadicadoService
   |
   v
ListaDocumentosRadicadosRepository
   |
   v
DapperCrudEngine + QueryOptions + MySQL
```

## 4. Diagrama de Secuencia Query
```text
Frontend -> Controller(query)
Controller -> ClaimsValidator(defaulalias, usuarioid)
Controller -> Service.SolicitaListaDocumentosRadicadosTreeAsync(...)
Service -> Repository.QueryListaDocumentos(...)
Repository -> DapperCrudEngine(QueryOptions parametrizado)
DapperCrudEngine -> Repository(rows)
Repository -> Service(rows normalizados)
Service -> Controller(AppResponses<object>)
Controller -> Frontend(200 con DynamicUiTableDto/DynamicUiRowsOnlyDto)
```

## 5. Diagrama de Secuencia Action
```text
Frontend -> Controller(action)
Controller -> ClaimsValidator(defaulalias, usuarioid)
Controller -> Service.EjecutaAccionListaDocumentosRadicadosTreeAsync(...)
Service -> Dispatch(ActionId)
Service -> (ver_documento) construir DocumentResolveRequest
Service -> (agregar_item/eliminar_item) ejecutar mutacion controlada
Service -> Controller(AppResponses<ListaDocumentosRadicadosTreeMutationResultDto>)
Controller -> Frontend(200/400 con contrato estandar)
```

## 6. Matriz de Decision ViewMode
| Regla | hierarchical | flatDocuments |
|---|---|---|
| `ParentRowId` | Requerido para expansion de hijos | Ignorado |
| `Level` | Usado para expansion por nivel | Ignorado funcionalmente |
| `NodeType` | Depende de tipo de nodo | Siempre `documento` |
| `ParentId` | Segun jerarquia | `null` |
| `HasChildren` | Segun datos de negocio | `false` |
| Forma de salida | Arbol parcial por contexto | Lista plana de documentos |

## 7. SOLID aplicado
- SRP: Controller solo HTTP, Service solo orquestacion/reglas, Repository solo acceso de datos.
- OCP: dispatch de `ActionId` extensible sin romper contrato base.
- LSP: interfaces de Service/Repository intercambiables para pruebas.
- ISP: interfaces enfocadas por caso de uso.
- DIP: dependencias via DI en `Program.cs`.

## 8. Reglas No Negociables
- Acceso DB solo con `DapperCrudEngine` y `QueryOptions`.
- SQL siempre parametrizado.
- `try/catch` obligatorio en controller/service/repository.
- No usar bootstrap-table semantics ni strings de estado legacy.
- No romper endpoints existentes de inbox/autocomplete/export.

## 9. Riesgos y Mitigaciones
- Riesgo: regresion de contrato AppTable.
  - Mitigacion: pruebas de contrato y regresion.
- Riesgo: errores por claims invalidos.
  - Mitigacion: validacion temprana en controller.
- Riesgo: deriva funcional respecto a legacy.
  - Mitigacion: tabla de mapping explicita y casos de paridad.
