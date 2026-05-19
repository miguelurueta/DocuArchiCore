## 1. Refinamiento OpenSpec

- [x] 1.1 Refinar `design.md` con alcance, no-alcance, contratos API y reglas de precedencia.
- [x] 1.2 Refinar `specs/jira-scrum-206/spec.md` con escenarios verificables por endpoint.
- [x] 1.3 Ajustar `tasks.md` a secuencia real de implementación backend por capas.

## 2. Discovery técnico previo a código

- [ ] 2.1 Confirmar rutas finales en repos satélite: Controller, Service, Repository y DTOs.
- [ ] 2.2 Confirmar estrategia de autorización admin para endpoints de terceros (claim/política existente).
- [ ] 2.3 Verificar tablas `ra_vis_per_*` y llaves requeridas para resolver precedencia y overrides por usuario.

## 3. Contratos y capa API

- [ ] 3.1 Definir/crear DTOs de contrato:
  - `VisorPdfPermissionsResponseDto`
  - `UpsertUserOverridesRequestDto`
  - `PermissionOverrideItemDto`
  - `SimpleOperationResultDto`
- [ ] 3.2 Implementar `PermisosVisorPdfController` con endpoints:
  - `GET implementaciones/{codigoImpl}/mis-permisos`
  - `GET implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
  - `PUT implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
  - `DELETE implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`
- [ ] 3.3 Validar claims obligatorios (`defaulalias`, `usuarioid`) y entradas (`codigoImpl`, `idUsuario`, `codigoPermiso`).
- [ ] 3.4 Asegurar que el contrato no exija `codiperfil` en requests de consumo.

## 4. Capa Service

- [ ] 4.1 Implementar resolución efectiva con precedencia:
  - override usuario > perfil activo > default implementación > deny
- [ ] 4.2 Implementar consulta de permisos para usuario autenticado.
- [ ] 4.3 Implementar consulta de permisos para usuario objetivo (admin).
- [ ] 4.4 Implementar mutaciones de overrides (`upsert` y `delete`) con respuestas controladas.
- [ ] 4.5 Garantizar `AppResponses<T>` y `try/catch` en flujos de éxito/error.

## 5. Capa Repository

- [ ] 5.1 Implementar lecturas de catálogo/permisos efectivos usando `DapperCrudEngine + QueryOptions`.
- [ ] 5.2 Implementar mutaciones de overrides con operación transaccional y parámetros seguros.
- [ ] 5.3 Resolver perfil activo del usuario de forma interna (sin `codiperfil` en API).
- [ ] 5.4 Verificar no uso de SQL manual ni concatenación insegura.

## 6. DI, AutoMapper y observabilidad

- [ ] 6.1 Registrar servicios en `Program.cs` bajo `// Services (L)`.
- [ ] 6.2 Registrar repositorios en `Program.cs` bajo `// Repositories (R)`.
- [ ] 6.3 Registrar perfiles AutoMapper necesarios.
- [ ] 6.4 Agregar logs mínimos (`Information`, `Warning`, `Error`) con campos de trazabilidad.

## 7. Pruebas

- [ ] 7.1 Controller tests:
  - claims faltantes/invalidos
  - endpoint admin sin autorización
  - contratos `AppResponses<T>` en success/error
- [ ] 7.2 Service unit tests:
  - precedencia completa
  - fallback deny
  - implementación inexistente
  - errores de repositorio
- [ ] 7.3 Repository integration tests:
  - escenarios default/perfil/override
  - persistencia delete/upsert
  - parametrización de consultas
- [ ] 7.4 Contract/regression tests:
  - shape de respuesta
  - no regresión en endpoints ya existentes del módulo
- [ ] 7.5 Ejecutar `dotnet test` o marcar integración `Skipped` con motivo explícito si Docker no está disponible.

## 8. Documentación técnica

- [ ] 8.1 Crear/actualizar en `Docs/Seguridad/PermisosVisorPdf/` los documentos estilo SCRUM-202.
- [ ] 8.2 Incluir contratos request/response completos por endpoint.
- [ ] 8.3 Incluir matriz de permisos del visor PDF (`pdf.view`, `pdf.print`, etc.) por implementación.
- [ ] 8.4 Incluir guía de consumo frontend para implementación por usuario individual.

## 9. Cierre OpenSpec

- [ ] 9.1 Ejecutar `openspec.cmd validate scrum-206-implementacion-api-permiso-visor-pdf`.
- [ ] 9.2 Publicar estado en flujo multi-repo (`orchestrate:publish`) al completar implementación.
- [ ] 9.3 Archivar (`orchestrate:archive`) después de merge multi-repo.
