## 1. Refinamiento de OpenSpec

- [x] 1.1 Refinar `design.md` con alcance/no alcance, decisiones arquitectónicas y política de compatibilidad.
- [x] 1.2 Refinar `specs/jira-scrum-205/spec.md` con escenarios medibles para query/action y modos de vista.
- [x] 1.3 Ajustar `tasks.md` a una secuencia de implementación por capas y validación.

## 2. Discovery técnico previo a código

- [x] 2.1 Confirmar rutas exactas de Controller/Service/Repository/DTO en repos satélite.
- [x] 2.2 Confirmar fuente legacy y estructura SQL base para `lista_documentos_relacionados`.
- [x] 2.3 Verificar contratos existentes reutilizables (`DynamicUiTableDto`, `DynamicUiRowsOnlyDto`, `AppResponses<T>`).

## 3. Contratos y capa API

- [x] 3.1 Crear/ajustar DTOs:
  - `ListaDocumentosRadicadosTreeQueryRequestDto`
  - `ListaDocumentosRadicadosTreeActionRequestDto`
  - `ListaDocumentosRadicadosTreeMutationResultDto`
  - `ListaDocumentosRadicadosResolveRequestDto`
- [x] 3.2 Implementar `ListaDocumentosRadicadoController` con endpoints `query` y `action`.
- [x] 3.3 Validar claims `defaulalias` y `usuarioid` con fallos controlados (`BadRequest`/seguridad).

## 4. Capa Service

- [x] 4.1 Implementar `SolicitaListaDocumentosRadicadosTreeAsync` con soporte `hierarchical` y `flatDocuments`.
- [x] 4.2 Implementar `EjecutaAccionListaDocumentosRadicadosTreeAsync` con dispatch inicial:
  - `ver_documento`
  - `agregar_item`
  - `eliminar_item`
- [x] 4.3 Integrar acción `ver_documento` con `/api/gestor-documental/documentos/visualizacion/resolve`.
- [x] 4.4 Garantizar `AppResponses<T>` y `try/catch` en todos los caminos críticos.

## 5. Capa Repository y migración legacy

- [x] 5.1 Implementar consulta parametrizada con `DapperCrudEngine + QueryOptions`.
- [x] 5.2 Migrar mapping legacy de campos `ID`, `DBT`, `PAG`, `TIPODOCUMENTO`, `ESTADO_FIRMA_DIGITAL` a `Values/Meta`.
- [x] 5.3 Validar que no exista SQL manual, ni contratos bootstrap-table, ni respuestas string legacy.

## 6. DI, AutoMapper y observabilidad

- [x] 6.1 Registrar servicios en `Program.cs` bajo `// Services (L)`.
- [x] 6.2 Registrar repositorios en `Program.cs` bajo `// Repositories (R)`.
- [x] 6.3 Registrar perfiles de mapping requeridos y logs mínimos (`Information`, `Warning`, `Error`).

## 7. Pruebas

- [x] 7.1 Controller tests: claims inválidos, query success, action success.
- [x] 7.2 Service unit tests: `hierarchical` raíz/hijos, `flatDocuments`, acción inválida y acciones soportadas.
- [x] 7.3 Repository integration tests: consulta legacy parametrizada y shape esperado.
  - Cobertura: `ListaDocumentosRadicadosRepositoryTests` (captura `QueryOptions`, whitelist filtros/sort y shape de respuesta).
  - Integración Docker/Testcontainers: `ListaDocumentosRadicadosRepositoryIntegrationTests` (marcado `Skip` por prerequisito de entorno).
- [x] 7.4 Contract/regression tests: shape `AppResponses<T>` y no regresión de inbox/autocomplete/export.
  - Cobertura: `ListaDocumentosRadicadosContractRegressionTests` (envelope `AppResponses` query/action + rutas públicas `workflowInboxgestion`/`autocomplete`/`/api/AppTable/export`).
- [x] 7.5 Ejecutar `dotnet test` (o marcar integración `Skipped` si Docker no está disponible).
  - Evidencia: `FullyQualifiedName~ListaDocumentosRadicado|FullyQualifiedName~ListaDocumentosRadicados` -> 14/14 OK + 1 skipped (integración Docker).
  - Evidencia: suite completa `TramiteDiasVencimiento.Tests` -> 606 total, 593 OK, 10 skipped, 3 failed (fallas preexistentes fuera del alcance SCRUM-205).

## 8. Documentación técnica

- [x] 8.1 Crear carpeta de documentación: `Docs/GestorDocumental/Documentos/ListaDocumentosRadicados/`.
- [x] 8.2 Crear `SCRUM-205-Arquitectura.md` con:
  - diagrama de capas (`Controller -> Service -> Repository`)
  - diagrama de secuencia para `query`
  - diagrama de secuencia para `action`
  - matriz de decisiones (`hierarchical` vs `flatDocuments`)
  - validación SOLID aplicada por componente
- [x] 8.3 Crear `SCRUM-205-Implementacion-Detallada.md` con:
  - lista de archivos creados/modificados por repo
  - firmas finales de Controller/Service/Repository/DTOs
  - configuración DI (`Program.cs`) y registros AutoMapper
  - estrategia `DapperCrudEngine + QueryOptions` usada
  - tabla de mapeo legacy -> contrato nuevo (`Values`/`Meta`)
- [x] 8.4 Crear `SCRUM-205-Integracion-Frontend.md` con:
  - payload completo de `query` (ejemplos `hierarchical` y `flatDocuments`)
  - payload completo de `action` por cada `ActionId` soportado
  - respuesta exitosa/error de `query`
  - respuesta exitosa/error de `action`
  - ejemplo de `DocumentResolveRequest` para `ver_documento`
- [x] 8.5 Crear `SCRUM-205-Pruebas.md` con:
  - plan de pruebas unitarias por capa
  - plan de pruebas de integración (incluyendo prerequisitos Docker/Testcontainers)
  - casos de regresión sobre inbox/autocomplete/export
  - evidencia de ejecución (`dotnet test`, resultados y/o skips justificados)
- [x] 8.6 Crear `SCRUM-205-Observabilidad.md` con:
  - catálogo de logs (`Information`, `Warning`, `Error`)
  - campos obligatorios (`requestId`, `usuarioid`, `alias`, `ViewMode`, `actionId`, `rowCount`, `duracionMs`)
  - métricas sugeridas (latencia, tasa de error, filas por consulta)
  - guía de troubleshooting para fallos típicos
- [x] 8.7 Crear `SCRUM-205-Seguridad.md` con:
  - validación de claims (`defaulalias`, `usuarioid`)
  - controles de entrada (`ViewMode`, `ActionId`, ids y paginación)
  - evidencia de parametrización SQL y no uso de SQL manual
  - manejo de errores sin exponer stacktrace
- [x] 8.8 Crear `SCRUM-205-Metadata.md` con:
  - ticket Jira, autores, fecha, versión del documento
  - repos impactados y estado por repo (coordinador/satélite)
  - links a PRs relacionados
  - estado de cumplimiento de criterios de aceptación
- [x] 8.9 Revisar consistencia cruzada entre `design.md`, `spec.md`, `tasks.md` y documentación técnica final.

## 9. Cierre OpenSpec

- [x] 9.1 Ejecutar `openspec.cmd validate scrum-205-crea-api-lista-documentos-radicados`.
- [x] 9.2 Publicar avance multi-repo (`orchestrate:publish`) al completar implementación.
- [ ] 9.3 Archivar (`orchestrate:archive`) después de merge multi-repo.
