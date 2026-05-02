# SCRUM-168 Pruebas Gabinete Inventario

## Matriz unitaria objetivo
- Validaciones de `GabineteInsertModel`:
  - nombre de gabinete vacio o invalido.
  - columnas dinamicas vacias, invalidas o duplicadas.
  - id/folio/carpeta/tipo fuera de rango.
- Validaciones de `InventarioInsertModel`:
  - campos obligatorios invalidos.
  - truncado seguro de `FullText` si supera limite.
  - id generado invalido.
- Coordinador:
  - orden de fases (reserve -> gabinete -> inventario -> commit).
  - rollback si falla inventario.
  - no ejecutar inventario si falla gabinete.

## Matriz integracion objetivo
- Commit exitoso persiste ambas inserciones.
- Error posterior a gabinete revierte ambas tablas.
- Transaccion compartida en ambas operaciones.
- Validacion de identificadores dinamicos contra inyeccion.

## Matriz concurrencia objetivo
- Ejecuciones paralelas no deben colisionar `IdAlmacen`.
- Fallo de una transaccion no contamina otra.
- Locks mantienen consistencia de secuencia.

## Matriz QT/calidad objetivo
- Sin `SELECT *` en rutas criticas.
- Sin `ExecuteAsync/QueryAsync/ExecuteScalarAsync` directo en repositories (si aplica DapperCrudEngine).
- Logs sin datos sensibles (`FullText`, payload documental).

## Evidencia de la corrida SCRUM-168 (coordinador)
- En `DocuArchiCore` se completo y valido OpenSpec del cambio.
- Se implementaron pruebas unitarias en `tests/TramiteDiasVencimiento.Tests` para:
  - `StorageTransactionCoordinator`
  - `GabineteStorageRepository`
  - `InventarioDocumentalRepository`
- Se implementaron pruebas de integracion/concurrencia en:
  - `StorageTransactionCoordinatorIntegrationTests`
- `MiApp.Repository` ya cuenta con PR funcional publicado: https://github.com/miguelurueta/MiApp.Repository/pull/58.

## Cobertura actual vs pendiente
- Cubierto en esta fase:
  - pruebas unitarias de validacion de modelos y de flujo transaccional coordinator.
  - pruebas unitarias de reglas de insercion/guardas en repositorios.
  - trazabilidad de arquitectura, alcance y gating de flujo orquestado.
- Pendiente:
  - validacion E2E en infraestructura completa (DB real + pipeline completa).
  - cierre del bloqueo de build de referencias MSBuild para recuperar corrida integral de toda la suite en este entorno.
