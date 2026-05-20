## 1. Refinamiento OpenSpec

- [x] 1.1 Refinar `design.md` con alcance, no-alcance, contratos API y reglas de precedencia.
- [x] 1.2 Refinar `specs/jira-scrum-206/spec.md` con escenarios verificables por endpoint.
- [x] 1.3 Ajustar `tasks.md` a secuencia real de implementación backend por capas.

## 2. Discovery técnico previo a código

- [x] 2.1 Confirmar rutas finales en repos satélite: Controller, Service, Repository y DTOs.
- [x] 2.2 Confirmar estrategia de autorización admin para endpoints de terceros (claim/política existente).
- [x] 2.3 Verificar tablas `ra_vis_per_*` y llaves requeridas para resolver precedencia y overrides por usuario.

## 3. Contratos y capa API

- [x] 3.1 Definir/crear DTOs de contrato:
  - `VisorPdfPermissionsResponseDto`
  - `UpsertUserOverridesRequestDto`
  - `PermissionOverrideItemDto`
  - `SimpleOperationResultDto`
- [x] 3.2 Implementar `PermisosVisorPdfController` con endpoints:
  - `GET implementaciones/{codigoImpl}/mis-permisos`
  - `GET implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
  - `PUT implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
  - `DELETE implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`
- [x] 3.3 Validar claims obligatorios (`defaulalias`, `usuarioid`) y entradas (`codigoImpl`, `idUsuario`, `codigoPermiso`).
- [x] 3.4 Asegurar que el contrato no exija `codiperfil` en requests de consumo.

## 4. Capa Service

- [x] 4.1 Implementar resolución efectiva con precedencia:
  - override usuario > perfil activo > default implementación > deny
- [x] 4.2 Implementar consulta de permisos para usuario autenticado.
- [x] 4.3 Implementar consulta de permisos para usuario objetivo (admin).
- [x] 4.4 Implementar mutaciones de overrides (`upsert` y `delete`) con respuestas controladas.
- [x] 4.5 Garantizar `AppResponses<T>` y `try/catch` en flujos de éxito/error.

## 5. Capa Repository

- [x] 5.1 Implementar lecturas de catálogo/permisos efectivos usando `DapperCrudEngine + QueryOptions`.
- [x] 5.2 Implementar mutaciones de overrides con operación transaccional y parámetros seguros.
- [x] 5.3 Resolver perfil activo del usuario de forma interna (sin `codiperfil` en API).
- [x] 5.4 Verificar no uso de SQL manual ni concatenación insegura.

## 6. DI, AutoMapper y observabilidad

- [x] 6.1 Registrar servicios en `Program.cs` bajo `// Services (L)`.
- [x] 6.2 Registrar repositorios en `Program.cs` bajo `// Repositories (R)`.
- [x] 6.3 Registrar perfiles AutoMapper necesarios.
  - N/A para esta capacidad: no hay mapeos AutoMapper nuevos en el flujo `PermisosVisorPdf` (DTOs construidos de forma directa).
- [x] 6.4 Agregar logs mínimos (`Information`, `Warning`, `Error`) con campos de trazabilidad.

## 7. Pruebas

- [x] 7.1 Controller tests:
  - claims faltantes/invalidos
  - endpoint admin sin autorización
  - contratos `AppResponses<T>` en success/error
- [x] 7.2 Service unit tests:
  - precedencia completa
  - fallback deny
  - implementación inexistente
  - errores de repositorio
- [ ] 7.3 Repository integration tests:
  - escenarios default/perfil/override
  - persistencia delete/upsert
  - parametrización de consultas
- [x] 7.4 Contract/regression tests:
  - shape de respuesta
  - no regresión en endpoints ya existentes del módulo
  - Validado con `PermisosVisorPdfControllerTests` (GET/PUT/DELETE) sobre contrato `AppResponses<T>`.
- [x] 7.5 Ejecutar `dotnet test` o marcar integración `Skipped` con motivo explícito si Docker no está disponible.

## 8. Documentación técnica

- [x] 8.1 Crear `Docs/Seguridad/PermisosVisorPdf/SCRUM-206-Arquitectura-Final-PermisosVisorPdf.md` con:
  - alcance/no alcance
  - arquitectura por capas (`Controller -> Service -> Repository`)
  - regla de precedencia (`override > perfil > default > deny`)
  - decision de contrato por usuario (sin `codiperfil` en request)
- [x] 8.2 Crear `Docs/Seguridad/PermisosVisorPdf/SCRUM-206-Implementacion-PermisosVisorPdf.md` con:
  - lista de artefactos objetivo por repo (Controller/Service/Repository/DTO)
  - contratos request/response completos de los 4 endpoints
  - contrato de errores y validaciones de entrada
  - lineamientos de DI (`Program.cs`) y politica `DapperCrudEngine + QueryOptions`
- [x] 8.3 Crear `Docs/Seguridad/PermisosVisorPdf/SCRUM-206-Integracion-Frontend-PermisosVisorPdf.md` con:
  - guia de consumo `mis-permisos` por implementacion
  - matriz FE de permisos del visor (`pdf.view`, `pdf.print`, `pdf.download`, firma, rotate, zoom)
  - pseudocodigo de aplicacion de permisos en componente
  - checklist de integracion por modulo (`workflow`, `gestion_correspondencia`, otros)
- [x] 8.4 Crear `Docs/Seguridad/PermisosVisorPdf/SCRUM-206-Seguridad-PermisosVisorPdf.md` con:
  - claims obligatorios (`defaulalias`, `usuarioid`)
  - controles de endpoints admin
  - validaciones de `codigoImpl`, `idUsuario`, `codigoPermiso`, `permitido`
  - mitigaciones y trazabilidad de seguridad
- [x] 8.5 Crear `Docs/Seguridad/PermisosVisorPdf/SCRUM-206-Pruebas-PermisosVisorPdf.md` con:
  - matriz de pruebas por capa (Controller/Service/Repository/Contract)
  - escenarios default/perfil/override/fallback
  - criterios de aceptacion y comando de verificacion
  - politica de `Skipped` para integracion sin Docker
- [x] 8.6 Crear `Docs/Seguridad/PermisosVisorPdf/SCRUM-206-Observabilidad-PermisosVisorPdf.md` con:
  - catalogo de eventos de log (`Information`, `Warning`, `Error`)
  - campos minimos (`requestId`, `usuarioid`, `alias`, `codigoImpl`, `idUsuarioObjetivo`)
  - metricas sugeridas y guia de troubleshooting
- [x] 8.7 Completar `SCRUM-206-Diagramas-PermisosVisorPdf.md` con:
  - diagrama de secuencia `mis-permisos`
  - diagrama de secuencia `upsert/delete override`
  - diagrama entidad-relacion simplificado `ra_vis_per_*`
- [x] 8.8 Completar `SCRUM-206-Metadata.md` con:
  - trazabilidad Jira/PR/branch/commit
  - estado multi-repo (implementation_required/traceability_only)
  - criterio de cierre y fecha de corte
- [x] 8.9 Revisar consistencia cruzada entre `design.md`, `spec.md`, `tasks.md` y documentos `SCRUM-206-*`.

## 9. Cierre OpenSpec

- [x] 9.1 Ejecutar `openspec.cmd validate scrum-206-implementacion-api-permiso-visor-pdf`.
- [x] 9.2 Publicar estado en flujo multi-repo (`orchestrate:publish`) al completar implementación.
- [ ] 9.3 Archivar (`orchestrate:archive`) después de merge multi-repo.
