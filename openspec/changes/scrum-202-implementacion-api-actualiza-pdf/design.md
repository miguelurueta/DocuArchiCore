## Context

- Jira issue key: SCRUM-202
- Jira summary: IMPLEMENTACION-API-ACTUALIZA-PDF
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-202
- Referencias:
  - openspec/context/multi-repo-context.md
  - openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se necesita permitir que el frontend reemplace el PDF final de un documento ya almacenado, usando el archivo editado que llega por upload temporal, sin introducir una segunda infraestructura de rutas ni de seguridad.

El reemplazo debe:
- Reusar resolución de ruta física ya existente en StorageEngine.
- Reusar resolución de ruta temporal ya existente (`rutaTemporalId` + `archivoTemporalId`).
- Guardar respaldo previo del archivo reemplazado en una subruta de temporales.
- Registrar auditoría funcional/técnica en `logdocuarchi`.

No se requiere en este ticket migrar todo el flujo legacy de versionado documental completo.

## Scope

### In scope

- Nuevo endpoint backend de reemplazo de contenido PDF.
- Servicio de aplicación para orquestar validación, backup, replace y auditoría.
- Repositorio para inserción en `logdocuarchi` usando `DapperCrudEngine + QueryOptions`.
- DTOs request/response para contrato frontend.
- Documentación y pruebas del flujo de reemplazo.

### Out of scope

- Implementación integral de versionado legacy (`ra_ver_control_version_documento`, `ra_ver_version_documento`) para cada modificación.
- Rediseño del StorageEngine actual.
- Tablas nuevas de auditoría.

## Architecture

- Patrón obligatorio: `Controller -> Service -> Repository`.
- Controller:
  - validar request HTTP.
  - resolver claims/autenticación existentes.
  - devolver `AppResponses<T>`.
- Service:
  - resolver archivo temporal.
  - resolver documento físico actual.
  - calcular hash/tamaño anterior y nuevo (reusando `CryptoHelper`).
  - crear backup en temporal.
  - reemplazar archivo final.
  - registrar auditoría.
- Repository:
  - operaciones DB por `DapperCrudEngine` y `QueryOptions`.

## Routing and Path Reuse Strategy

- Reusar componentes existentes de ruta física del StorageEngine para ubicar el documento destino.
- Reusar `StoragePathResolver` (o equivalente actual) para resolver y validar archivo temporal.
- Reusar validaciones anti-path-traversal existentes.
- No aceptar rutas físicas desde frontend.

## Backup Strategy

- Root de backup: reutilizar root temporal ya existente.
- Subruta obligatoria:
  - `replacement-versions/{gabinete}/{idDocumento}/{yyyyMMddHHmmss}/`
- El backup se crea antes del replace.
- Si falla creación de backup, no se reemplaza.

## Audit Strategy (logdocuarchi)

- Insertar registro con `desc_op` de reemplazo documental.
- Persistir en `CAMPOS` un JSON con:
  - `idDocumento`
  - `rutaTemporalId`
  - `archivoTemporalId`
  - `rutaArchivoOriginal`
  - `rutaRespaldo`
  - `hashAnterior`
  - `hashNuevo`
  - `tamanoAnterior`
  - `tamanoNuevo`
  - `motivo`
- Completar `IP_TRANS` reutilizando mecanismo existente del proyecto.

## Error and Consistency Model

- Modelo híbrido DB/FS (sin transacción ACID única).
- Orden recomendado:
  1) Validaciones.
  2) Backup.
  3) Replace físico.
  4) Insert auditoría.
- Si falla replace físico: retornar error, sin auditoría de éxito.
- Si replace físico fue exitoso y falla auditoría: retornar error controlado y loggear inconsistencia para soporte.

## Testing Strategy

- Unit tests:
  - request inválido.
  - temporal inexistente.
  - extensión no permitida.
  - documento destino inexistente.
  - backup exitoso.
  - replace exitoso.
  - error de replace.
  - error de auditoría.
- Integration tests:
  - flujo completo upload temporal -> replace -> logdocuarchi.
- Regresión:
  - no romper flujo actual de almacenamiento.

## Documentation Deliverables

- Documento técnico de implementación del endpoint.
- Contrato frontend con ejemplo completo.
- Observabilidad/troubleshooting.
- Runbook de recuperación ante falla en replace/auditoría.
