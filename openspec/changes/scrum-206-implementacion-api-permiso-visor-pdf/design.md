## Context

- Jira issue key: SCRUM-206
- Jira summary: IMPLEMENTACION-API-PERMISO-VISOR-PDF
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-206

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md
- Docs/Seguridad/PermisosVisorPdf/ra_vis_per_schema.sql
- Docs/Seguridad/PermisosVisorPdf/ra_vis_per_diccionario_datos.md

## Problem Statement

Se requiere una API backend para exponer permisos efectivos del visor PDF en múltiples implementaciones frontend
(`workflow`, `gestion_correspondencia`, otras futuras), resolviendo permisos por usuario con precedencia determinística y
seguridad basada en claims.

El frontend necesita consumir un mapa simple `{ "pdf.xxx": true|false }` para decidir render, ocultar o deshabilitar
acciones del componente visor.

## Scope

### In Scope

- Endpoint de consulta de permisos del usuario autenticado por implementación.
- Endpoint de consulta de permisos de un usuario específico (uso administrativo).
- Endpoint para upsert de overrides por usuario/implementación.
- Endpoint para eliminar override específico.
- Resolución efectiva con precedencia:
  1. override usuario
  2. perfil usuario (si existe asignación vigente)
  3. default implementación
  4. fallback deny
- Uso obligatorio de `DapperCrudEngine + QueryOptions`.
- Contrato de salida uniforme `AppResponses<T>`.

### Out of Scope

- UI administrativa.
- ABM completo de implementaciones/perfiles/catálogo de permisos.
- Integración con proveedor externo IAM.

## Target Architecture

- Patrón: `Controller -> Service -> Repository`.
- Claims obligatorios: `defaulalias`, `usuarioid`.
- Seguridad administrativa para operaciones sobre terceros (consultar otro usuario y mutaciones de overrides).
- `try/catch` en controller, service y repository.

Rutas esperadas:
- Controller: `DocuArchi.Api/Controllers/GestorDocumental/PermisosVisorPdf/PermisosVisorPdfController.cs`
- Service: `MiApp.Services/Service/GestorDocumental/PermisosVisorPdf/*`
- Repository: `MiApp.Repository/Repositorio/GestorDocumental/PermisosVisorPdf/*`
- DTOs: `MiApp.DTOs/DTOs/GestorDocumental/PermisosVisorPdf/*`

## API Contract (Detailed)

Base route:
- `/api/gestor-documental/permisos-visorpdf`

### 1) Mis permisos por implementación
- `GET /implementaciones/{codigoImpl}/mis-permisos`
- Source usuario: claim `usuarioid`.
- Respuesta: `AppResponses<VisorPdfPermissionsResponseDto>`

### 2) Permisos de usuario específico (admin)
- `GET /implementaciones/{codigoImpl}/usuarios/{idUsuario}/permisos`
- Requiere autorización administrativa.
- Respuesta: `AppResponses<VisorPdfPermissionsResponseDto>`

Nota de contrato:
- El consumo es por usuario (`usuarioid` claim o `idUsuario` ruta). La API no requiere `codiperfil` como parámetro de entrada.
- Si existe perfil activo, su uso es interno al motor de resolución.

### 3) Upsert overrides usuario (admin)
- `PUT /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides`
- Request: `UpsertUserOverridesRequestDto`
- Respuesta: `AppResponses<SimpleOperationResultDto>`

### 4) Eliminar override usuario (admin)
- `DELETE /implementaciones/{codigoImpl}/usuarios/{idUsuario}/overrides/{codigoPermiso}`
- Respuesta: `AppResponses<SimpleOperationResultDto>`

## DTO Decisions

- `VisorPdfPermissionsResponseDto`
  - `CodigoImplementacion: string`
  - `IdUsuario: int`
  - `Permissions: Dictionary<string, bool>`
  - `Sources: Dictionary<string, string>` (opcional, trazabilidad)
  - `GeneratedAt: DateTime`
- `UpsertUserOverridesRequestDto`
  - `Overrides: List<PermissionOverrideItemDto>`
- `PermissionOverrideItemDto`
  - `CodigoPermiso: string`
  - `Permitido: int (0|1)`
  - `Motivo: string?`
- `SimpleOperationResultDto`
  - `CodigoImplementacion: string`
  - `IdUsuario: int`
  - `Procesados: int?`

## Data Access Strategy

- Resolver permisos sobre catálogo activo `ra_vis_per_permiso (estado=1)`.
- Join de resolución efectiva usando tablas `ra_vis_per_*` y vigencia de `ra_vis_per_usuario_perfil`.
- Mutaciones de overrides con operación transaccional.
- Validar `codigoImpl` con regex segura `^[A-Za-z0-9_]+$`.
- Resolver `codiperfil` solo desde persistencia cuando aplique, nunca desde payload del consumidor.

## Security Rules

- `defaulalias` y `usuarioid` son obligatorios.
- Alias de DB se toma exclusivamente de `defaulalias`.
- Usuario autenticado se toma exclusivamente de `usuarioid`.
- Endpoints administrativos:
  - `GET .../usuarios/{idUsuario}/permisos`
  - `PUT .../overrides`
  - `DELETE .../overrides/{codigoPermiso}`
  requieren check explícito de autorización (claim/permiso administrativo).

## Observability and Error Handling

Logs mínimos:
- `Information`: inicio/fin resolución permisos, cantidad permisos.
- `Warning`: implementación o usuario inexistente, sin asignación vigente.
- `Error`: excepción técnica o rollback.

Campos:
- `requestId`, `usuarioid`, `alias`, `codigoImpl`, `idUsuarioObjetivo`, `permisosTotales`, `duracionMs`.

## Testing Strategy

- Controller tests:
  - claims inválidos
  - no admin en endpoints admin
  - success response contract
- Service tests:
  - precedencia override > perfil > default > deny
  - vigencia perfil
  - implementación inexistente
- Repository tests:
  - query options correctos
  - mutación overrides
  - merge de fuentes
- Integration tests:
  - schema/seed mínimo `ra_vis_per_*`
  - caso default, perfil, override, fallback deny
- Regression:
  - sin impacto en workflow inbox/autocomplete/export.

## Documentation Deliverables

Ruta:
- `Docs/Seguridad/PermisosVisorPdf/`

Archivos objetivo estilo SCRUM-202:
- `SCRUM-206-Arquitectura-Final-PermisosVisorPdf.md`
- `SCRUM-206-Diagramas-PermisosVisorPdf.md`
- `SCRUM-206-Implementacion-PermisosVisorPdf.md`
- `SCRUM-206-Integracion-Frontend-PermisosVisorPdf.md`
- `SCRUM-206-Pruebas-PermisosVisorPdf.md`
- `SCRUM-206-Observabilidad-PermisosVisorPdf.md`
- `SCRUM-206-Seguridad-PermisosVisorPdf.md`
- `SCRUM-206-Metadata.md`

## Approach

- Refinar specs y tareas con escenarios verificables y contratos explícitos.
- Implementar por capas cumpliendo OPSXJ_BACKEND_RULES.
- Validar OpenSpec con `openspec.cmd validate scrum-206-implementacion-api-permiso-visor-pdf`.
